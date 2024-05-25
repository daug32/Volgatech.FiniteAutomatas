using Grammars.LL.Convertors.LeftRecursions.Implementation;
using Grammars.LL.Models;

namespace Grammars.LL.Convertors.LeftRecursions;

public class LeftRecursionRemoverConvertor : IGrammarConvertor
{
    public LlOneGrammar Convert( LlOneGrammar grammar )
    {
        return new LeftRecursionRemover().RemoveLeftRecursion( grammar );
    }
}