namespace FiniteAutomatas.Domain.Models.ValueObjects.Implementation;

internal class StateIdIncrementer
{
    private int _lastId;

    public StateIdIncrementer( IEnumerable<State> states )
    {
        int? lastId = null;
        foreach ( State state in states )
        {
            if ( state.Id.Value > lastId )
            {
                lastId = state.Id.Value;
            }
        }

        _lastId = lastId ?? 0;
    }

    public StateIdIncrementer( StateId maxId )
    {
        _lastId = maxId.Value;
    }

    public StateId Next() => new( ++_lastId );
}