using Grammars.Common;
using Grammars.Common.Convertings.Convertors;
using Grammars.Common.Convertings.Convertors.Epsilons;
using Grammars.Common.Convertings.Convertors.LeftRecursions;
using Grammars.Common.Convertings.Convertors.Semantics;
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