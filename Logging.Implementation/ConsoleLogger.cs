namespace Logging.Implementation;

public class ConsoleLogger : ILogger
{
    public void Write( LogLevel logLevel, string message )
    {
        switch ( logLevel )
        {
            case LogLevel.Warning:
                ConsoleColor savedColor = Console.ForegroundColor; 
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write( "WARNING" );
                Console.ForegroundColor = savedColor;
                Console.WriteLine( $": {message}" );
                return;
            default: throw new ArgumentOutOfRangeException( nameof( logLevel ), logLevel, null );
        }
    }
}