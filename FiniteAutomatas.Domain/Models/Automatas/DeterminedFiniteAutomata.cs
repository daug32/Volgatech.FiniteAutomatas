using FiniteAutomatas.Domain.Models.ValueObjects;

namespace FiniteAutomatas.Domain.Models.Automatas;

public class DeterminedFiniteAutomata : IFiniteAutomata
{
    public HashSet<Argument> Alphabet { get; private set; }
    public IReadOnlySet<Transition> Transitions { get; private set; }
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
            if ( !AllStates.Any( x => x.Id == transition.From ) )
            {
                throw new ArgumentException( 
                    $"Some of the transitions has a state that is not presented in the {nameof( AllStates )}. " + 
                    $"Transition: {transition}. State: {transition.From}" );
            }

            if ( !AllStates.Any( x => x.Id == transition.To ) )
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

    public HashSet<State> GetStates( HashSet<StateId> stateIds )
    {
        var result = new HashSet<State>();
        foreach ( State state in AllStates )
        {
            if ( stateIds.Contains( state.Id ) )
            {
                result.Add( state );
            }
        }

        if ( result.Count != stateIds.Count )
        {
            throw new ArgumentException( "Can't find states with given ids" );
        }

        return result;
    }

    public State GetState( StateId stateId )
    {
        return AllStates.Single( x => x.Id == stateId );
    }

    public HashSet<StateId> Move( StateId from, Argument argument ) 
    {
        return Transitions
            .Where( transition =>
                transition.Argument == argument &&
                transition.From == from )
            .Select( transition => transition.To )
            .ToHashSet();
    }
}