namespace Grammars.Grammars.LeftRoRightOne.Models.ValueObjects;

public class GrammarRule
{
    public readonly RuleName Name;
    public readonly IReadOnlyList<RuleValue> Values;

    public GrammarRule( RuleName name, IEnumerable<RuleValue> values )
    {
        Name = name;
        Values = values.ToList();
    }
}