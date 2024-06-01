using Grammars.Common.Convertors.Convertors.Whitespaces.Implementation;
using Grammars.Common.Grammars;

namespace Grammars.Common.Convertors.Convertors.Whitespaces;

public class RemoveWhitespacesConvertor : IGrammarConvertor
{
    public CommonGrammar Convert( CommonGrammar grammar )
    {
        return new RemoveWhitespacesHandler().RemoveWhitespaces( grammar );
    }
}