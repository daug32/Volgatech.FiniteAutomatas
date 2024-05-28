using Grammars.Common.ValueObjects;

namespace Grammar.Parsers.Implementation.Implementation.Models;

internal class GrammarLineParseResult
{
    public RuleName? RuleName;
    public List<RuleDefinition>? Rules = null;

    public bool HasData => RuleName is not null || Rules != null;
}