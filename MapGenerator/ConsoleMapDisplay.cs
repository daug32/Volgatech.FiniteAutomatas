namespace MapGenerator;

public static class ConsoleMapDisplay
{
    public static void Display( Map map )
    {
        var serializedMap = new string[ map.Count + 2 ];
        serializedMap[0] = serializedMap[^1] = new string( '-', map.First().Count );

        for ( var y = 0; y < map.Count; y++ )
        {
            List<MapUnit> mapRow = map[y];
            
            var serializedRow = new char[mapRow.Count + 2];
            
            serializedRow[0] = serializedRow[^1] = '|';
            for ( int x = 1; x < serializedRow.Length - 1; x++ )
            {
                serializedRow[x] = MapUnitTypeToChar( mapRow[x - 1].Type );
            }

            serializedMap[y + 1] = new string( serializedRow );
        }

        foreach ( string s in serializedMap )
        {
            Console.WriteLine( s );
        }
    }

    private static char MapUnitTypeToChar( MapUnitType mapUnitType )
    {
        return mapUnitType switch
        {
            MapUnitType.None => ' ',
            MapUnitType.Wall => '#',
            _ => throw new ArgumentOutOfRangeException( nameof( mapUnitType ), mapUnitType, null )
        };
    }
}