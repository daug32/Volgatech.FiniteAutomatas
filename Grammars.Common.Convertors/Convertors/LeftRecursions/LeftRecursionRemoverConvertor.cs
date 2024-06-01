using Grammars.Common.Convertors.Convertors.LeftRecursions.Implementation;
using Grammars.Common.Grammars;

namespace Grammars.Common.Convertors.Convertors.LeftRecursions;

public class LeftRecursionRemoverConvertor : IGrammarConvertor
{
    public CommonGrammar Convert( CommonGrammar grammar )
    {
        return new LeftRecursionRemoverHandler( grammar ).RemoveLeftRecursion();
    }
}