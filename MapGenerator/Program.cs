namespace MapGenerator;

public class Program
{
    public static void Main( string[] args )
    {
        var map = new Map( 10, 20 );
        ConsoleMapDisplay.Display( map );
    }
}
