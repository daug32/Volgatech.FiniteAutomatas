using FiniteAutomatas.Domain.ValueObjects;

namespace FiniteAutomatas.Domain.Automatas;

public class Miley : FiniteAutomata
{
    public Miley( ICollection<Transition> transitions )
        : base( transitions )
    {
        if ( Transitions.Any( x => x.AdditionalData == null ) )
        {
            throw new ArgumentException( nameof( Transitions ) );
        }
    }
}