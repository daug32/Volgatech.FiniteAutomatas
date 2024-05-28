using Grammars.Common.ValueObjects;
using Grammars.Common.ValueObjects.Symbols;

namespace Grammar.Common.Tests;

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