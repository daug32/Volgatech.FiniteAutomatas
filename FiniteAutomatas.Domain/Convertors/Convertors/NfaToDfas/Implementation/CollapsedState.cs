using FiniteAutomatas.Domain.Models.ValueObjects;

namespace FiniteAutomatas.Domain.Convertors.Convertors.NfaToDfas.Implementation;

internal class CollapsedState
{
    public StateName Name { get; }
    public bool IsTerminateState => IsEnd || IsError;
    public bool IsError { get; set; }
    public bool IsStart { get; set; }
    public bool IsEnd { get; set; }

    public readonly HashSet<State> States = new();

    public CollapsedState( State state )
    {
        Name = state.Name;
        IsStart = state.IsStart;
        IsEnd = state.IsEnd;
        IsError = state.IsError;
        States.Add( state );
    }

    public CollapsedState( HashSet<State> states, bool isStart, bool isEnd )
    {
        Name = new StateName( String.Join( "_", states.Select( x => x.Name ).OrderBy( x => x ) ) );
        IsEnd = isEnd;
        IsStart = isStart;

        foreach ( State state in states )
        {
            IsError = IsError || state.IsError;
            IsEnd = IsEnd || state.IsEnd;
            IsStart = IsStart || state.IsStart;
            States.Add( state );
        }
    }

    public State ToState()
    {
        return new State( Name, IsStart, IsEnd, IsError );
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