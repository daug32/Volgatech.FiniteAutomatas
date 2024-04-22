using FiniteAutomatas.Domain.Models.ValueObjects;

namespace FiniteAutomatas.Domain.Convertors.Convertors.Minimization.Implementation.Models;

public class MinimizationGroup
{
    private readonly Dictionary<string, State> _states = new();

    public MinimizationGroup()
    {
    }

    public MinimizationGroup( State state )
    {
        _states.Add( state.Name, state );
    }

    public void Add( State state )
    {
        if ( _states.ContainsKey( state.Name ) )
        {
            throw new InvalidOperationException(
                $"Can't add state to the {nameof( MinimizationGroup )} because it already contains the given state. StateName: {state.Name}" );
        }

        _states.Add( state.Name, state );
    }

    public void Remove( string stateName )
    {
        if ( !_states.Remove( stateName ) )
        {
            throw new InvalidOperationException(
                $"Can't remove state from {nameof( MinimizationGroup )} because it doesn't contain the given state. StateName: {stateName}" );
        }
    }

    public bool Contains( string stateName ) => _states.ContainsKey( stateName );

    public int Count => _states.Count;

    public bool Any() => _states.Any();

    public bool Any( Func<State, bool> predicate ) => _states.Values.Any( predicate );

    public List<State> GetStates() => _states.Values.ToList();
}