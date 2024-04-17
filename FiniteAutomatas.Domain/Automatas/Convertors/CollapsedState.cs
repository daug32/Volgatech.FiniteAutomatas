using FiniteAutomatas.Domain.ValueObjects;

namespace FiniteAutomatas.Domain.Automatas.Convertors;

public class CollapsedState
{
    public string Name { get; set; }
    
    public readonly bool IsStart;
    public readonly bool IsEnd;
    
    public readonly HashSet<State> States = new();

    public CollapsedState( State state )
    {
        Name = state.Name;
        IsStart = state.IsStart;
        IsEnd = state.IsEnd;
        States.Add( state );
    }

    public CollapsedState( HashSet<State> states )
    {
        Name = String.Join( "_", states.Select( x => x.Name ).OrderBy( x => x ) );
        IsEnd = false;
        IsStart = false;

        foreach ( State state in states )
        {
            IsStart = IsStart || state.IsStart;
            IsEnd = IsEnd || state.IsEnd;
            States.Add( state );
        }
    }

    public State ToState() => new( name: Name, isStart: IsStart, isEnd: IsEnd );

    public override bool Equals( object? obj ) => obj is CollapsedState other && Equals( other );

    public bool Equals( CollapsedState? other ) => Name == other?.Name;

    public override int GetHashCode() => Name.GetHashCode();
}