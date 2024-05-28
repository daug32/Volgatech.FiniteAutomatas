using Grammars.Common.Convertors.Implementation.Recursions;
using Grammars.Common.Grammars;

namespace Grammars.Common.Convertors.Convertors;

public class LeftRecursionRemoverConvertor : IGrammarConvertor
{
    public CommonGrammar Convert( CommonGrammar grammar )
    {
        return new LeftRecursionRemover().RemoveLeftRecursion( grammar );
    }
}