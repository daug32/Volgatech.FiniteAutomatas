using Grammars.Common.Convertors.Convertors.Factorization.Implementation;
using Grammars.Common.Grammars;

namespace Grammars.Common.Convertors.Convertors.Factorization;

public class LeftFactorizationConvertor : IGrammarConvertor
{
    public CommonGrammar Convert( CommonGrammar grammar )
    {
        return new LeftFactorizationHandler( grammar ).Factorize();
    }
}