using Grammars.Common.Grammars.ValueObjects;
using Grammars.Common.Grammars.ValueObjects.GrammarRules;
using Grammars.Common.Grammars.ValueObjects.RuleDefinitions;
using Grammars.Common.Grammars.ValueObjects.RuleNames;
using Grammars.Common.Grammars.ValueObjects.Symbols;

namespace Grammars.Common.Grammars;

public class CommonGrammar
{
    public RuleName StartRule { get; }
    public IDictionary<RuleName, GrammarRule> Rules { get; }

    public CommonGrammar( RuleName startRule, IEnumerable<GrammarRule> rules )
    {
        StartRule = startRule;
        Rules = rules.ToDictionary(
            x => x.Name,
            x => x );

        Validate();
    }

    public CommonGrammar Copy() => new CommonGrammar( StartRule, Rules.Values.Select( grammarRule => grammarRule.Copy() ) );

    public void Validate()
    {
        if ( !Rules.ContainsKey( StartRule ) )
        {
            throw new ArgumentException( $"StartRule was not found. StartRuleName: {StartRule}" );
        }

        foreach ( GrammarRule rule in Rules.Values )
        {
            foreach ( RuleDefinition definition in rule.Definitions )
            {
                foreach ( RuleSymbol symbol in definition.Symbols )
                {
                    if ( symbol.Type != RuleSymbolType.NonTerminalSymbol )
                    {
                        continue;
                    }

                    if ( !Rules.ContainsKey( symbol.RuleName! ) )
                    {
                        throw new ArgumentException( $"Grammar doesn't have a definition for rule. RuleName: {symbol}" );
                    }
                }
            }
        }
    }
}