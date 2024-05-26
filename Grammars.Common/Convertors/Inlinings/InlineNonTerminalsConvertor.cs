using Grammars.Common.Convertors.LeftFactorization.Implementation.Inlinings;

namespace Grammars.Common.Convertors.Inlinings;

public class InlineNonTerminalsConvertor : IGrammarConvertor
{
    public CommonGrammar Convert( CommonGrammar grammar )
    {
        return new BeggingNonTerminalsInliner().Inline( grammar );
    }
}