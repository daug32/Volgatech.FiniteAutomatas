namespace MapGenerator;

public class MapUnit
{
    public MapUnitType Type { get; set; }

    public MapUnit( MapUnitType type )
    {
        Type = type;
    }
}