using Grammars.Common.Convertors.Convertors.Inlining.Implementation;
using Grammars.Common.Grammars;

namespace Grammars.Common.Convertors.Convertors.Inlining;

public class InlineRulesConvertor : IGrammarConvertor
{
    public CommonGrammar Convert( CommonGrammar grammar )
    {
        return new InlineRulesHandler().Inline( grammar );
    }
}