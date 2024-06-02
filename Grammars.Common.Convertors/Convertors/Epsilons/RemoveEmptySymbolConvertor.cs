using Grammars.Common.Convertors.Convertors.Epsilons.Implementation;
using Grammars.Common.Grammars;

namespace Grammars.Common.Convertors.Convertors.Epsilons;

public class RemoveEmptySymbolConvertor : IGrammarConvertor
{
    public CommonGrammar Convert( CommonGrammar grammar )
    {
        return new RemoveEmptySymbolHandler().RemoveEpsilons( grammar );
    }
}