using System.Text;
using Libs;

namespace Sudoku;

public class Program
{
    public static void Main( string[] args )
    {
        Test( 
            new[]
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
            }, 
            new[] 
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
            } );
        
        Test( 
            new[]
            {   
                new[] { 0, 0, 0,    0, 7, 0,    1, 0, 0 },
                new[] { 5, 0, 9,    0, 0, 0,    0, 0, 0 },
                new[] { 6, 0, 0,    0, 0, 0,    0, 0, 0 },
                
                new[] { 0, 7, 0,    0, 2, 0,    0, 0, 4 },
                new[] { 0, 0, 8,    0, 0, 0,    0, 6, 0 },
                new[] { 0, 0, 0,    0, 0, 0,    0, 5, 9 },
                
                new[] { 0, 3, 0,    0, 0, 0,    8, 0, 0 },
                new[] { 4, 0, 0,    6, 0, 0,    0, 0, 0 },
                new[] { 0, 0, 0,    9, 0, 0,    0, 0, 0 },
            }, 
            new[] 
            {
                new[] { 0, 0, 0,    0, 7, 0,    1, 0, 0 },
                new[] { 5, 0, 9,    0, 0, 0,    0, 0, 0 },
                new[] { 6, 0, 0,    0, 0, 0,    0, 0, 0 },
                
                new[] { 0, 7, 0,    0, 2, 0,    0, 0, 4 },
                new[] { 0, 0, 8,    0, 0, 0,    0, 6, 0 },
                new[] { 0, 0, 0,    0, 0, 0,    0, 5, 9 },
                
                new[] { 0, 3, 0,    0, 0, 0,    8, 0, 0 },
                new[] { 4, 0, 0,    6, 0, 0,    0, 0, 0 },
                new[] { 0, 0, 0,    9, 0, 0,    0, 0, 0 },
            } );
        
        CheckPerformance(
            new[]
            {   
                new[] { 0, 0, 0,    0, 7, 0,    1, 0, 0 },
                new[] { 5, 0, 9,    0, 0, 0,    0, 0, 0 },
                new[] { 6, 0, 0,    0, 0, 0,    0, 0, 0 },
                
                new[] { 0, 7, 0,    0, 2, 0,    0, 0, 4 },
                new[] { 0, 0, 8,    0, 0, 0,    0, 6, 0 },
                new[] { 0, 0, 0,    0, 0, 0,    0, 5, 9 },
                
                new[] { 0, 3, 0,    0, 0, 0,    8, 0, 0 },
                new[] { 4, 0, 0,    6, 0, 0,    0, 0, 0 },
                new[] { 0, 0, 0,    9, 0, 0,    0, 0, 0 },
            } );
    }

    public static void CheckPerformance( int[][] sudoku )
    {
        using ( TimeTracker.Track() )
        {
            for ( int i = 0; i < 10; i++ )
            {
                SudokuSimpleSolver.Solve( sudoku );
            }
        }
    }

    private static void Test( int[][] sudoku, int[][] expected )
    {
        int[][] result = SudokuSimpleSolver.Solve( sudoku );

        for ( var y = 0; y < 9; y++ )
        {
            for ( var x = 0; x < 9; x++ )
            {
                if ( result[y][x] != expected[y][x] )
                {
                    Print2dMap( sudoku );
                    Print2dMap( result );
                    return;
                }
            }
        }
    }

    private static void Print2dMap( int[][] map )
    {
        var message = new StringBuilder();

        for ( var y = 0; y < map.Length; y++ )
        {
            if ( y % 3 == 0 )
            {
                message.AppendLine();
            }
            
            for ( int x = 0; x < map[y].Length; x++ )
            {
                if ( x % 3 == 0 )
                {
                    message.Append( " " );
                }
                
                message.Append( map[y][x] );
            }
            
            message.AppendLine();
        }

        Console.WriteLine( message.ToString() );
    }
}