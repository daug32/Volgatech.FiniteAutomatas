using Grammars.LL.Models;
using Grammars.LL.Models.ValueObjects;

namespace Grammars.LL.Convertors.LeftRecursions.Implementation;

public class MutableGrammar
{
    public RuleName StartRule;
    public Dictionary<RuleName, GrammarRule> Rules = new();

    public MutableGrammar( LlOneGrammar grammar )
    {
        StartRule = grammar.StartRule;
        Rules = grammar.Rules.ToDictionary( x => x.Key, x => x.Value );
    }

    public void Replace( GrammarRule rule )
    {
        Rules[rule.Name] = rule;
    }

    public LlOneGrammar ToLlOneGrammar() => new( StartRule, Rules.Values );
}