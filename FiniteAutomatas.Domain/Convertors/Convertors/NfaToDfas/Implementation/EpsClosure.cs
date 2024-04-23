using FiniteAutomatas.Domain.Models.ValueObjects;

namespace FiniteAutomatas.Domain.Convertors.Convertors.NfaToDfas.Implementation;

internal class EpsClosure
{
    public readonly HashSet<State> Closures;
    public readonly bool HasError;
    public readonly bool HasStart;
    public readonly bool HasEnd;

    public EpsClosure( HashSet<State> closures )
    {
        Closures = closures;
        
        foreach ( State closure in closures )
        {
            HasEnd |= closure.IsEnd;
            HasError |= closure.IsError;
            HasStart |= closure.IsStart;
        }
    }
}