using Grammars.Common.Grammars;
using Grammars.Common.Grammars.ValueObjects.GrammarRules;
using Grammars.Common.Grammars.ValueObjects.RuleDefinitions;
using Grammars.Common.Grammars.ValueObjects.RuleNames;
using Grammars.Common.Grammars.ValueObjects.Symbols;

namespace Grammars.Common.Convertors.Convertors.Renaming.Implementation;

internal class RenameRuleNamesHandler
{
    private readonly RenameRuleNamesOptions _options;

    public RenameRuleNamesHandler( RenameRuleNamesOptions? options = null )
    {
        _options = options ?? new RenameRuleNamesOptions();
    }
    
    public CommonGrammar StandardizeNaming( CommonGrammar grammar )
    {
        var oldNameToNewName = new Dictionary<RuleName, RuleName>();

        var index = 0;
        foreach ( RuleName ruleName in grammar.Rules.Keys )
        {
            oldNameToNewName[ruleName] =
                _options.RenameOnlyUnreadableRules && Int64.TryParse( ruleName.Value, out _ )
                    ? new RuleName( ( index++ ).ToString() )
                    : ruleName;
        }

        var newRules = new List<GrammarRule>();
        foreach ( RuleName ruleName in grammar.Rules.Keys )
        {
            GrammarRule rule = grammar.Rules[ruleName];

            var newDefinitions = new List<RuleDefinition>();
            foreach ( RuleDefinition definition in rule.Definitions )
            {
                var symbols = new List<RuleSymbol>();
                foreach ( RuleSymbol symbol in definition.Symbols )
                {
                    symbols.Add( symbol.Type == RuleSymbolType.NonTerminalSymbol
                        ? RuleSymbol.NonTerminalSymbol( oldNameToNewName[symbol.RuleName!] )
                        : symbol );
                }

                newDefinitions.Add( new RuleDefinition( symbols ) );
            }

            newRules.Add( new GrammarRule( oldNameToNewName[ruleName], newDefinitions ) );
        }

        return new CommonGrammar( oldNameToNewName[grammar.StartRule], newRules );
    }
}