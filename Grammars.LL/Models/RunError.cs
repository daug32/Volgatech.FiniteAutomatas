namespace Grammars.LL.Models;

public class RunError
{
    public readonly int InvalidSymbolIndex;

    public RunError( int invalidSymbolIndex )
    {
        InvalidSymbolIndex = invalidSymbolIndex;
    }
}