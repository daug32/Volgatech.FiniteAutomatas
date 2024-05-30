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
    private int _number = 0;
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

            if ( !grammar.Rules.ContainsKey( ruleToProcessName ) )
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
                var newRule = new GrammarRule( new RuleName( $"{_number++}" ), new List<RuleDefinition>() );

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
                        newRule.Definitions.Add( oldDefinitionToMigrate.Copy() );
                        continue;
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
        bool hasDefinitionWithoutCommonHeading = definitionsWithCommonHeadings.Count != ruleToInline.Definitions.Count;

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
                    
            ruleToInline.Definitions[index] = RemoveStartSymbol( ruleDefinitionWhereToExtract );
        }

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

        OptimizeRule( ruleToInline.Name, grammar );

        return hasChanges;
    }

    private void OptimizeRule( RuleName name, CommonGrammar grammar )
    {
        foreach ( RuleDefinition definition in grammar.Rules[name].Definitions )
        {
            foreach ( RuleSymbol symbol in definition.Symbols )
            {
                if ( symbol.Type != RuleSymbolType.TerminalSymbol )
                {
                    return;
                }
            }
        }

        bool hasNonEpsilonProductions = grammar.Rules[name].Definitions.Any( def => def.FirstSymbol().Symbol!.Type != TerminalSymbolType.EmptySymbol ); 

        foreach ( GrammarRule rule in grammar.Rules.Values )
        {
            if ( rule.Name == name )
            {
                continue;
            }

            var processedDefinitions = new List<RuleDefinition>();
            for ( var definitionIndex = 0; definitionIndex < rule.Definitions.Count; definitionIndex++ )
            {
                RuleDefinition definition = rule.Definitions[definitionIndex];

                for ( var symbolIndex = 0; symbolIndex < definition.Symbols.Count; symbolIndex++ )
                {
                    RuleSymbol symbol = definition.Symbols[symbolIndex];
                    if ( symbol.Type != RuleSymbolType.NonTerminalSymbol || symbol.RuleName! != name )
                    {
                        continue;
                    }

                    List<RuleSymbol> newDefinition = definition.Symbols.ToListExcept( symbolIndex );
                    if ( !newDefinition.Any() )
                    {
                        newDefinition.Add( RuleSymbol.TerminalSymbol( TerminalSymbol.EmptySymbol() ) );
                    }

                    rule.Definitions.Add( new RuleDefinition( newDefinition ) );
                    processedDefinitions.Add( definition );
                }
            }

            if ( !hasNonEpsilonProductions )
            {
                rule.Definitions = rule.Definitions.Where( def => !processedDefinitions.Contains( def ) ).ToList();

                grammar.Rules.Remove( name );
            }
        }
        
    }

    private static RuleDefinition RemoveStartSymbol( RuleDefinition definitionWhereToRemove )
    {
        var newDefinitionSymbols = definitionWhereToRemove.Symbols.ToListExcept( 0 );
        if ( !newDefinitionSymbols.Any() )
        {
            newDefinitionSymbols.Add( RuleSymbol.TerminalSymbol( TerminalSymbol.EmptySymbol() ) );
        }

        return new RuleDefinition( newDefinitionSymbols );
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
}