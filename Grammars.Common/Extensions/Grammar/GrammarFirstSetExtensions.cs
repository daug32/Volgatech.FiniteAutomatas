using Grammars.Common.Extensions.Grammar.ValuesObjects;
using Grammars.Common.ValueObjects;
using Grammars.Common.ValueObjects.Symbols;
using LinqExtensions;

namespace Grammars.Common.Extensions.Grammar;

public static class GrammarFirstSetExtensions
{
    public static GuidingSymbolsSet GetFirstSet( this CommonGrammar grammar, RuleName ruleName )
    {
        return grammar.GetFirstSet( ruleName, grammar.Rules[ruleName].Definitions );
    }

    public static GuidingSymbolsSet GetFirstSet( this CommonGrammar grammar, RuleName ruleName, RuleDefinition definition )
    {
        return grammar.GetFirstSet( ruleName, new[] { definition } );
    }

    private static GuidingSymbolsSet GetFirstSet( this CommonGrammar grammar, RuleName ruleName, IEnumerable<RuleDefinition> definitions )
    {
        var result = new HashSet<RuleSymbol>();
        
        foreach ( RuleDefinition ruleDefinition in definitions )
        {
            for ( int index = 0; index < ruleDefinition.Symbols.Count; index++ )
            {
                RuleSymbol symbol = ruleDefinition.Symbols[index];
                if ( symbol.Type == RuleSymbolType.TerminalSymbol )
                {
                    result.Add( symbol );
                    break;
                }

                // If non terminal, just recursively get headings from it
                GuidingSymbolsSet innerRuleHeadings = grammar.GetFirstSet( symbol.RuleName! );
                innerRuleHeadings.GuidingSymbols
                    .Where( x => x.Symbol!.Type != TerminalSymbolType.EmptySymbol )
                    .ForEach( innerRuleHeading => result.Add( innerRuleHeading ) );
                    
                // If there is an epsilon, we need to get next symbol
                if ( innerRuleHeadings.Has( TerminalSymbolType.EmptySymbol ) )
                {
                    continue;
                }

                break;
            }
        }

        return new GuidingSymbolsSet( ruleName, result );
    }
}