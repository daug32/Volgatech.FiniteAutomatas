using Grammars.Common.Extensions.Grammar.ValuesObjects;
using Grammars.Common.ValueObjects;
using Grammars.Common.ValueObjects.Symbols;
using LinqExtensions;

namespace Grammars.Common.Extensions.Grammar;

public static class GrammarFollowSetExtensions
{
    public static GuidingSymbolsSet GetFollowSet( this CommonGrammar grammar, RuleName ruleName )
    {
        var result = new HashSet<RuleSymbol>();

        if ( grammar.StartRule == ruleName )
        {
            var startRuleEndings = grammar.Rules[grammar.StartRule]
                .Definitions
                .Select( definition => definition.Symbols.Last() );

            result.AddRange( startRuleEndings );
        }

        foreach ( GrammarRule rule in grammar.Rules.Values )
        {
            bool hasEpsilonProductions = rule.Has( TerminalSymbolType.EmptySymbol );
            
            foreach ( RuleDefinition definition in rule.Definitions )
            {
                for ( var index = 0; index < definition.Symbols.Count; index++ )
                {
                    RuleSymbol symbol = definition.Symbols[index];
                    if ( symbol.Type != RuleSymbolType.NonTerminalSymbol )
                    {
                        continue;
                    }

                    if ( symbol.RuleName != ruleName )
                    {
                        continue;
                    }

                    if ( index + 1 == definition.Symbols.Count )
                    {
                        result.AddRange( grammar.GetFollowSet( rule.Name ).GuidingSymbols );
                        continue;
                    }

                    result.Add( definition.Symbols[index + 1] );
                    break;
                }
            }
        }

        return new GuidingSymbolsSet( ruleName, result );
    }
}