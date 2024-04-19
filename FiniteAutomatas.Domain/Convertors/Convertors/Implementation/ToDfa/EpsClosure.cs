﻿using FiniteAutomatas.Domain.ValueObjects;

namespace FiniteAutomatas.Domain.Convertors.Convertors.Implementation.ToDfa;

internal class EpsClosure
{
    public readonly State From;
    public readonly HashSet<State> Closures;
    public readonly bool HasEnd;
    public readonly bool HasStart;

    public EpsClosure( State from, HashSet<State> closures )
    {
        From = from;
        Closures = closures;
        
        foreach ( State closure in closures )
        {
            HasEnd |= closure.IsEnd;
            HasStart |= closure.IsStart;
        }
    }
}