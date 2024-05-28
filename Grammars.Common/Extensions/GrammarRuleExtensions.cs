using Grammars.Common.ValueObjects;
using Grammars.Common.ValueObjects.Symbols;

namespace Grammars.Common.Extensions;

public static class GrammarRuleExtensions
{
    public static bool Has(
        this GrammarRule rule,
        TerminalSymbolType terminalSymbolType ) => rule.Definitions.Any( definition => definition.Has( terminalSymbolType ) );
}