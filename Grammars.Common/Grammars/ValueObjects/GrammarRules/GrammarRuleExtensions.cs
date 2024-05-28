using Grammars.Common.Grammars.ValueObjects.RuleDefinitions;
using Grammars.Common.Grammars.ValueObjects.Symbols;

namespace Grammars.Common.Grammars.ValueObjects.GrammarRules;

public static class GrammarRuleExtensions
{
    public static bool Has(
        this GrammarRule rule,
        TerminalSymbolType terminalSymbolType ) => rule.Definitions.Any( definition => definition.Has( terminalSymbolType ) );
}