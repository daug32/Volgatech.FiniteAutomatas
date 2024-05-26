using Grammars.Common.Convertors.LeftFactorization.Implementation;
using Grammars.Common.Convertors.LeftRecursions.Implementation;

namespace Grammars.Common.Convertors.LeftFactorization;

public class LeftFactorizationConvertor : IGrammarConvertor
{
    public CommonGrammar Convert( CommonGrammar grammar )
    {
        return new LeftFactorizationHandler().Factorize( new MutableGrammar( grammar ) );
    }
}