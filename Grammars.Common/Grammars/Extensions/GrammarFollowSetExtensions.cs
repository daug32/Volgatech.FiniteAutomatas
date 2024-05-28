using Grammars.Common.Grammars.ValueObjects;
using Grammars.Common.Grammars.ValueObjects.GrammarRules;
using Grammars.Common.Grammars.ValueObjects.RuleDefinitions;
using Grammars.Common.Grammars.ValueObjects.Symbols;
using LinqExtensions;

namespace Grammars.Common.Grammars.Extensions;

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
                    
                    RuleSymbol nextSymbol = definition.Symbols[index + 1];
                    if ( nextSymbol.Type == RuleSymbolType.NonTerminalSymbol )
                    {
                        GuidingSymbolsSet nextRuleGuidingSymbolsSet = grammar.GetFirstSet( nextSymbol.RuleName! );
                        
                        bool hasEpsilonProduction = nextRuleGuidingSymbolsSet.Has( TerminalSymbolType.EmptySymbol );
                        if ( hasEpsilonProduction )
                        {
                            result.AddRange( nextRuleGuidingSymbolsSet.GuidingSymbols.Where( x => x.Symbol!.Type != TerminalSymbolType.EmptySymbol ) );
                            result.AddRange( grammar.GetFollowSet( nextSymbol.RuleName! ).GuidingSymbols );
                            break;
                        }

                        result.AddRange( nextRuleGuidingSymbolsSet.GuidingSymbols );
                        break;
                    }
                    
                    result.Add( nextSymbol );
                    break;
                }
            }
        }

        return new GuidingSymbolsSet( ruleName, result );
    }
}