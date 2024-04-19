using FiniteAutomatas.Domain.Automatas;
using FiniteAutomatas.Domain.ValueObjects;

namespace FiniteAutomatas.Domain.Convertors.Convertors;

public class EClosure
{
    public readonly State From;
    public readonly HashSet<State> Closures;
    public readonly bool HasEnd;
    public readonly bool HasStart;

    public EClosure( FiniteAutomata automata, State from )
        : this ( from, automata.EpsClosure( from ).ToHashSet() )
    {
    }

    public EClosure( State from, HashSet<State> closures )
    {
        From = from;
        Closures = closures;
        
        foreach ( State closure in closures )
        {
            HasEnd |= closure.IsEnd;
            HasStart |= closure.IsStart;
        }
    }
}

public class NfaToDfaConvertor : IAutomataConvertor<FiniteAutomata>
{
    public FiniteAutomata Convert( FiniteAutomata automata )
    {
        var dfaTransitions = new HashSet<Transition>();
        var dfaStart = new CollapsedState( automata.AllStates.First( x => x.IsStart ) );
        
        IEnumerable<Argument> alphabet = automata.Alphabet
            .Where( x => x != Argument.Epsilon )
            .ToHashSet();
        Dictionary<State, EClosure> stateToEpsClosures = automata.AllStates
            .Select( x => new EClosure( automata, x ) )
            .ToDictionary( x => x.From, x => x );
        
        var queue = new Queue<CollapsedState>();
        queue.Enqueue( dfaStart );
        var processedStates = new HashSet<CollapsedState>();
        
        while ( queue.Any() )
        {
            CollapsedState fromState = queue.Dequeue();
            processedStates.Add( fromState );

            foreach ( State state in fromState.States )
            {
                EClosure epsClosure = stateToEpsClosures[state];
                fromState.IsStart |= epsClosure.HasStart;
                fromState.IsEnd |= epsClosure.HasEnd;
            }

            foreach ( Argument argument in alphabet )
            {
                HashSet<State> achievableStates = fromState.States
                    .SelectMany( state => automata.Move( state, argument ) )
                    .ToHashSet();
                if ( !achievableStates.Any() )
                {
                    continue;
                }

                var toState = new CollapsedState(
                    achievableStates,
                    isStart: false,
                    isEnd: false );
                // If we didn't process the state yet
                if ( !processedStates.Contains( toState ) )
                {
                    queue.Enqueue( toState );
                }

                dfaTransitions.Add( new Transition(
                    fromState.ToState(),
                    argument,
                    toState.ToState() ) );
            }
        }

        FiniteAutomata dfa = BuildDfa( 
            dfaTransitions,
            processedStates.Select( x => x.ToState() ) );

        return dfa;
    }

    private static FiniteAutomata BuildDfa( ICollection<Transition> transitions, IEnumerable<State> states )
    {
        // oldName, newName
        var statesList = states.ToList();

        var nameOverrides = new Dictionary<string, string>();
        for ( var i = 0; i < statesList.Count; i++ )
        {
            State state = statesList[i];

            string oldName = state.Name;
            string newName = $"S{i}";

            nameOverrides.Add( oldName, newName );
            state.Name = newName;
        }

        var alphabet = new HashSet<Argument>();
        foreach ( Transition transition in transitions )
        {
            transition.From.Name = nameOverrides[transition.From.Name];
            transition.To.Name = nameOverrides[transition.To.Name];
            alphabet.Add( transition.Argument );
        }

        return new FiniteAutomata(
            alphabet,
            transitions,
            statesList );
    }
}