using Grammars.Common.Convertings.Convertors.Inlinings.Implementation;

namespace Grammars.Common.Convertings.Convertors.Inlinings;

public class InlineNonTerminalsConvertor : IGrammarConvertor
{
    public CommonGrammar Convert( CommonGrammar grammar )
    {
        return new NonTerminalsInliner().Inline( grammar );
    }
}