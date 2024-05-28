using Grammars.Common;
using Grammars.Common.Convertings.Convertors;
using Grammars.Common.ValueObjects;
using Grammars.Common.ValueObjects.Symbols;

namespace Grammars.Console;

public class RemoveWhitespacesConvertor : IGrammarConvertor
{
    public CommonGrammar Convert( CommonGrammar grammar )
    {
        return new CommonGrammar(
            grammar.StartRule,
            grammar.Rules.Values.Select( rule => new GrammarRule(
                rule.Name,
                rule.Definitions.Select( definition => new RuleDefinition( definition.Symbols.Where( symbol =>
                    symbol.Type == RuleSymbolType.NonTerminalSymbol || 
                    symbol.Type == RuleSymbolType.TerminalSymbol && 
                    symbol.Symbol.Type != TerminalSymbolType.WhiteSpace ) ) ) ) ) );
    }
}