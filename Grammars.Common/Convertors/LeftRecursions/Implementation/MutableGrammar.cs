using Grammars.Common.ValueObjects;

namespace Grammars.Common.Convertors.LeftRecursions.Implementation;

internal class MutableGrammar
{
    public RuleName StartRule;
    public Dictionary<RuleName, GrammarRule> Rules = new();

    public MutableGrammar( CommonGrammar grammar )
    {
        StartRule = grammar.StartRule;
        Rules = grammar.Rules.ToDictionary( x => x.Key, x => x.Value );
    }

    public void Replace( GrammarRule rule )
    {
        Rules[rule.Name] = rule;
    }

    public CommonGrammar ToGrammar() => new( StartRule, Rules.Values );
}