using Grammars.Common.Grammars.ValueObjects;
using Grammars.Common.Grammars.ValueObjects.GrammarRules;
using Grammars.Common.Grammars.ValueObjects.RuleDefinitions;
using Grammars.Common.Grammars.ValueObjects.RuleNames;

namespace Grammar.Parsers.Implementation.Implementation.Models;

internal class GrammarLineParseResult
{
    public RuleName? RuleName;
    public List<RuleDefinition>? Rules = null;

    public bool HasData => RuleName is not null || Rules != null;
}