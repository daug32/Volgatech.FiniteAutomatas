using FiniteAutomatas.Grammars.LeftRoRightOne.Models.ValueObjects;

namespace FiniteAutomatas.Grammars.LeftRoRightOne.Models;

public class Grammar
{
    public IReadOnlyDictionary<RuleName, GrammarRule> Rules { get; }

    public Grammar( IEnumerable<GrammarRule> rules )
    {
        Rules = rules.ToDictionary(
            x => x.Name,
            x => x );

        foreach ( GrammarRule rule in Rules.Values )
        foreach ( RuleValue ruleValue in rule.Values )
        foreach ( RuleValueItem valueItem in ruleValue.Items )
        {
            if ( valueItem.Type != RuleValueItemType.NonTerminalSymbol )
            {
                continue;
            }

            if ( !Rules.ContainsKey( valueItem.RuleName! ) )
            {
                throw new ArgumentException(
                    $"There is a mention about a rule that is not presented in the given values. RuleName: {valueItem.RuleName}" );
            }
        }
    }
}