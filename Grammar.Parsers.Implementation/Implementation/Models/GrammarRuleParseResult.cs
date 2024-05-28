using Grammars.Common.ValueObjects;

namespace Grammar.Parsers.Implementation.Implementation.Models;

public class GrammarRuleParseResult
{
    public readonly RuleName RuleName;
    public List<RuleDefinition> RuleDefinitions = new();

    public GrammarRuleParseResult( RuleName ruleName )
    {
        RuleName = ruleName;
    }
}