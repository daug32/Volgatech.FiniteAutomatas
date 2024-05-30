using System.Diagnostics;
using Grammars.Common.Grammars;
using Grammars.Common.Grammars.Extensions;
using Grammars.Common.Grammars.ValueObjects;
using Grammars.Common.Grammars.ValueObjects.GrammarRules;
using Grammars.Common.Grammars.ValueObjects.RuleDefinitions;
using Grammars.Common.Grammars.ValueObjects.Symbols;
using LinqExtensions;

namespace Grammars.Common.Convertors.Implementation.Factorization;

internal class LeftFactorizationHandler
{
    public CommonGrammar Factorize( CommonGrammar grammar )
    {
        Queue<RuleName> processedRules = new Queue<RuleName>();
        Queue<RuleName> rulesToProcess = new Queue<RuleName>().EnqueueRange( grammar.Rules.Keys );

        while ( rulesToProcess.Any() )
        {
            RuleName ruleToProcessName = rulesToProcess.Dequeue();
            if ( processedRules.Contains( ruleToProcessName ) )
            {
                continue;
            }

            GrammarRule rule = grammar.Rules[ruleToProcessName];
            List<UnitableDefinitionsGroups> unitableGroups = UnitableDefinitionsGroups.Create( rule.Name, grammar);

            if ( InlineFirstSymbolsFromNonTerminals( unitableGroups, grammar ) )
            {
                rulesToProcess.Enqueue(ruleToProcessName);
                continue;
            }

            processedRules.Enqueue( ruleToProcessName );

            var currentRuleDefinitions = rule.Definitions.ToList();
            foreach ( UnitableDefinitionsGroups unitableGroup in unitableGroups )
            {
                var newRule = new GrammarRule( RuleName.Random(), new List<RuleDefinition>() );

                RuleSymbol heading = unitableGroup.Headings.First();

                currentRuleDefinitions = currentRuleDefinitions
                    .Where( definition => !NeedToBeRemoved( definition, unitableGroup.Definitions ) )
                    .ToList()
                    .With( new RuleDefinition( new[]
                    {
                        heading,
                        RuleSymbol.NonTerminalSymbol( newRule.Name ),
                    } ) );

                foreach ( RuleDefinition oldDefinitionToMigrate in unitableGroup.Definitions )
                {
                    RuleSymbol firstSymbol = oldDefinitionToMigrate.Symbols.First();
                    if ( firstSymbol.Type == RuleSymbolType.NonTerminalSymbol )
                    {
                        throw new UnreachableException();
                    }

                    var migratedDefinition = oldDefinitionToMigrate.Symbols.ToList().WithoutFirst();
                    if ( firstSymbol.Type == RuleSymbolType.TerminalSymbol && !migratedDefinition.Any() )
                    {
                        migratedDefinition.Add(
                            RuleSymbol.TerminalSymbol(
                                TerminalSymbol.EmptySymbol() ) );
                    }

                    newRule.Definitions.Add( new RuleDefinition( migratedDefinition ) );
                }

                grammar.Rules.Add( newRule.Name, new GrammarRule( newRule.Name, newRule.Definitions ) );
                rulesToProcess.Enqueue( newRule.Name );
            }

            grammar.Rules[rule.Name] = new GrammarRule( rule.Name, currentRuleDefinitions );
        }

        grammar.RemoveAllDuplicateDefinitions();

        return grammar;
    }

    /// <summary> Returns true if has any changes</summary>
    private bool InlineFirstSymbolsFromNonTerminals(
        List<UnitableDefinitionsGroups> unitableGroups,
        CommonGrammar grammar )
    {
        bool hasChanges = false;
        foreach ( UnitableDefinitionsGroups unitableGroup in unitableGroups )
        {
            if ( unitableGroup.Definitions.Count < 2 )
            {
                continue;
            }
            
            LinkedList<RuleName> rulesToInlineQueue = BuildRuleQueue( unitableGroup, grammar );

            while ( rulesToInlineQueue.Any() )
            {
                hasChanges |= InlineFirstSymbolsPresentedInHashSet( 
                    rulesToInlineQueue.DequeueFirst(),
                    unitableGroup.Headings,
                    grammar );
            }
        }

        return hasChanges;
    }

    private bool InlineFirstSymbolsPresentedInHashSet(
        RuleName ruleToInlineName,
        HashSet<RuleSymbol> symbolsToInline, 
        CommonGrammar grammar )
    {
        bool hasChanges = false;
        
        GrammarRule ruleToInline = grammar.Rules[ruleToInlineName];
        
        List<(RuleName DefinitionOwner, int DefinitionIndex, int RuleToReplaceIndex)> ruleUsers = FindRuleUsers( ruleToInlineName, grammar );

        List<RuleDefinition> definitionsWithCommonHeadings = ruleToInline.Definitions
            .Where( definition => grammar.GetFirstSet( ruleToInline.Name, definition ).HasIntersections( symbolsToInline ) )
            .ToList();

        for ( var index = 0; index < ruleToInline.Definitions.Count; index++ )
        {
            RuleDefinition ruleDefinitionWhereToExtract = ruleToInline.Definitions[index];
            if ( !definitionsWithCommonHeadings.Contains( ruleDefinitionWhereToExtract ) )
            {
                continue;
            }
            
            hasChanges = true;
                    
            RuleSymbol symbolToExtract = ruleDefinitionWhereToExtract.FirstSymbol();
            if ( symbolToExtract.Type == RuleSymbolType.NonTerminalSymbol )
            {
                throw new UnreachableException();
            }

            // Replace ruleToInline in the definitions using this rule by symbolToExtract + ruleToInlineName
            foreach ( (RuleName DefinitionOwner, int DefinitionIndex, int RuleToReplaceIndex) ruleUser in ruleUsers )
            {
                GrammarRule ruleWhereToInline = grammar.Rules[ruleUser.DefinitionOwner];
                RuleDefinition definitionWhereToInline = ruleWhereToInline.Definitions[ruleUser.DefinitionIndex];
                
                var definitionWithReplaced = definitionWhereToInline.Symbols.ToList();
                definitionWithReplaced.Insert( ruleUser.RuleToReplaceIndex, symbolToExtract );
                ruleWhereToInline.Definitions.Add( new RuleDefinition( definitionWithReplaced ) );                
            }
                    
            // Remove start symbol
            var newDefinition = ruleDefinitionWhereToExtract.Symbols.ToListExcept( 0 );
            if ( !newDefinition.Any() )
            {
                newDefinition.Add( RuleSymbol.TerminalSymbol( TerminalSymbol.EmptySymbol() ) );
            }

            ruleToInline.Definitions[index] = new RuleDefinition( newDefinition );
        }
        
        bool hasDefinitionWithoutCommonHeading = definitionsWithCommonHeadings.Count != ruleToInline.Definitions.Count;
        if ( !hasDefinitionWithoutCommonHeading )
        {
            var definitionsToRemove = ruleUsers
                .Select( ruleUser => grammar.Rules[ruleUser.DefinitionOwner].Definitions[ruleUser.DefinitionIndex] )
                .ToList();
            
            foreach ( (RuleName DefinitionOwner, int DefinitionIndex, int RuleToReplaceIndex) ruleUser in ruleUsers )
            {
                GrammarRule ruleWhereToInline = grammar.Rules[ruleUser.DefinitionOwner];
                ruleWhereToInline.Definitions = ruleWhereToInline.Definitions
                    .Where( definition => !definitionsToRemove.Contains( definition ) )
                    .ToList();
            }
        }

        return hasChanges;
    }

    private List<(RuleName DefinitionOwner, int DefinitionIndex, int RuleToReplaceIndex)> FindRuleUsers(
        RuleName ruleToSearchName,
        CommonGrammar grammar )
    {
        var result = new List<(RuleName DefinitionOwner, int DefinitionIndex, int RuleToReplaceIndex)>();

        foreach ( GrammarRule rule in grammar.Rules.Values )
        {
            for ( var definitionIndex = 0; definitionIndex < rule.Definitions.Count; definitionIndex++ )
            {
                RuleDefinition definition = rule.Definitions[definitionIndex];
                for ( var symbolIndex = 0; symbolIndex < definition.Symbols.Count; symbolIndex++ )
                {
                    RuleSymbol symbol = definition.Symbols[symbolIndex];
                    if ( symbol.Type == RuleSymbolType.NonTerminalSymbol && 
                         symbol.RuleName! == ruleToSearchName )
                    {
                        result.Add( ( rule.Name, definitionIndex, symbolIndex ) );
                    }
                }
            }
        }

        return result;
    }

    private static LinkedList<RuleName> BuildRuleQueue(
        UnitableDefinitionsGroups unitableGroup,
        CommonGrammar grammar )
    {
        var result = new LinkedList<RuleName>();

        var rulesToCheckQueue = new Queue<RuleName>( unitableGroup.Definitions
            .Where( definition => definition.FirstSymbolType() == RuleSymbolType.NonTerminalSymbol )
            .Select( definition => definition.FirstSymbol().RuleName! )
            .ToHashSet() );

        while ( rulesToCheckQueue.Any() )
        {
            GrammarRule ruleToCheck = grammar.Rules[rulesToCheckQueue.Dequeue()];
            result.AddFirst( ruleToCheck.Name );

            HashSet<RuleName> startingNonTerminals = ruleToCheck.Definitions
                .Where( x => x.FirstSymbolType() == RuleSymbolType.NonTerminalSymbol )
                .Select( x => x.FirstSymbol().RuleName! )
                .ToHashSet();

            if ( startingNonTerminals.Any( rule => result.Contains( rule ) ) )
            {
                throw new ArgumentException( "Left factorization can not be performed at a grammar with left recursion" );
            }

            rulesToCheckQueue.EnqueueRange( startingNonTerminals );
        }

        return result;
    }

    private bool NeedToBeRemoved( RuleDefinition definition, List<RuleDefinition> definitionToRemove )
    {
        foreach ( RuleDefinition ruleDefinition in definitionToRemove )
        {
            if ( ruleDefinition.Equals( definition ) )
            {
                return true;
            }
        }

        return false;
    }

    private static Dictionary<RuleDefinition, GuidingSymbolsSet> BuildDefinitionsToHeadings( CommonGrammar grammar, GrammarRule rule )
    {
        return rule.Definitions.ToDictionary(
            x => x,
            x => grammar.GetFirstSet( rule.Name, x ) );
    }
}