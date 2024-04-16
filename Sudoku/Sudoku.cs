namespace Sudoku;

public class Sudoku
{
    private bool _hasChanges = true;
    private readonly int _meaninglessValue = 0;
    private readonly int[] _singleArraySudokuData;

    public int NumberOfItems => Size * Size;
    public int Size { get; private set; }
    public int SquareSize { get; private set; }
    public int MaxValue { get; private set; }

    public Sudoku( int[][] map )
    {
        Size = map.Length;
        SquareSize = ( int )Math.Sqrt( Size );
        MaxValue = Size;
        
        _singleArraySudokuData = ArrayHandler.Convert2dArrayMapToArray( map );
    }

    public Sudoku( int[] singleArraySudoku, int size = 9 )
    {
        Size = size;
        SquareSize = ( int )Math.Sqrt( size );
        MaxValue = size;

        _singleArraySudokuData = singleArraySudoku;
    }

    public bool IsSolved()
    {
        return _singleArraySudokuData.All( x => x != _meaninglessValue );
    }

    public bool IsSet( int index )
    {
        return _singleArraySudokuData[index] != 0;
    }

    public int GetValue( int index )
    {
        return _singleArraySudokuData[index];
    } 

    public void SetValue( int index, int value )
    {
        if ( value <= _meaninglessValue || value > MaxValue )
        {
            throw new ArgumentException();
        } 

        _singleArraySudokuData[index] = value;
        _hasChanges = true;
    }

    public void RemoveValue( int index )
    {
        _singleArraySudokuData[index] = _meaninglessValue;
        _hasChanges = true;
    }

    public Sudoku Copy()
    {
        var newData = new int[_singleArraySudokuData.Length];
        _singleArraySudokuData.CopyTo( newData, 0 );
        return new Sudoku( newData );
    }
    
    public bool DoesColumnContains( int index, int valueToSearch )
    {
        int offset = index % Size;
        for ( var y = 0; y < Size; y++ )
        {
            int existentValue = _singleArraySudokuData[y * Size + offset];
            if ( existentValue == valueToSearch )
            {
                return true;
            }
        }

        return false;
    }
    
    public bool DoesRowContains( int index, int valueToSearch )
    {
        int offset = index / Size * Size;
        for ( var x = 0; x < Size; x++ )
        {
            int existentValue = _singleArraySudokuData[offset + x];
            if ( existentValue == valueToSearch )
            {
                return true;
            }
        }

        return false;
    }
    
    public bool HasValueInCurrentSquare( int index, int valueToSearch )
    {
        int posX = index % Size;
        int startX = posX / SquareSize * SquareSize;
        int endX = startX + SquareSize;

        int posY = index / Size;
        int startY = posY / SquareSize * SquareSize;
        int endY = startY + SquareSize;

        for ( int y = startY; y < endY; y++ )
        {
            int offset = y * Size;
            for ( int x = startX; x < endX; x++ )
            {
                int existentValue = _singleArraySudokuData[offset + x];
                if ( existentValue == valueToSearch )
                {
                    return true;
                }
            }
        }

        return false;
    }

    public int[][] To2dArray() => ArrayHandler.ConvertArrayTo2dArrayMap(
        _singleArraySudokuData,
        Size, 
        Size );
}