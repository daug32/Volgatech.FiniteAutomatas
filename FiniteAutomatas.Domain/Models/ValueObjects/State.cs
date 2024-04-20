namespace FiniteAutomatas.Domain.Models.ValueObjects;

public class State : IComparable
{
    public string Name { get; set; }
    public bool IsEnd;
    public bool IsStart;

    public State( string name, bool isStart = false, bool isEnd = false )
    {
        Name = name;
        IsStart = isStart;
        IsEnd = isEnd;
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
}