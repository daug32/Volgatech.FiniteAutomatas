namespace Grammars.Grammars.LeftRoRightOne.Models.ValueObjects;

public class GrammarRule
{
    public readonly RuleName Name;
    public List<RuleValue> Values = new();

    public GrammarRule( RuleName name )
    {
        Name = name;
    }
}