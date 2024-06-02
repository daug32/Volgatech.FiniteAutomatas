using Grammars.Common.Convertors;
using Grammars.Common.Grammars;

namespace Grammars.SLR.Convertors.Implementation;

internal class RemoveReferencesToStartRuleConvertor : IGrammarConvertor
{
    public CommonGrammar Convert( CommonGrammar grammar )
    {
        return new RemoveReferencesToStartRuleHandler( grammar ).RemoveReferencesToStartRule();
    }
}