using Grammars.Common.Convertors.LeftFactorization.Implementation;
using Grammars.Common.Convertors.LeftFactorization.Implementation.Inlinings;
using Grammars.Common.Convertors.LeftRecursions.Implementation;

namespace Grammars.Common.Convertors.LeftFactorization;

public class LeftFactorizationConvertor : IGrammarConvertor
{
    public CommonGrammar Convert( CommonGrammar grammar )
    {
        var inliner = new BeggingNonTerminalsInliner();
        var mutableGrammar = new MutableGrammar( inliner.Inline( grammar ) );
        
        var factorizer = new LeftFactorizationHandler();
        return factorizer.Factorize( mutableGrammar );
    }
}