using Grammars.Common.Convertors.Convertors.Epsilons.Implementation;
using Grammars.Common.Grammars;

namespace Grammars.Common.Convertors.Convertors.Epsilons;

public class RemoveEpsilonsConvertor : IGrammarConvertor
{
    public CommonGrammar Convert( CommonGrammar grammar )
    {
        return new RemoveEpsilonsHandler().RemoveEpsilons( grammar );
    }
}