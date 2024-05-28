using Grammars.Common.Convertings.Convertors.LeftFactorization.Implementation;

namespace Grammars.Common.Convertings.Convertors.LeftFactorization;

public class LeftFactorizationConvertor : IGrammarConvertor
{
    public CommonGrammar Convert( CommonGrammar grammar )
    {
        return new LeftFactorizationHandler().Factorize( grammar );
    }
}