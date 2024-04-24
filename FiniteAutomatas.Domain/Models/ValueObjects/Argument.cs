namespace FiniteAutomatas.Domain.Models.ValueObjects;

public class Argument<T> : IComparable
{
    public static readonly Dictionary<Type, IComparable> Comparers = new();  
        
    public readonly T? Value;

    public static Argument<T> Epsilon => new( default );

    public Argument( T? value )
    {
        Value = value;
    }

    public override bool Equals( object? obj )
    {
        return obj is Argument<T> other && Equals( other );
    }

    public bool Equals( Argument<T> other )
    {
        return Value.Equals( other.Value );
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public int CompareTo( object? obj )
    {
        if ( obj is not Argument<T> other )
        {
            throw new ArgumentException();
        }

        return Comparer<T>.Default.Compare( Value, other.Value );
    }

    public static bool operator ==( Argument<T> a, Argument<T> b )
    {
        return a.Equals( b );
    }

    public static bool operator !=( Argument<T> a, Argument<T> b )
    {
        return !a.Equals( b );
    }

    public Argument<T> Copy() => new Argument<T>( Value );
}