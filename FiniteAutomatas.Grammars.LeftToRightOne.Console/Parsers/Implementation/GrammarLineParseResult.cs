using FiniteAutomatas.Grammars.LeftRoRightOne.Models.ValueObjects;

namespace FiniteAutomatas.Grammars.LeftToRightOne.Console.Parsers.Implementation;

internal class GrammarLineParseResult
{
    public RuleName? RuleName;
    public List<RuleValue>? Rules = null;

    public bool HasData => RuleName is not null || Rules != null;
}