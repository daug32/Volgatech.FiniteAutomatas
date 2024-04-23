using FiniteAutomatas.Domain.Convertors.Convertors.NfaToDfas.Implementation;
using FiniteAutomatas.Domain.Models.Automatas;
using FiniteAutomatas.Domain.Models.ValueObjects;

namespace FiniteAutomatas.Domain.Convertors.Convertors.NfaToDfas;

public class NfaToDfaConvertor : IAutomataConvertor<NonDeterminedFiniteAutomata, DeterminedFiniteAutomata>
{
    public DeterminedFiniteAutomata Convert( NonDeterminedFiniteAutomata automata )
    {
        // Result data
        var dfaTransitions = new HashSet<Transition>();
        var errorState = new State( "-1", isError: true );
        
        // For optimization
        var stateToEpsClosures = automata.EpsClosure().ToDictionary( x => x.Key, x => new EpsClosure( x.Key, x.Value ) );
        stateToEpsClosures[errorState] = new EpsClosure( errorState, new HashSet<State>() { errorState } );

        // Algorithm data
        var alphabet = automata.Alphabet.Where( x => x != Argument.Epsilon ).ToHashSet();
        var queue = new Queue<CollapsedState>();
        queue.Enqueue( new CollapsedState( automata.AllStates.First( x => x.IsStart ) ) );
        var processedStates = new HashSet<CollapsedState>();
        
        // Algorithm
        while ( queue.Any() )
        {
            CollapsedState fromState = queue.Dequeue();
            processedStates.Add( fromState );

            foreach ( State state in fromState.States )
            {
                EpsClosure epsClosure = stateToEpsClosures[state];
                fromState.IsError |= epsClosure.HasError;
                fromState.IsStart |= epsClosure.HasStart;
                fromState.IsEnd |= epsClosure.HasEnd;
            }

            foreach ( Argument argument in alphabet )
            {
                HashSet<State> achievableStates = fromState.States
                    .SelectMany( state => automata.Move( state, argument, stateToEpsClosures[state].Closures ) )
                    .ToHashSet();
                if ( !achievableStates.Any() )
                {
                    achievableStates.Add( errorState );
                }

                var toState = new CollapsedState(
                    achievableStates,
                    isStart: false,
                    isEnd: false );
                // If we didn't process the state yet
                if ( !processedStates.Contains( toState ) && !queue.Contains( toState ) )
                {
                    queue.Enqueue( toState );
                }

                dfaTransitions.Add( new Transition(
                    fromState.ToState(),
                    argument,
                    toState.ToState() ) );
            }
        }

        DeterminedFiniteAutomata dfa = BuildDfa( 
            dfaTransitions,
            processedStates.Select( x => x.ToState() ) );

        return dfa;
    }

    private static DeterminedFiniteAutomata BuildDfa( ICollection<Transition> transitions, IEnumerable<State> states )
    {
        var statesList = states.ToList();

        // oldName, newName
        var nameOverrides = new Dictionary<string, string>();
        for ( var i = 0; i < statesList.Count; i++ )
        {
            State state = statesList[i];

            string oldName = state.Name;
            string newName = i.ToString();

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

        return new DeterminedFiniteAutomata(
            alphabet,
            transitions,
            statesList );
    }
}