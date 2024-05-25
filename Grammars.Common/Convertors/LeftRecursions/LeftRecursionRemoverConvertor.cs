using Grammars.Common.Convertors.LeftRecursions.Implementation;

namespace Grammars.Common.Convertors.LeftRecursions;

public class LeftRecursionRemoverConvertor : IGrammarConvertor
{
    public CommonGrammar Convert( CommonGrammar grammar )
    {
        return new LeftRecursionRemover().RemoveLeftRecursion( grammar );
    }
}