using Grammars.Common.Grammars;

namespace Grammars.Common.Convertors;

public interface IGrammarConvertor
{
    CommonGrammar Convert( CommonGrammar grammar );
}

public interface IGrammarConvertor<out TOutput> where TOutput : CommonGrammar
{
    TOutput Convert( CommonGrammar grammar );
}