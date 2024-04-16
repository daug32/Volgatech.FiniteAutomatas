namespace Sudoku;

public class SudokuSimpleSolver
{
    public static Sudoku Solve( Sudoku sudoku )
    {
        Sudoku result = sudoku.Copy();

        for ( var index = 0; index < sudoku.NumberOfItems; index++ )
        {
            // If hint, skip it
            if ( sudoku.IsSet( index ) )
            {
                continue;
            }

            // If value was suggested, then use it
            if ( TryToSuggestValue( result, index, out int? suggestedValue ) )
            {
                result.SetValue( index, suggestedValue!.Value );
                continue;
            }

            // If value wasn't suggested, then go back to previous steps
            // Clean suggested values to avoid collision when suggesting a new one
            result.RemoveValue( index );
            index--;

            while ( true )
            {
                // If hint, skip it
                if ( sudoku.IsSet( index ) )
                {
                    index--;
                    continue;
                }

                // If we can't use a next value for that item, then skip it
                if ( result.GetValue( index ) + 1 > sudoku.MaxValue )
                {
                    result.RemoveValue( index );
                    index--;
                    continue;
                }

                // Break on value that is not a hint and can be switched to the next value
                break;
            }

            // Addition decrease is used to avoid for-loop increment  
            index--;
        }

        return result;
    }

    private static bool TryToSuggestValue( Sudoku sudoku, int index, out int? suggestedValue )
    {
        int initialValue = sudoku.GetValue( index ) + 1;
        
        for ( int possibleValue = initialValue; possibleValue <= sudoku.MaxValue; possibleValue++ )
        {
            bool isAlreadyInUse =
                sudoku.DoesColumnContains( index, possibleValue ) || 
                sudoku.DoesRowContains( index, possibleValue ) || 
                sudoku.HasValueInCurrentSquare( index, possibleValue );

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
}