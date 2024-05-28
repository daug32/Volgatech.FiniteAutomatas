using Grammars.Common.Grammars.ValueObjects;
using Grammars.Common.Grammars.ValueObjects.GrammarRules;
using Grammars.Common.Grammars.ValueObjects.RuleDefinitions;
using Grammars.Common.Grammars.ValueObjects.Symbols;
using LinqExtensions;

namespace Grammars.Common.Grammars.Extensions;

public static class GrammarFollowSetExtensions
{
    public static GuidingSymbolsSet GetFollowSet( this CommonGrammar grammar, RuleName ruleNameToGetFollowSet )
    {
        var result = grammar.Rules.Keys.ToDictionary( x => x, x => new HashSet<RuleSymbol>() );
        var relations = grammar.Rules.Keys.ToDictionary( x => x, x => (Follows: new HashSet<RuleName>(), Firsts: new HashSet<RuleName>()) );

        InitalizeResultAndRelationsTables( grammar, relations, result );
        
        bool hasChanges = true;
        while ( hasChanges )
        {
            hasChanges = false;

            foreach ( RuleName ruleName in relations.Keys )
            {
                (HashSet<RuleName> Follows, HashSet<RuleName> Firsts) ruleRelations = relations[ruleName];

                int countBeforeChanges = result[ruleName].Count;
                foreach ( RuleName toFirst in ruleRelations.Firsts )
                {
                    GuidingSymbolsSet guidingSymbols = grammar.GetFirstSet( toFirst );
                    if ( guidingSymbols.Has( TerminalSymbolType.EmptySymbol ) )
                    {
                        ruleRelations.Follows.Add( toFirst );
                    }
                    
                    result[ruleName].AddRange( guidingSymbols.GuidingSymbols );
                }

                foreach ( RuleName toFollow in ruleRelations.Follows )
                {
                    result[ruleName].AddRange( result[toFollow] );
                }
                
                int countAfterChanges = result[ruleName].Count;

                hasChanges |= countAfterChanges != countBeforeChanges;
            }
        }

        RemoveAllEpsilons( result );

        return new GuidingSymbolsSet( ruleNameToGetFollowSet, result[ruleNameToGetFollowSet] );
    }

    private static void RemoveAllEpsilons( Dictionary<RuleName, HashSet<RuleSymbol>> result )
    {
        foreach ( RuleName ruleName in result.Keys )
        {
            result[ruleName].RemoveWhere( x => x.Symbol!.Type == TerminalSymbolType.EmptySymbol );
        }
    }

    private static void InitalizeResultAndRelationsTables(
        CommonGrammar grammar,
        Dictionary<RuleName, (HashSet<RuleName> Follows, HashSet<RuleName> Firsts)> relations,
        Dictionary<RuleName, HashSet<RuleSymbol>> result )
    {
        foreach ( GrammarRule rule in grammar.Rules.Values )
        {
            foreach ( RuleDefinition definition in rule.Definitions )
            {
                for ( var index = 0; index < definition.Symbols.Count; index++ )
                {
                    RuleSymbol symbol = definition.Symbols[index];
                    if ( symbol.Type == RuleSymbolType.TerminalSymbol )
                    {
                        continue;
                    }
                    
                    (HashSet<RuleName> Follows, HashSet<RuleName> Firsts) symbolRelations = relations[symbol.RuleName!];

                    if ( index + 1 == definition.Symbols.Count )
                    {
                        symbolRelations.Follows.Add( rule.Name );
                        continue;
                    }

                    RuleSymbol nextSymbol = definition.Symbols[index + 1];
                    if ( nextSymbol.Type == RuleSymbolType.NonTerminalSymbol )
                    {
                        symbolRelations.Firsts.Add( nextSymbol.RuleName! );
                        continue;
                    }

                    result[symbol.RuleName!].Add( nextSymbol );
                }
            }
        }

        bool hasEndSymbols = grammar.Rules[grammar.StartRule].Definitions.Any( definition =>
        {
            RuleSymbol lastSymbol = definition.Symbols.Last();
            return 
                lastSymbol.Type == RuleSymbolType.TerminalSymbol && 
                lastSymbol.Symbol.Type == TerminalSymbolType.End;
        } );

        if ( hasEndSymbols )
        {
            result[grammar.StartRule].Add( RuleSymbol.TerminalSymbol( TerminalSymbol.End() ) );
        }
    }
}