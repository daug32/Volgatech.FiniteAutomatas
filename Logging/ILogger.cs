namespace Logging;

public interface ILogger
{
    public void Write( LogLevel logLevel, string message );
}