using Grammars.Common.Grammars;
using Grammars.Common.Grammars.ValueObjects.GrammarRules;
using Grammars.Common.Grammars.ValueObjects.RuleDefinitions;
using Grammars.Common.Grammars.ValueObjects.Symbols;

namespace Grammars.Common.Convertors.Convertors;

public class RemoveWhitespacesConvertor : IGrammarConvertor
{
    public CommonGrammar Convert( CommonGrammar grammar )
    {
        var rules = new List<GrammarRule>();
        foreach ( GrammarRule rule in grammar.Rules.Values )
        {
            var definitions = new List<RuleDefinition>();
            foreach ( RuleDefinition definition in rule.Definitions )
            {
                var symbols = new List<RuleSymbol>();
                foreach ( RuleSymbol symbol in definition.Symbols )
                {
                    if ( symbol.Type == RuleSymbolType.TerminalSymbol )
                    {
                        if ( symbol.Symbol!.Type == TerminalSymbolType.WhiteSpace )
                        {
                            continue;
                        }

                        symbols.Add( symbol );
                        continue;
                    }

                    symbols.Add( symbol );
                }

                if ( symbols.Any() )
                {
                    definitions.Add( new RuleDefinition( symbols ) );
                }
            }

            rules.Add( new GrammarRule( rule.Name, definitions ) );
        }

        return new CommonGrammar( grammar.StartRule, rules );
    }
}