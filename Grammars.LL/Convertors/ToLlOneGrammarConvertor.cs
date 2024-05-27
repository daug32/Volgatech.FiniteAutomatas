using Grammars.Common;
using Grammars.Common.Convertors;
using Grammars.Common.Convertors.LeftFactorization;
using Grammars.Common.Convertors.LeftRecursions;
using Grammars.Common.Convertors.Semantics;
using Grammars.LL.Models;

namespace Grammars.LL.Convertors;

public class ToLlOneGrammarConvertor : IGrammarConvertor<LlOneGrammar>
{
    public LlOneGrammar Convert( CommonGrammar grammar )
    {
        CommonGrammar normalizedGrammar = grammar
            .Convert( new LeftFactorizationConvertor() )
            .Convert( new LeftRecursionRemoverConvertor() )
            .Convert( new RenameRuleNamesConvertor() );

        return new LlOneGrammar( 
            normalizedGrammar.StartRule,
            normalizedGrammar.Rules.Values );
    }
}