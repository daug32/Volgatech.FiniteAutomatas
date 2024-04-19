using System.Reflection.Metadata;
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

    public IEnumerable<State> Move( State from, Argument argument )
    {
        var result = new List<State>();
        
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

                if ( transition.Argument == argument )
                {
                    result.Add( transition.To );
                    continue;
                }

                if ( transition.Argument == Argument.Epsilon )
                {
                    statesToProcess.Enqueue( transition.To );
                    continue;
                }
            }
        }

        return result;
    }

    public IEnumerable<State> EpsClosure( State from )
    {
        yield return from;
        
        var statesToProcess = new Queue<State>();
        statesToProcess.Enqueue( from );
        var processedStates = new HashSet<State>();

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

    public FiniteAutomata Copy() => new(
        alphabet: Alphabet.ToHashSet(),
        transitions: Transitions,
        allStates: AllStates.ToHashSet() );
}