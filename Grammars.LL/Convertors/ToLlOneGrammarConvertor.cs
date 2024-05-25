using Grammars.Common;
using Grammars.Common.Convertors;
using Grammars.Common.Convertors.LeftRecursions;
using Grammars.LL.Models;

namespace Grammars.LL.Convertors;

public class ToLlOneGrammarConvertor : IGrammarConvertor<LlOneGrammar>
{
    public LlOneGrammar Convert( CommonGrammar grammar )
    {
        CommonGrammar normalizedGrammar = grammar.Convert( new LeftRecursionRemoverConvertor() );

        return new LlOneGrammar( 
            normalizedGrammar.StartRule,
            normalizedGrammar.Rules.Values );
    }
}