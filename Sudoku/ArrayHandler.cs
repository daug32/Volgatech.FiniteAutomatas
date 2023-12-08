namespace Sudoku;

public static class ArrayHandler
{
    public static int[] Convert2dArrayMapToArray( int[][] map )
    {
        int height = map.Length;
        int width = map.First().Length;

        var result = new int[width * height];

        for ( var y = 0; y < height; y++ )
        {
            int offset = y * width;
            Array.Copy( map[y], 0, result, offset, width );
        }

        return result;
    }

    public static int[][] ConvertArrayTo2dArrayMap( int[] singleArrayMap, int width, int height )
    {
        var result = new int[height][];

        for ( var y = 0; y < height; y++ )
        {
            int offset = y * width;
            result[y] = new int[width];
            Array.Copy( singleArrayMap, offset, result[y], 0, width );
        }

        return result;
    }
}