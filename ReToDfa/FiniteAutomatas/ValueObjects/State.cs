namespace ReToDfa.FiniteAutomatas.ValueObjects;

public class State
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

    public override string ToString() => Name;

    public override bool Equals( object? obj ) => obj is State other && Equals( other );

    public bool Equals( State? other ) => Name == other?.Name;

    public override int GetHashCode() => Name.GetHashCode();
}