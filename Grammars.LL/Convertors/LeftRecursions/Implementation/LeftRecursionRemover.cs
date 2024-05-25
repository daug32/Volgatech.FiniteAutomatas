using Grammars.LL.Models;
using Grammars.LL.Models.ValueObjects;
using Grammars.LL.Models.ValueObjects.Symbols;
using LinqExtensions;

namespace Grammars.LL.Convertors.LeftRecursions.Implementation;

internal class LeftRecursionRemover
{
    public LlOneGrammar RemoveLeftRecursion( LlOneGrammar grammar )
    {
        List<RuleName> allRules = grammar.Rules.Keys.ToList();
        MutableGrammar mutableGrammar = new MutableGrammar( grammar );

        int mainIterator = 0;
        while ( true )
        {
            ReplaceLeftRecursionByNewRule( allRules[mainIterator], mutableGrammar );
            
            if ( mainIterator == allRules.Count - 1 )
            {
                break;
            }

            mainIterator++;

            for ( int secondIterator = 0; secondIterator < mainIterator; secondIterator++ )
            {
                GrammarRule mainRule = mutableGrammar.Rules[allRules[mainIterator]];
                GrammarRule secondRule = mutableGrammar.Rules[allRules[secondIterator]];

                mutableGrammar.Replace( new GrammarRule( mainRule.Name, ReplaceHeadingRules( mainRule, secondRule ) ) );
            }
        }

        return mutableGrammar.ToLlOneGrammar();
    }

    private static List<RuleDefinition> ReplaceHeadingRules( GrammarRule mainRule, GrammarRule secondRule )
    {
        var newDefinitions = new List<RuleDefinition>();
        foreach ( RuleDefinition mainRuleDefinition in mainRule.Definitions )
        {
            RuleSymbol firstSymbol = mainRuleDefinition.Symbols.First();

            if ( firstSymbol.Type != RuleSymbolType.NonTerminalSymbol || 
                 firstSymbol.RuleName != secondRule.Name )
            {
                newDefinitions.Add( mainRuleDefinition );
                continue;
            }

            // Main rule definition symbols except for heading second rule
            List<RuleSymbol> tail = mainRuleDefinition.Symbols
                .ToList()
                .WithoutFirst();

            foreach ( RuleDefinition secondRuleDefinition in secondRule.Definitions )
            {
                var newDefinitionSymbols = secondRuleDefinition.Symbols
                    .Select( symbol => symbol )
                    .ToList()
                    .With( tail );

                newDefinitions.Add( new RuleDefinition( newDefinitionSymbols ) );
            }
        }

        return newDefinitions;
    }

    private void ReplaceLeftRecursionByNewRule( RuleName targetRuleName, MutableGrammar grammar )
    {
        (List<RuleDefinition> WithLeftRecursion, List<RuleDefinition> WithoutLeftRecursion) 
            groupedDefinitions = GroupRuleDefinitionsByLeftRecursionUsage( targetRuleName, grammar );

        if ( !groupedDefinitions.WithLeftRecursion.Any() )
        {
            return;
        }

        var copiedRule = new GrammarRule(
            targetRuleName,
            new List<RuleDefinition>() );
        var newRule = new GrammarRule( 
            new RuleName( targetRuleName.Value + '`' ), 
            new List<RuleDefinition>() );

        foreach ( RuleDefinition definition in groupedDefinitions.WithoutLeftRecursion )
        {
            copiedRule.AddDefinition( new RuleDefinition( definition.Symbols.ToList() ) );    
            
            copiedRule.AddDefinition( new RuleDefinition( definition.Symbols
                .ToList()
                .With( RuleSymbol.NonTerminalSymbol( newRule.Name ) ) ) );    
        }

        foreach ( RuleDefinition definition in groupedDefinitions.WithLeftRecursion )
        {
            newRule.AddDefinition( new RuleDefinition( definition.Symbols
                .ToList()
                .WithoutFirst() ) );
            
            newRule.AddDefinition( new RuleDefinition( definition.Symbols
                .ToList()
                .WithoutFirst()
                .With( RuleSymbol.NonTerminalSymbol( newRule.Name ) ) ) );
        }
        
        grammar.Replace( newRule );
        grammar.Replace( copiedRule );
    }

    internal 
        (List<RuleDefinition> WithLeftRecursion, List<RuleDefinition> WithoutLeftRecursion) 
        GroupRuleDefinitionsByLeftRecursionUsage( RuleName targetRuleName, MutableGrammar grammar )
    {
        var withLR = new List<RuleDefinition>();
        var withoutLR = new List<RuleDefinition>();
        
        GrammarRule targetRule = grammar.Rules[targetRuleName];

        foreach ( RuleDefinition targetRuleDefinition in targetRule.Definitions )
        {
            RuleSymbol targetRuleDefinitionStartSymbol = targetRuleDefinition.Symbols.First();
            if ( targetRuleDefinitionStartSymbol.Type == RuleSymbolType.TerminalSymbol )
            {
                withoutLR.Add( targetRuleDefinition );
                continue;
            }

            if ( targetRuleDefinitionStartSymbol.RuleName == targetRule.Name )
            {
                withLR.Add( targetRuleDefinition );
                continue;
            }
            
            withoutLR.Add( targetRuleDefinition );
        }

        return (withLR, withoutLR);
    }
}