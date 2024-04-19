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

        Transitions = transitions
            .All( x =>
                AllStates.Contains( x.From ) && 
                AllStates.Contains( x.To ) && 
                Alphabet.Contains( x.Argument ) )
            ? transitions.ToHashSet()
            : throw new ArgumentException( "Not all From and To states of the transitions are presented in the AllStates" );
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

    public IEnumerable<State> EpsClosure( State from )
    {
        yield return from;
        
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

                yield return transition.To;
                statesToProcess.Enqueue( transition.To );
            }
        }
    }
}