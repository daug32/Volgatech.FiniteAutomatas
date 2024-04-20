using FiniteAutomatas.Domain.ValueObjects;

namespace FiniteAutomatas.Domain.Automatas;

public class FiniteAutomata
{
    public readonly HashSet<Argument> Alphabet;
    public readonly HashSet<Transition> Transitions;
    public readonly HashSet<State> AllStates;

    public FiniteAutomata( 
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

    public FiniteAutomata( ICollection<Transition> transitions )
        : this(
            transitions.Select( x => x.Argument ).Distinct(),
            transitions, 
            transitions.SelectMany( x => new[] { x.From, x.To } ) )
    {
    }

    public HashSet<State> Move( State from, Argument argument, HashSet<State>? epsClosures = null )
    {
        epsClosures ??= EpsClosure( from ).ToHashSet();

        return Transitions
            .Where( transition => 
                transition.Argument.Equals( argument ) && 
                epsClosures.Contains( transition.From ) )
            .Select( transition => transition.To )
            .ToHashSet();
    }

    public Dictionary<State, HashSet<State>> EpsClosure()
    {
        var result = new Dictionary<State, HashSet<State>>();
        foreach ( State state in AllStates )
        {
            result[state] = EpsClosure( state );
        }

        return result;
    }

    public HashSet<State> EpsClosure( State from )
    {
        var closures = new HashSet<State>();
        closures.Add( from );
        
        var statesToProcess = new Queue<State>();
        var processedStates = new HashSet<State>();
        statesToProcess.Enqueue( from );

        while ( statesToProcess.Any() )
        {
            State state = statesToProcess.Dequeue();
            if ( processedStates.Contains( state ) )
            {
                continue;
            }

            processedStates.Add( state );
            
            var stateTransitions = new Queue<Transition>( Transitions.Where( transition => transition.From == state ) );

            while ( stateTransitions.Any() )
            {
                Transition transition = stateTransitions.Dequeue();
                if ( transition.Argument != Argument.Epsilon )
                {
                    continue;
                }

                closures.Add( transition.To );
                statesToProcess.Enqueue( transition.To );
            }
        }

        return closures;
    }
}