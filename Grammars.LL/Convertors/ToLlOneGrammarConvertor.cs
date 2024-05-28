using Grammars.Common;
using Grammars.Common.Convertors;
using Grammars.Common.Convertors.Convertors;
using Grammars.LL.Models;

namespace Grammars.LL.Convertors;

public class ToLlOneGrammarConvertor : IGrammarConvertor<LlOneGrammar>
{
    public LlOneGrammar Convert( CommonGrammar grammar )
    {
        CommonGrammar normalizedGrammar = grammar
            .Convert( new RemoveEpsilonsConvertor()  )
            .Convert( new LeftRecursionRemoverConvertor() )
            .Convert( new RemoveEpsilonsConvertor() )
            .Convert( new RenameRuleNamesConvertor() );

        return new LlOneGrammar( 
            normalizedGrammar.StartRule,
            normalizedGrammar.Rules.Values );
    }
}