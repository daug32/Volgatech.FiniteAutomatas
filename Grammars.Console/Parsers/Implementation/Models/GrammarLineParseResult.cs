using Grammars.Common.ValueObjects;

namespace Grammars.Console.Parsers.Implementation;

internal class GrammarLineParseResult
{
    public RuleName? RuleName;
    public List<RuleDefinition>? Rules = null;

    public bool HasData => RuleName is not null || Rules != null;
}