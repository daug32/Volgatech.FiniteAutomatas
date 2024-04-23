using FiniteAutomatas.Domain.Models.ValueObjects;

namespace FiniteAutomatas.Domain.Convertors.Convertors.NfaToDfas.Implementation;

internal class EpsClosure
{
    public readonly State From;
    public readonly HashSet<State> Closures;
    public readonly bool HasError;
    public readonly bool HasStart;
    public readonly bool HasEnd;

    public EpsClosure( State from, HashSet<State> closures )
    {
        From = from;
        Closures = closures;
        
        foreach ( State closure in closures )
        {
            HasEnd |= closure.IsEnd;
            HasError |= closure.IsTerminateState;
            HasStart |= closure.IsStart;
        }
    }
}