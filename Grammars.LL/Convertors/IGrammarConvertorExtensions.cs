using Grammars.LL.Models;

namespace Grammars.LL.Convertors;

// ReSharper disable once InconsistentNaming
public static class IGrammarConvertorExtensions
{
    public static LlOneGrammar Convert( this LlOneGrammar grammar, IGrammarConvertor convertor )
    {
        return convertor.Convert( grammar );
    }
}