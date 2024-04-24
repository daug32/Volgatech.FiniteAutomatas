using FiniteAutomatas.Domain.Models.ValueObjects;

namespace FiniteAutomatas.Domain.Convertors.Convertors.Minimization.Implementation.Models;

internal class MinimizationGroup
{
    private readonly Dictionary<StateId, State> _states = new();

    public MinimizationGroup()
    {
    }

    public MinimizationGroup( State state )
    {
        _states.Add( state.Id, state );
    }
    
    public MinimizationGroup( IEnumerable<State> states )
    {
        foreach ( State state in states )
        {
            _states.Add( state.Id, state );
        }
    }

    public void Add( State state )
    {
        if ( _states.ContainsKey( state.Id ) )
        {
            throw new InvalidOperationException(
                $"Can't add state to the {nameof( MinimizationGroup )} because it already contains the given state. StateName: {state.Id}" );
        }

        _states.Add( state.Id, state );
    }

    public void Remove( StateId stateId )
    {
        if ( !_states.Remove( stateId ) )
        {
            throw new InvalidOperationException(
                $"Can't remove state from {nameof( MinimizationGroup )} because it doesn't contain the given state. StateName: {stateId}" );
        }
    }

    public bool Contains( StateId stateId ) => _states.ContainsKey( stateId );

    public int Count => _states.Count;

    public bool Any() => _states.Any();

    public bool Any( Func<State, bool> predicate ) => _states.Values.Any( predicate );

    public List<State> GetStates() => _states.Values.ToList();

    public MinimizationGroup Copy() => new( _states.Values );
}