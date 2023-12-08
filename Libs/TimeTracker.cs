namespace Libs;

public class TimeTracker : IDisposable
{
    private readonly DateTime _start;

    public static TimeTracker Track()
    {
        return new( DateTime.Now );
    }

    private TimeTracker( DateTime start )
    {
        _start = start;
    }

    public void Dispose()
    {
        Console.WriteLine( DateTime.Now - _start );
    }
}