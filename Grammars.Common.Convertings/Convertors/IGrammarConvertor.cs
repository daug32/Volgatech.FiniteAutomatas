namespace Grammars.Common.Convertings.Convertors;

public interface IGrammarConvertor
{
    CommonGrammar Convert( CommonGrammar grammar );
}

public interface IGrammarConvertor<out TOutput> where TOutput : CommonGrammar
{
    TOutput Convert( CommonGrammar grammar );
}