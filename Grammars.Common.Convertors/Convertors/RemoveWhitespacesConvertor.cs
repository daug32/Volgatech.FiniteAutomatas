using Grammars.Common.Grammars;

namespace Grammars.Common.Convertors.Convertors;

public class RemoveWhitespacesConvertor : IGrammarConvertor
{
    public CommonGrammar Convert( CommonGrammar grammar )
    {
        return new WhitespacesRemover().RemoveWhitespaces( grammar );
    }
}