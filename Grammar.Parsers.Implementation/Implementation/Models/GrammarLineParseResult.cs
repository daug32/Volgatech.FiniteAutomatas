using Grammars.Common.Grammars.ValueObjects;
using Grammars.Common.Grammars.ValueObjects.RuleDefinitions;

namespace Grammar.Parsers.Implementation.Implementation.Models;

internal class GrammarLineParseResult
{
    public RuleName? RuleName;
    public List<RuleDefinition>? Rules = null;

    public bool HasData => RuleName is not null || Rules != null;
}