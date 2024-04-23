namespace FiniteAutomatas.Domain.Models.ValueObjects;

public class State : IComparable
{
    public StateId Id { get; set; }
    public bool IsEnd;
    public bool IsError;
    public bool IsStart;
    
    public bool IsTerminateState => IsEnd || IsError;
    
    public State( StateId id, bool isStart = false, bool isEnd = false, bool isError = false )
    {
        Id = id;
        IsStart = isStart;
        IsEnd = isEnd;
        IsError = isError;

        if ( IsError && IsEnd )
        {
            throw new ArgumentException( "State can't be both terminating and error" );
        }
    }

    public override bool Equals( object? obj ) => obj is State other && Equals( other );

    public bool Equals( State other ) => Id.Equals( other.Id );

    public override int GetHashCode() => Id.GetHashCode();

    public override string ToString() => Id.ToString();

    public int CompareTo( object? obj )
    {
        if ( obj is not State other )
        {
            throw new ArgumentException();
        }

        return Id.CompareTo( other.Id );
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
        Id,
        IsStart,
        IsEnd,
        IsError );
}