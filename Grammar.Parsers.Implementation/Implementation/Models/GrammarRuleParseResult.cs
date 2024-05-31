using Grammars.Common.Grammars.ValueObjects;
using Grammars.Common.Grammars.ValueObjects.GrammarRules;
using Grammars.Common.Grammars.ValueObjects.RuleDefinitions;
using Grammars.Common.Grammars.ValueObjects.RuleNames;

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