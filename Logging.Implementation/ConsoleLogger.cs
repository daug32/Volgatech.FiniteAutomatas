namespace Logging.Implementation;

public class ConsoleLogger : ILogger
{
    public LogLevel MinimalLogLevel { get; set; } = LogLevel.Error;

    public void Write( LogLevel logLevel, string message )
    {
        if ( logLevel < MinimalLogLevel )
        {
            return;
        }
        
        ConsoleColor savedColor;
        switch ( logLevel )
        {
            case LogLevel.Debug:
                savedColor = Console.ForegroundColor; 
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write( "DEBUG" );
                Console.WriteLine( $": {message}" );
                Console.ForegroundColor = savedColor;
                return;
            case LogLevel.Warning:
                savedColor = Console.ForegroundColor; 
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write( "WARNING" );
                Console.ForegroundColor = savedColor;
                Console.WriteLine( $": {message}" );
                return;
            case LogLevel.Error:
                savedColor = Console.ForegroundColor; 
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write( "ERROR" );
                Console.ForegroundColor = savedColor;
                Console.WriteLine( $": {message}" );
                return;
            case LogLevel.None: return;
            default: throw new ArgumentOutOfRangeException( nameof( logLevel ), logLevel, null );
        }
    }
}