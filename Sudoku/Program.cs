namespace Sudoku;

public class Program
{
    public static void Main( string[] args )
    {
        int[][] result = Solve( new[]
        {   
            new[] { 0, 0, 0,   0, 0, 0,    0, 5, 0 },
            new[] { 0, 0, 4,   0, 0, 8,    6, 0, 0 },
            new[] { 0, 0, 0,   0, 7, 0,    1, 0, 2 },
            
            new[] { 6, 0, 0,   0, 0, 0,    0, 0, 0 },
            new[] { 0, 0, 9,   0, 3, 0,    8, 0, 0 },
            new[] { 4, 7, 0,   9, 0, 0,    0, 0, 6 },
            
            new[] { 0, 1, 3,   4, 0, 0,    0, 0, 0 },
            new[] { 0, 0, 0,   0, 0, 0,    0, 0, 0 },
            new[] { 7, 0, 0,   2, 9, 6,    0, 0, 0 },
        } );

        int[][] expected = 
        {
            new[] { 2, 8, 7,    1, 6, 9,    4, 5, 3 },
            new[] { 1, 5, 4,    3, 2, 8,    6, 9, 7 },
            new[] { 3, 9, 6,    5, 7, 4,    1, 8, 2 },
            
            new[] { 6, 3, 8,    7, 4, 5,    9, 2, 1 },
            new[] { 5, 2, 9,    6, 3, 1,    8, 7, 4 },
            new[] { 4, 7, 1,    9, 8, 2,    5, 3, 6 },
            
            new[] { 8, 1, 3,    4, 5, 7,    2, 6, 9 },
            new[] { 9, 6, 2,    8, 1, 3,    7, 4, 5 },
            new[] { 7, 4, 5,    2, 9, 6,    3, 1, 8 },
        };

        Print( result );
    }

    public static int[][] Solve( int[][] sudoku )
    {
        var result = new int[9][];
        for ( int y = 0; y < 9; y++ )
        {
            result[y] = new int[9];
            for ( int x = 0; x < 9; x++ )
            {
                result[y][x] = sudoku[y][x];
            }
        }

        for ( int index = 0; index < 9 * 9; index++ )
        {
            int x = index % 9;
            int y = index / 9;

            // Если в оригинальном судоку стоит значение, мы не имеем права его менять, пропускаем
            if ( sudoku[y][x] != 0 )
            {
                continue;
            }

            int initialValue = result[y][x] + 1;
            
            bool setValue = false;
            for ( int z = initialValue; z <= 9; z++ )
            {
                bool valueAlreadyExists =
                    ColumnContains( result, x, z ) ||
                    RowContains( result, y, z ) ||
                    HasInSquare( result, x, y, z );

                // Если можем, ставим значение и говорим, что так и надо
                if ( !valueAlreadyExists )
                {
                    result[y][x] = z;
                    setValue = true;
                    break;
                }
            }

            // Если мы не смогли поставить значение, значит, надо вернуть назад и попробовать другое
            if ( !setValue )
            {
                result[y][x] = 0;
                index--;
                x = index % 9;
                y = index / 9;

                while ( true )
                {
                    if ( sudoku[y][x] != 0 )
                    {
                        index--;
                        x = index % 9;
                        y = index / 9;
                        continue;
                    }

                    if ( result[y][x] + 1 > 9 )
                    {
                        result[y][x] = 0;
                        index--;
                        x = index % 9;
                        y = index / 9;
                        continue;
                    }

                    break;
                }

                index--;
            }
        }

        return result;
    }

    private static bool ColumnContains( int[][] sudoku, int x, int valueToSearch )
    {
        for ( int y = 0; y < 9; y++ )
        {
            int existentValue = sudoku[y][x];
            if ( existentValue == valueToSearch )
            {
                return true;
            }
        }

        return false;
    }

    private static bool RowContains( int[][] sudoku, int y, int valueToSearch )
    {
        for ( int x = 0; x < 9; x++ )
        {
            int existentValue = sudoku[y][x];
            if ( existentValue == valueToSearch )
            {
                return true;
            }
        }

        return false;
    }

    private static bool HasInSquare(
        int[][] sudoku,
        int columnIndex,
        int rowIndex,
        int valueToSearch )
    {
        int startX = ( columnIndex / 3 ) * 3;
        int endX = startX + 3;
        
        int startY = ( rowIndex / 3 ) * 3;
        int endY = startY + 3;

        for ( int x = startX; x < endX; x++ )
        {
            for ( int y = startY; y < endY; y++ )
            {
                int existentValue = sudoku[y][x];
                if ( existentValue == 0 )
                {
                    continue;
                }
                
                if ( existentValue == valueToSearch )
                {
                    return true;
                }
            }
        }
        
        return false;
    }

    private static void Print( int[][] sudoku )
    {
        for ( int y = 0; y < 9; y++ )
        {
            if ( y % 3 == 0 )
            {
                Console.WriteLine();
            }

            for ( int x = 0; x < 9; x++ )
            {
                if ( x % 3 == 0 )
                {
                    Console.Write( " " );
                }

                Console.Write( sudoku[y][x] );
            }

            Console.WriteLine();
        }
    }
}