using Grammars.LL.Models.ValueObjects;

namespace Grammars.LL.Console.Parsers.Implementation;

internal class GrammarLineParseResult
{
    public RuleName? RuleName;
    public List<RuleDefinition>? Rules = null;

    public bool HasData => RuleName is not null || Rules != null;
}