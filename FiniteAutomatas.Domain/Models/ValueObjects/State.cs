namespace FiniteAutomatas.Domain.Models.ValueObjects;

public class State : IComparable
{
    public StateName Name { get; set; }
    public bool IsEnd;
    public bool IsError;
    public bool IsStart;
    
    public bool IsTerminateState => IsEnd || IsError;
    
    public State( StateName name, bool isStart = false, bool isEnd = false, bool isError = false )
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

    public override bool Equals( object? obj ) => obj is State other && Equals( other );

    public bool Equals( State other ) => Name.Equals( other.Name );

    public override int GetHashCode() => Name.GetHashCode();

    public override string ToString() => Name.ToString();

    public int CompareTo( object? obj )
    {
        if ( obj is not State other )
        {
            throw new ArgumentException();
        }

        return Name.CompareTo( other.Name );
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