using Grammars.Common.Convertings.Convertors.LeftRecursions.Implementation;

namespace Grammars.Common.Convertings.Convertors.LeftRecursions;

public class LeftRecursionRemoverConvertor : IGrammarConvertor
{
    public CommonGrammar Convert( CommonGrammar grammar )
    {
        return new LeftRecursionRemover().RemoveLeftRecursion( grammar );
    }
}