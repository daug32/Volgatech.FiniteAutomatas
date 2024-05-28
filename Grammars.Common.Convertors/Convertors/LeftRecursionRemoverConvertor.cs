using Grammars.Common.Convertors.Implementation.Recursions;

namespace Grammars.Common.Convertors.Convertors;

public class LeftRecursionRemoverConvertor : IGrammarConvertor
{
    public CommonGrammar Convert( CommonGrammar grammar )
    {
        return new LeftRecursionRemover().RemoveLeftRecursion( grammar );
    }
}