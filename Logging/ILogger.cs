namespace Logging;

public interface ILogger
{
    public LogLevel MinimalLogLevel { get; set; }
    public void Write( LogLevel logLevel, string message );
}