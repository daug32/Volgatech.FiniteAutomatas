using Grammars.Grammars.LeftRoRightOne.Models.ValueObjects;

namespace Grammars.LL.Console.Parsers.Implementation.Models;

public class GrammarRuleParseResult
{
    public RuleName RuleName;
    public List<RuleValue> Values = new();

    public GrammarRuleParseResult( RuleName ruleName )
    {
        RuleName = ruleName;
    }
}