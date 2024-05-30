using Grammars.Common.Grammars;

namespace Grammars.Common.Convertors.Convertors;

public class RemoveEpsilonsConvertor : IGrammarConvertor
{
    public CommonGrammar Convert( CommonGrammar grammar )
    {
        return new EpsilonRemover().RemoveEpsilons( grammar );
    }
}