using FiniteAutomatas.Grammar.LeftToRightOne.Console.Models;

namespace FiniteAutomatas.Grammar.LeftToRightOne.Console.Parsers.Implementation;

internal class GrammarRuleLineParseResult
{
    public GrammarRuleName? RuleName;
    public List<GrammarRuleValue>? Rules = null;

    public bool HasData => RuleName is not null || Rules != null;
}