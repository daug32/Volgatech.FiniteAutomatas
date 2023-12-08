namespace MapGenerator;

public class Map : List<List<MapUnit>>
{
    public Map( int width, int height )
        : base( Enumerable
            .Range( 0, height )
            .Select( _ => Enumerable
                .Range( 0, width )
                .Select( __ => new MapUnit( MapUnitType.None ) )
                .ToList() ) )
    {
    }
}