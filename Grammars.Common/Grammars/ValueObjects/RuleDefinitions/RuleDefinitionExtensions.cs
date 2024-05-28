using Grammars.Common.Grammars.ValueObjects.Symbols;

namespace Grammars.Common.Grammars.ValueObjects.RuleDefinitions;

public static class RuleDefinitionExtensions
{
    public static bool Has( this RuleDefinition definition, RuleName ruleName ) =>
        definition.Symbols.Any( symbol => 
            symbol.Type == RuleSymbolType.NonTerminalSymbol && 
            symbol.RuleName == ruleName );

    public static bool Has( this RuleDefinition definition, RuleSymbol symbolToFind ) => 
        definition.Symbols.Any( symbol => symbol == symbolToFind );

    public static bool Has( this RuleDefinition definition, TerminalSymbolType terminalSymbolType ) => 
        definition.Symbols.Any( symbol =>
            symbol.Type == RuleSymbolType.TerminalSymbol && 
            symbol.Symbol!.Type == terminalSymbolType );

}