using Grammars.Common.Convertors.Implementation.Factorization;
using Grammars.Common.Grammars;

namespace Grammars.Common.Convertors.Convertors;

public class LeftFactorizationConvertor : IGrammarConvertor
{
    public CommonGrammar Convert( CommonGrammar grammar )
    {
        return new LeftFactorizationHandler( grammar ).Factorize();
    }
}