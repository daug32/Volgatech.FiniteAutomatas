using Grammars.Common.Convertors.Implementation.Inlining;

namespace Grammars.Common.Convertors.Convertors;

public class InlineNonTerminalsConvertor : IGrammarConvertor
{
    public CommonGrammar Convert( CommonGrammar grammar )
    {
        return new NonTerminalsInliner().Inline( grammar );
    }
}