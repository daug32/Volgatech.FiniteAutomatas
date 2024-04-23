using FiniteAutomatas.Domain.Models.ValueObjects;

namespace FiniteAutomatas.Domain.Models.Automatas;

public class DeterminedFiniteAutomata : IFiniteAutomata
{
    public HashSet<Argument> Alphabet { get; private set; }
    public HashSet<Transition> Transitions { get; private set; }
    public HashSet<State> AllStates { get; private set; }

    public DeterminedFiniteAutomata( 
        IEnumerable<Argument> alphabet,
        IEnumerable<Transition> transitions, 
        IEnumerable<State> allStates )
    {
        Alphabet = alphabet.ToHashSet();
        AllStates = allStates.ToHashSet();
        Transitions = transitions.ToHashSet();
        
        foreach ( Transition transition in Transitions )
        {
            if ( !AllStates.Contains( transition.From ) )
            {
                throw new ArgumentException( 
                    $"Some of the transitions has a state that is not presented in the {nameof( AllStates )}. " + 
                    $"Transition: {transition}. State: {transition.From}" );
            }

            if ( !AllStates.Contains( transition.To ) )
            {
                throw new ArgumentException( 
                    $"Some of the transitions has a state that is not presented in the {nameof( AllStates )}. " + 
                    $"Transition: {transition}. State: {transition.To}" );
            }

            if ( !Alphabet.Contains( transition.Argument ) )
            {
                throw new ArgumentException( 
                    $"Some of the transitions has an argument that is not presented in the {nameof( Alphabet )}. " + 
                    $"Transition: {transition}. Argument: {transition.Argument}" );
            }
        }
    }

    public HashSet<State> Move( State from, Argument argument ) 
    {
        return Transitions
            .Where( transition =>
                transition.Argument.Equals( argument ) &&
                transition.From.Equals( from ) )
            .Select( transition => transition.To.Name )
            .Select( stateName => AllStates.Single( x => x.Name == stateName ) )
            .ToHashSet();
    }
}