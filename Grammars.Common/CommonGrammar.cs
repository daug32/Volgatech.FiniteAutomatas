using Grammars.Common.ValueObjects;

namespace Grammars.Common;

public class CommonGrammar
{
    public RuleName StartRule { get; }
    public IDictionary<RuleName, GrammarRule> Rules { get; protected set; }

    public CommonGrammar( RuleName startRule, IEnumerable<GrammarRule> rules )
    {
        StartRule = startRule;
        Rules = rules.ToDictionary(
            x => x.Name,
            x => x );
    }
}