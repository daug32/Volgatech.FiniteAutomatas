using System.Diagnostics;
using Grammars.Common.Grammars.ValueObjects;
using Grammars.Common.Grammars.ValueObjects.GrammarRules;
using Grammars.Common.Grammars.ValueObjects.RuleDefinitions;
using Grammars.Common.Grammars.ValueObjects.RuleNames;
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
        return grammar.GetFirstSet( ruleName, new List<RuleDefinition> { definition } )[ruleName];
    }

    private static Dictionary<RuleName, GuidingSymbolsSet> GetFirstSet( this CommonGrammar grammar, RuleName ruleNameToGetFirstSet, List<RuleDefinition> definitions )
    {
        List<ConcreteDefinition> concreteDefinitions = BuildConcreteDefinitions( grammar, ruleNameToGetFirstSet, definitions );
        
        var result = grammar.Rules.Keys.ToDictionary( x => x, _ => new HashSet<RuleSymbol>() );
        var relations = grammar.Rules.Keys.ToDictionary( x => x, _ => new List<(ConcreteDefinition Definition, int Index)>() );

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

                    bool hasEpsilon = guidingSymbols.All( x => x.Symbol!.Type != TerminalSymbolType.EmptySymbol );
                    if ( hasEpsilon )
                    {
                        continue;
                    }

                    bool alreadyHasRelation = !ruleRelations.Any( x => x.Index == toFirst.Index && Equals( x.Definition, toFirst.Definition ) );
                    if ( alreadyHasRelation )
                    {
                        ruleRelations.Add( toFirst );
                    }

                    // Add a new relation
                    for ( int i = toFirst.Index + 1; i < toFirstDefinition.Symbols.Count; i++ )
                    {
                        RuleSymbol symbol = toFirstDefinition.Symbols[i];
                        if ( symbol.Type == RuleSymbolType.TerminalSymbol )
                        {
                            if ( symbol.Symbol!.Type != TerminalSymbolType.EmptySymbol )
                            {
                                result[ruleName].Add( symbol );
                                break;
                            }

                            continue;
                        }

                        relations[ruleName].Add( ( toFirst.Definition, i ) );
                        result[ruleName].AddRange( result[symbol.RuleName!].Where( x => x.Symbol!.Type != TerminalSymbolType.EmptySymbol ) );
                    }
                }

                int countAfterChanges = result[ruleName].Count;

                hasChanges |= countAfterChanges != countBeforeChanges;
            }
        }

        RemoveEpsilonsIfNeeded( result, grammar );

        return result.ToDictionary(
            pair => pair.Key,
            pair => new GuidingSymbolsSet( pair.Key, pair.Value ) );
    }

    private static void RemoveEpsilonsIfNeeded( Dictionary<RuleName, HashSet<RuleSymbol>> result, CommonGrammar grammar )
    {
        foreach ( var rule in grammar.Rules.Values )
        {
            bool allDefinitionsEndsWithNotEmptyTerminal = rule.Definitions.All( d => 
                d.Symbols.Any( s => 
                    s.Type == RuleSymbolType.TerminalSymbol && s.Symbol!.Type != TerminalSymbolType.EmptySymbol ) );

            if ( allDefinitionsEndsWithNotEmptyTerminal )
            {
                result[rule.Name].RemoveWhere( x => x.Type == RuleSymbolType.TerminalSymbol && x.Symbol!.Type == TerminalSymbolType.EmptySymbol );
            }
        }
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

    private static List<ConcreteDefinition> BuildConcreteDefinitions( CommonGrammar grammar, RuleName ruleName, List<RuleDefinition> definitionsToUse )
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

                        if ( symbol.Symbol!.Type == TerminalSymbolType.EmptySymbol )
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
}