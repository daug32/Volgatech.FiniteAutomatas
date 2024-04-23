using FiniteAutomatas.Domain.Models.ValueObjects;
using FluentAssertions;

namespace FiniteAutomatas.Domain.Convertors.Convertors.Implementation;

public class StateIdIncrementer
{
    private int _lastId = 0;

    public StateIdIncrementer( IEnumerable<State> states )
    {
        _lastId = Int32.Parse( states.MaxBy( x => x.Id.Value ).ThrowIfNull().Id.Value );
    }

    public StateIdIncrementer( IEnumerable<StateId> stateIds )
    {
        _lastId = stateIds.Select( x => Int32.Parse( x.Value ) ).Max();
    }

    public StateIdIncrementer( StateId maxId )
    {
        _lastId = Int32.Parse( maxId.Value );
    }

    public StateId Next() => new( ( ++_lastId ).ToString() );

    public StateId Convert( IEnumerable<State> states )
    {
        var newStateId = 0;
        foreach ( State state in states )
        {
            newStateId -= Int32.Parse( state.Id.Value );
        }

        return new StateId( newStateId );
    }
}