using FiniteAutomatas.Domain.ValueObjects;

namespace FiniteAutomatas.Domain.Convertors.Convertors;

internal class CollapsedState
{
    public string Name { get; }
    public bool IsStart { get; set; }
    public bool IsEnd { get; set; }

    public readonly HashSet<State> States = new();

    public CollapsedState( State state )
    {
        Name = state.Name;
        IsStart = state.IsStart;
        IsEnd = state.IsEnd;
        States.Add( state );
    }

    public CollapsedState( HashSet<State> states, bool isStart, bool isEnd )
    {
        Name = String.Join( "_", states.Select( x => x.Name ).OrderBy( x => x ) );
        IsEnd = isEnd;
        IsStart = isStart;

        foreach ( State state in states )
        {
            IsEnd = IsEnd || state.IsEnd;
            IsStart = IsStart || state.IsStart;
            States.Add( state );
        }
    }

    public State ToState()
    {
        return new State( Name, IsStart, IsEnd );
    }

    public override bool Equals( object? obj )
    {
        return obj is CollapsedState other && Equals( other );
    }

    public bool Equals( CollapsedState? other )
    {
        return Name == other?.Name;
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}