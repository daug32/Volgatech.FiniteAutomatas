using Grammars.LL.Models.ValueObjects;

namespace Grammars.LL.Console.Parsers.Implementation.Models;

public class GrammarRuleParseResult
{
    public readonly RuleName RuleName;
    public List<RuleDefinition> RuleDefinitions = new();

    public GrammarRuleParseResult( RuleName ruleName )
    {
        RuleName = ruleName;
    }
}