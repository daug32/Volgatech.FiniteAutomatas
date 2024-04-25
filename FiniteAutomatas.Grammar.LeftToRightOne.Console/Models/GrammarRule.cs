namespace FiniteAutomatas.Grammar.LeftToRightOne.Console.Models;

public class GrammarRule
{
    public readonly GrammarRuleName Name;
    public List<GrammarRuleValue> Values = new();

    public GrammarRule( GrammarRuleName name )
    {
        Name = name;
    }
}