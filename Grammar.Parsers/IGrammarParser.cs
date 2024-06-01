using Grammars.Common.Grammars;

namespace Grammar.Parsers;

public interface IGrammarParser
{
    CommonGrammar Parse();
}