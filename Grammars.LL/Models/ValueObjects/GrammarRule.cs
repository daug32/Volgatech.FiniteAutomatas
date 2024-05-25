namespace Grammars.Grammars.LeftRoRightOne.Models.ValueObjects;

public class GrammarRule
{
    public readonly RuleName Name;
    public readonly IReadOnlyList<RuleDefinition> Definitions;

    public GrammarRule( RuleName name, IEnumerable<RuleDefinition> values )
    {
        Name = name;
        Definitions = values.ToList();
    }
}