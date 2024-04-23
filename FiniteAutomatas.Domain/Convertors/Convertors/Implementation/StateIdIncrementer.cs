﻿using FiniteAutomatas.Domain.Models.ValueObjects;

namespace FiniteAutomatas.Domain.Convertors.Convertors.Implementation;

public class StateIdIncrementer
{
    private int _lastId = 0;

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