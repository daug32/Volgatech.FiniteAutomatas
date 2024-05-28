using Grammars.Common;

namespace Grammar.Parsers;

public interface IGrammarParser
{
    CommonGrammar Parse();
}