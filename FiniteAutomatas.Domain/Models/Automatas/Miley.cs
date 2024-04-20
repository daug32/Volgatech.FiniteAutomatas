using FiniteAutomatas.Domain.Models.ValueObjects;

namespace FiniteAutomatas.Domain.Models.Automatas;

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