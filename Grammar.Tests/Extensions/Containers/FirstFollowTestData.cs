using Grammars.Common.Grammars.ValueObjects.RuleNames;
using Grammars.Common.Grammars.ValueObjects.Symbols;

namespace Grammar.Tests.Extensions.Containers;

public class FirstFollowTestData
{
    public readonly RuleName TargetRuleName;
    public readonly string RawGrammar;
    public readonly HashSet<RuleSymbol> ExpectedRuleSymbols;

    public FirstFollowTestData(
        RuleName ruleName,
        string rawGrammar,
        IEnumerable<RuleSymbol> expectedRuleSymbols )
    {
        TargetRuleName = ruleName;
        RawGrammar = rawGrammar;
        ExpectedRuleSymbols = expectedRuleSymbols.ToHashSet();
    }
}