namespace Grammars.Common.Convertors;

// ReSharper disable once InconsistentNaming
public static class IGrammarConvertorExtensions
{
    public static CommonGrammar Convert( this CommonGrammar grammar, IGrammarConvertor convertor )
    {
        return convertor.Convert( grammar );
    }
    public static TOutput Convert<TOutput>( this CommonGrammar grammar, IGrammarConvertor<TOutput> convertor )
        where TOutput : CommonGrammar
    {
        return convertor.Convert( grammar );
    }
}