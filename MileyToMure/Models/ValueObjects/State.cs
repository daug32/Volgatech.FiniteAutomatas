namespace MileyToMure.Models.ValueObjects;

public class State : IComparable
{
    public readonly string Name;

    public State(string name)
    {
        Name = name;
    }

    public override bool Equals(object? obj)
    {
        return obj is State other && Equals(other);
    }

    public bool Equals(State other)
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

    public int CompareTo(object? obj)
    {
        if (obj is not State other)
        {
            throw new ArgumentException();
        }

        return String.CompareOrdinal(Name, other.Name);
    }
}