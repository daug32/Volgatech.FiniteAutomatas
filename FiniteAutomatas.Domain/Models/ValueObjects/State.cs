namespace FiniteAutomatas.Domain.Models.ValueObjects;

public class State : IComparable
{
    public string Name { get; set; }
    public bool IsEnd;
    public bool IsError;
    public bool IsStart;
    
    public bool IsTerminateState => IsEnd || IsError;
    
    public State( string name, bool isStart = false, bool isEnd = false, bool isError = false )
    {
        Name = name;
        IsStart = isStart;
        IsEnd = isEnd;
        IsError = isError;

        if ( IsError && IsEnd )
        {
            throw new ArgumentException( "State can't be both terminating and error" );
        }
    }

    public override bool Equals( object? obj )
    {
        return obj is State other && Equals( other );
    }

    public bool Equals( State other )
    {
        return Name == other.Name;
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }

    public override string ToString()
    {
        return Name;
    }

    public int CompareTo( object? obj )
    {
        if ( obj is not State other )
        {
            throw new ArgumentException();
        }

        return String.CompareOrdinal( Name, other.Name );
    }

    public static bool operator ==( State a, State b )
    {
        return a.Equals( b );
    }

    public static bool operator !=( State a, State b )
    {
        return !a.Equals( b );
    }

    public State Copy() => new(
        Name,
        IsStart,
        IsEnd,
        IsError );
}