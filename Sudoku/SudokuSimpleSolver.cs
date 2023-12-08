namespace Sudoku;

public static class SudokuSimpleSolver
{
    private static readonly int _sudokuSize = 9;
    private static readonly int _sudokuSmallSquareSize = 3;
    
    private static readonly int _sudokuMaxValue = 9;
    private static readonly int _sudokuMeaninglessValue = 0;

    public static int[][] Solve( int[][] sudoku )
    {
        return Solve( ArrayHandler.Convert2dArrayMapToArray( sudoku ) );
    }
    
    public static int[][] Solve( int[] singleArraySudoku )
    {
        var result = new int[_sudokuSize * _sudokuSize];
        Array.Copy( singleArraySudoku, result, _sudokuSize * _sudokuSize );

        for ( var index = 0; index < _sudokuSize * _sudokuSize; index++ )
        {
            // If hint, skip it
            if ( singleArraySudoku[index] != _sudokuMeaninglessValue )
            {
                continue;
            }

            // If value was suggested, then use it
            if ( TryToSuggestValue( result, index, out int? suggestedValue ) )
            {
                result[index] = suggestedValue!.Value;
                continue;
            }

            // If value wasn't suggested, then go back to previous steps
            // Clean suggested values to avoid collision when suggesting a new one
            result[index] = _sudokuMeaninglessValue;
            index--;

            while ( true )
            {
                // If hint, skip it
                if ( singleArraySudoku[index] != _sudokuMeaninglessValue )
                {
                    index--;
                    continue;
                }

                // If we can't use a next value for that item, then skip it
                if ( result[index] + 1 > _sudokuSize )
                {
                    result[index] = _sudokuMeaninglessValue;
                    index--;
                    continue;
                }

                // Break on value that is not a hint and can be switched to the next value
                break;
            }

            // Addition decrease is used to avoid for-loop increment  
            index--;
        }

        return ArrayHandler.ConvertArrayTo2dArrayMap( result, _sudokuSize, _sudokuSize );
    }

    private static bool TryToSuggestValue( int[] singleArraySudoku, int index, out int? suggestedValue )
    {
        int initialValue = singleArraySudoku[index] + 1;
        
        for ( int possibleValue = initialValue; possibleValue <= _sudokuMaxValue; possibleValue++ )
        {
            bool isAlreadyInUse =
                DoesColumnContains( singleArraySudoku, index, possibleValue ) || 
                DoesRowContains( singleArraySudoku, index, possibleValue ) || 
                HasValueInCurrentSquare( singleArraySudoku, index, possibleValue );

            // Skip if the value is not free
            if ( isAlreadyInUse )
            {
                continue;
            }

            suggestedValue = possibleValue;
            return true;
        }

        suggestedValue = null;
        return false;
    }

    private static bool DoesColumnContains( int[] singleArraySudoku, int index, int valueToSearch )
    {
        int offset = index % _sudokuSize;
        for ( var y = 0; y < _sudokuSize; y++ )
        {
            int existentValue = singleArraySudoku[y * _sudokuSize + offset];
            if ( existentValue == valueToSearch )
            {
                return true;
            }
        }

        return false;
    }

    private static bool DoesRowContains( 
        IReadOnlyList<int> singleArraySudoku,
        int index, 
        int valueToSearch )
    {
        int offset = index / _sudokuSize * _sudokuSize;
        for ( var x = 0; x < _sudokuSize; x++ )
        {
            int existentValue = singleArraySudoku[offset + x];
            if ( existentValue == valueToSearch )
            {
                return true;
            }
        }

        return false;
    }

    private static bool HasValueInCurrentSquare(
        IReadOnlyList<int> singleArraySudoku,
        int index,
        int valueToSearch )
    {
        int posX = index % _sudokuSize;
        int startX = posX / _sudokuSmallSquareSize * _sudokuSmallSquareSize;
        int endX = startX + _sudokuSmallSquareSize;

        int posY = index / _sudokuSize;
        int startY = posY / _sudokuSmallSquareSize * _sudokuSmallSquareSize;
        int endY = startY + _sudokuSmallSquareSize;

        for ( int y = startY; y < endY; y++ )
        {
            int offset = y * _sudokuSize;
            for ( int x = startX; x < endX; x++ )
            {
                int existentValue = singleArraySudoku[offset + x];
                if ( existentValue == valueToSearch )
                {
                    return true;
                }
            }
        }

        return false;
    }
}