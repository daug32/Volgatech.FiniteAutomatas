using FiniteAutomatas.Domain.Models.ValueObjects;

namespace FiniteAutomatas.Domain.Convertors.Convertors.NfaToDfas.Implementation;

internal class EpsClosure
{
    public readonly HashSet<StateId> Closures;
    public readonly bool HasError;
    public readonly bool HasStart;
    public readonly bool HasEnd;

    public EpsClosure( HashSet<State> closures )
    {
        Closures = new HashSet<StateId>();
        
        foreach ( State closure in closures )
        {
            Closures.Add( closure.Id );
            
            HasEnd |= closure.IsEnd;
            HasError |= closure.IsError;
            HasStart |= closure.IsStart;
        }
    }
}