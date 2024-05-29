using System.Diagnostics;
using Grammars.Common.Grammars.ValueObjects;
using Grammars.Common.Grammars.ValueObjects.GrammarRules;
using Grammars.Common.Grammars.ValueObjects.RuleDefinitions;
using Grammars.Common.Grammars.ValueObjects.Symbols;
using LinqExtensions;

namespace Grammars.Common.Grammars.Extensions;

public static class GrammarFirstSetExtensions
{
    public static GuidingSymbolsSet GetFirstSet( this CommonGrammar grammar, RuleName ruleName )
    {
        return grammar.GetFirstSet( ruleName, grammar.Rules[ruleName].Definitions )[ruleName];
    }

    public static GuidingSymbolsSet GetFirstSet( this CommonGrammar grammar, RuleName ruleName, RuleDefinition definition )
    {
        return grammar.GetFirstSet( ruleName, new[] { definition } )[ruleName];
    }

    private static Dictionary<RuleName, GuidingSymbolsSet> GetFirstSet( this CommonGrammar grammar, RuleName ruleNameToGetFirstSet, IEnumerable<RuleDefinition> definitions )
    {
        List<ConcreteDefinition> concreteDefinitions = BuildConcreteDefinitions( grammar, ruleNameToGetFirstSet, definitions );
        
        var result = grammar.Rules.Keys.ToDictionary( x => x, x => new HashSet<RuleSymbol>() );
        var relations = grammar.Rules.Keys.ToDictionary( x => x, x => new List<(ConcreteDefinition Definition, int Index)>() );

        InitializeResultAndRelationsTables( concreteDefinitions, relations, result );
        
        bool hasChanges = true;
        while ( hasChanges )
        {
            hasChanges = false;

            foreach ( RuleName ruleName in relations.Keys )
            {
                List<(ConcreteDefinition Definition, int Index)> ruleRelations = relations[ruleName];

                int countBeforeChanges = result[ruleName].Count;
                for ( var index = 0; index < ruleRelations.Count; index++ )
                {
                    (ConcreteDefinition Definition, int Index) toFirst = ruleRelations[index];
                    RuleDefinition toFirstDefinition = toFirst.Definition.Definition;
                    RuleName toFirstRuleName = toFirstDefinition.Symbols[toFirst.Index].RuleName ?? throw new UnreachableException();

                    HashSet<RuleSymbol> guidingSymbols = result[toFirstRuleName];

                    result[ruleName].AddRange( result[toFirstRuleName] );
                    
                    if ( guidingSymbols.All( x => x.Symbol!.Type != TerminalSymbolType.EmptySymbol ) )
                    {
                        continue;
                    }

                    if ( !ruleRelations.Any( x => x.Index == toFirst.Index && Equals( x.Definition, toFirst.Definition ) ) )
                    {
                        ruleRelations.Add( toFirst );
                    }

                    for ( int i = toFirst.Index + 1; i < toFirstDefinition.Symbols.Count; i++ )
                    {
                        RuleSymbol symbol = toFirstDefinition.Symbols[i];
                        if ( symbol.Type == RuleSymbolType.TerminalSymbol )
                        {
                            result[ruleName].Add( symbol );

                            if ( symbol.Symbol!.Type != TerminalSymbolType.EmptySymbol )
                            {
                                break;
                            }

                            continue;
                        }

                        relations[ruleName].Add( ( toFirst.Definition, i ) );
                        result[ruleName].AddRange( result[symbol.RuleName!] );
                    }

                    result[ruleName].AddRange( guidingSymbols );
                }

                int countAfterChanges = result[ruleName].Count;

                hasChanges |= countAfterChanges != countBeforeChanges;
            }
        }

        return result.ToDictionary(
            pair => pair.Key,
            pair => new GuidingSymbolsSet( pair.Key, pair.Value ) );
    }

    private static void InitializeResultAndRelationsTables( 
        List<ConcreteDefinition> definitions,
        Dictionary<RuleName, List<(ConcreteDefinition Definition, int Index)>> relations,
        Dictionary<RuleName, HashSet<RuleSymbol>> result )
    {
        foreach ( ConcreteDefinition definition in definitions )
        {
            RuleSymbol firstSymbol = definition.Definition.FirstSymbol();
            if ( firstSymbol.Type == RuleSymbolType.TerminalSymbol )
            {
                result[definition.RuleName].Add( firstSymbol );
            }
            else
            {
                relations[definition.RuleName].Add( (definition, 0) );
            }
        }
    }

    private static List<ConcreteDefinition> BuildConcreteDefinitions( CommonGrammar grammar, RuleName ruleName, IEnumerable<RuleDefinition> definitionsToUse )
    {
        var result = new List<ConcreteDefinition>();
        
        foreach ( GrammarRule rule in grammar.Rules.Values )
        {
            var definitionsToEnumerate = rule.Name == ruleName
                ? definitionsToUse
                : rule.Definitions;

            foreach ( RuleDefinition definition in definitionsToEnumerate )
            {
                var newDefinition = new List<RuleSymbol>();
                foreach ( RuleSymbol symbol in definition.Symbols )
                {
                    if ( symbol.Type == RuleSymbolType.TerminalSymbol )
                    {
                        newDefinition.Add( symbol );

                        if ( symbol.Symbol.Type == TerminalSymbolType.EmptySymbol )
                        {
                            continue;
                        }

                        break;
                    }
                    
                    newDefinition.Add( symbol );
                }
                
                result.Add( new ConcreteDefinition( rule.Name, new RuleDefinition( newDefinition ) ) );
            }
        }

        return result;
    }

    private static Queue<ConcreteDefinition> BuildQueue(
        RuleName ruleName,
        List<RuleDefinition> targetDefinitions,
        CommonGrammar grammar )
    {
        var result = new List<ConcreteDefinition>();

        var processedNonTerms = new HashSet<RuleName>();
        var nonTermsToProcess = new Queue<RuleName>( ExtractAchievableNonTerminals( targetDefinitions ) );

        while ( nonTermsToProcess.Any() )
        {
            RuleName rule = nonTermsToProcess.Dequeue();
            if ( processedNonTerms.Contains( rule ) )
            {
                continue;
            }

            List<RuleDefinition> ruleDefinitions = grammar.Rules[rule].Definitions;
            IEnumerable<RuleName> nonTerms =
                ruleDefinitions.SelectMany( x => x.Symbols.Where( x => x.Type == RuleSymbolType.NonTerminalSymbol ).Select( x => x.RuleName! ) );

            foreach ( RuleName nonTerm in nonTerms )
            {
                nonTermsToProcess.Enqueue( nonTerm );
            }

            result.AddRange( ruleDefinitions.Select( x => new ConcreteDefinition( rule, x ) ) );

            processedNonTerms.Add( rule );
        }

        if ( !processedNonTerms.Contains( ruleName ) )
        {
            result.AddRange( targetDefinitions.Select( x => new ConcreteDefinition( ruleName, x ) ) );
        }

        return new Queue<ConcreteDefinition>( result.OrderBy( definition =>
        {
            RuleSymbol first = definition.Definition.FirstSymbol();

            if ( first.Type == RuleSymbolType.NonTerminalSymbol )
            {
                return 2;
            }

            if ( first.Symbol!.Type == TerminalSymbolType.EmptySymbol )
            {
                return 1;
            }

            return 0;
        } ) );
    }

    private static IEnumerable<RuleName> ExtractAchievableNonTerminals( List<RuleDefinition> targetDefinitions )
    {
        var result = new HashSet<RuleName>();
        
        foreach ( RuleDefinition definition in targetDefinitions )
        {
            foreach ( RuleSymbol symbol in definition.Symbols )
            {
                if ( symbol.Type == RuleSymbolType.TerminalSymbol )
                {
                    break;
                }

                result.Add( symbol.RuleName! );
            }
        }

        return result;
    }
}