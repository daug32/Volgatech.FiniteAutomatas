using Grammars.LL.Models;

namespace Grammars.LL.Convertors;

public interface IGrammarConvertor
{
    LlOneGrammar Convert( LlOneGrammar grammar );
}