using Grammars.Common.Convertors.Implementation;
using Grammars.Common.Grammars;

namespace Grammars.Common.Convertors.Convertors;

public class LeftRecursionRemoverConvertor : IGrammarConvertor
{
    public CommonGrammar Convert( CommonGrammar grammar )
    {
        return new LeftRecursionRemover( grammar ).RemoveLeftRecursion();
    }
}