using Grammars.Common.ValueObjects;

namespace Grammars.Console.Parsers.Implementation.Models;

public class GrammarRuleParseResult
{
    public readonly RuleName RuleName;
    public List<RuleDefinition> RuleDefinitions = new();

    public GrammarRuleParseResult( RuleName ruleName )
    {
        RuleName = ruleName;
    }
}