namespace Grammars.LL.Runners.Results;

public class RunError
{
    public readonly bool IsNotLlGrammar;
    public readonly int? InvalidSymbolIndex;

    public static RunError NotLl() => new( null, true );
    public static RunError InvalidSentence( int invalidSymbolIndex ) => new( invalidSymbolIndex, false );
    
    private RunError( int? invalidSymbolIndex, bool isNotLlGrammar )
    {
        InvalidSymbolIndex = invalidSymbolIndex;
        IsNotLlGrammar = isNotLlGrammar;
    }
}