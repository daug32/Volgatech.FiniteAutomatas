namespace Grammars.Common.Runners.Results;

public class RunError
{
    public readonly Exception? Exception;
    public readonly int? InvalidSymbolIndex;

    public static RunError UnhandledException( Exception exception ) => new( null, exception );
    public static RunError InvalidSentence( int invalidSymbolIndex ) => new( invalidSymbolIndex, null );
    
    private RunError( int? invalidSymbolIndex, Exception? exception )
    {
        InvalidSymbolIndex = invalidSymbolIndex;
        Exception = exception;
    }
}