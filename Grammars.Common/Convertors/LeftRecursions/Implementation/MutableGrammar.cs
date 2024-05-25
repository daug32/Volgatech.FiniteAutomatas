using Grammars.Common.ValueObjects;

namespace Grammars.Common.Convertors.LeftRecursions.Implementation;

internal class MutableGrammar : CommonGrammar
{
    public MutableGrammar( CommonGrammar grammar )
        : base( grammar.StartRule, grammar.Rules.Values )
    {
    }

    public void Replace( GrammarRule rule )
    {
        Rules[rule.Name] = rule;
    }

    public CommonGrammar ToGrammar() => new( StartRule, Rules.Values );
}