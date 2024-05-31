using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using Grammars.Common.Grammars;
using Grammars.Common.Grammars.Extensions;
using Grammars.Common.Grammars.ValueObjects.GrammarRules;
using Grammars.Common.Grammars.ValueObjects.RuleDefinitions;
using Grammars.Common.Grammars.ValueObjects.RuleNames;
using Grammars.Common.Grammars.ValueObjects.Symbols;
using LinqExtensions;

namespace Grammars.Common.Convertors.Implementation.Factorization;

internal class GrammarRulesInliner
{
    public void InlineRules( CommonGrammar grammar )
    {
        var hasChanges = true;
        while ( hasChanges )
        {
            hasChanges = false;

            List<RuleName> grammarRules = grammar.Rules.Keys.ToList();
            for ( int i = 0; i < grammarRules.Count; i++ )
            {
                hasChanges |= InlineRule( grammarRules[i], grammar );
            }
        }
    }

    public bool InlineFirstSymbolsFromNonTerminals(
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

            var rulesToInlineQueue = BuildRuleQueue( unitableGroup, grammar );

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

    private bool InlineRule( RuleName ruleToInline, CommonGrammar grammar )
    {
        List<RuleDefinition> ruleToInlineDefinitions = grammar.Rules[ruleToInline].Definitions.ToList();
        
        bool hasReferenceToSelf = grammar.Rules[ruleToInline].Has( ruleToInline );
        bool canBeRemoved = !hasReferenceToSelf && grammar.StartRule != ruleToInline;
        if ( !canBeRemoved )
        {
            return false;
        }
        
        grammar.Rules.Remove( ruleToInline );
        
        bool hasChanges = false;
        foreach ( GrammarRule rule in grammar.Rules.Values )
        {
            var newDefinitions = new List<RuleDefinition>();
            var definitionsToRemove = new List<RuleDefinition>();
            
            foreach ( RuleDefinition definition in rule.Definitions )
            {
                for ( var index = 0; index < definition.Symbols.Count; index++ )
                {
                    RuleSymbol symbol = definition.Symbols[index];
                    if ( symbol.Type != RuleSymbolType.NonTerminalSymbol )
                    {
                        continue;
                    }

                    if ( symbol.RuleName != ruleToInline )
                    {
                        continue;
                    }

                    foreach ( RuleDefinition ruleToInlineDefinition in ruleToInlineDefinitions )
                    {
                        var newDefinition = definition.Symbols.ToList();
                        newDefinition.RemoveAt( index );
                        newDefinition.InsertRange( index, ruleToInlineDefinition.Symbols );
                        newDefinitions.Add( new RuleDefinition( newDefinition ) );
                    }
                    
                    definitionsToRemove.Add( definition );
                }
            }

            hasChanges = newDefinitions.Any();

            rule.Definitions = rule.Definitions.Where( x => !definitionsToRemove.Contains( x ) ).ToList();
            rule.Definitions.AddRange( newDefinitions );
        }
        
        return hasChanges;
    }

    private bool InlineFirstSymbolsPresentedInHashSet(
        RuleName ruleToInlineName,
        HashSet<RuleSymbol> symbolsToInline,
        CommonGrammar grammar )
    {
        var hasChanges = false;
        
        GrammarRule ruleToInline = grammar.Rules[ruleToInlineName];

        var ruleUsers = FindRuleUsers( ruleToInlineName, grammar );

        var definitionsWithCommonHeadings = ruleToInline.Definitions
            .Where( definition => grammar.GetFirstSet( ruleToInline.Name, definition ).HasIntersections( symbolsToInline ) )
            .ToList();

        bool hasDefinitionWithoutCommonHeading = definitionsWithCommonHeadings.Count != ruleToInline.Definitions.Count;

        var definitionsWhereToRemoveStartSymbols = new List<int>();
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
            
            definitionsWhereToRemoveStartSymbols.Add( index );
        }

        foreach ( int definitionsWhereToRemoveStartSymbol in definitionsWhereToRemoveStartSymbols )
        {
            RuleDefinition definition = ruleToInline.Definitions[definitionsWhereToRemoveStartSymbol];
            definition = RemoveStartSymbol( definition );

            if ( definition.Symbols.Count == 1 )
            {
                InlineRule( ruleToInlineName, grammar );
            }
        }

        if ( !hasDefinitionWithoutCommonHeading )
        {
            List<(RuleName RuleName, RuleDefinition Definition)> definitionsToRemove = ruleUsers
                .Select( ruleUser => (RuleName: ruleUser.DefinitionOwner, Definition: grammar.Rules[ruleUser.DefinitionOwner].Definitions[ruleUser.DefinitionIndex] ) )
                .ToList();

            foreach ( (RuleName RuleName, RuleDefinition Definition) ruleToDefinitionToRemove in definitionsToRemove )
            {
                GrammarRule rule = grammar.Rules[ruleToDefinitionToRemove.RuleName];
                rule.Definitions = rule.Definitions
                    .Where( definition => !definition.Equals( ruleToDefinitionToRemove.Definition ) )
                    .ToList();
            }
        }
        
        return hasChanges;
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
                    if ( symbol.Type == RuleSymbolType.NonTerminalSymbol && symbol.RuleName! == ruleToSearchName )
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
                continue;
            }

            rulesToCheckQueue.EnqueueRange( startingNonTerminals );
        }

        return result;
    }
}