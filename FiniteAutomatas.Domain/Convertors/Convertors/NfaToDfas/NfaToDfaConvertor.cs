using FiniteAutomatas.Domain.Convertors.Convertors.Implementation;
using FiniteAutomatas.Domain.Convertors.Convertors.NfaToDfas.Implementation;
using FiniteAutomatas.Domain.Models.Automatas;
using FiniteAutomatas.Domain.Models.ValueObjects;

namespace FiniteAutomatas.Domain.Convertors.Convertors.NfaToDfas;

public class NfaToDfaConvertor : IAutomataConvertor<NonDeterminedFiniteAutomata, DeterminedFiniteAutomata>
{
    public DeterminedFiniteAutomata Convert( NonDeterminedFiniteAutomata automata )
    {
        // Result data
        var dfaTransitions = new List<(CollapsedState from, Argument argument, CollapsedState to)>();
        var errorState = new State( new StateId( -1 ), isError: true );
        
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

                dfaTransitions.Add( ( fromState, argument, toState ) );
            }
        }

        return BuildDfa( dfaTransitions );
    }

    private static DeterminedFiniteAutomata BuildDfa( List<(CollapsedState from, Argument argument, CollapsedState to)> rawTransitions )
    {
        var stateIdIncrementor = new StateIdIncrementer( new StateId( 0 ) );
        
        var states = new Dictionary<CollapsedState, State>();
        var transitions = new List<Transition>();
        var alphabet = new HashSet<Argument>();
        foreach ( (CollapsedState from, Argument argument, CollapsedState to) rawTransition in rawTransitions )
        {
            State fromState; 
            if ( states.ContainsKey( rawTransition.from ) )
            {
                fromState = states[rawTransition.from];
            }
            else
            {
                fromState = rawTransition.from.ToState( stateIdIncrementor.Next() );
                states[rawTransition.from] = fromState;
            }

            State toState; 
            if ( states.ContainsKey( rawTransition.to ) )
            {
                toState = states[rawTransition.to];
            }
            else
            {
                toState = rawTransition.to.ToState( stateIdIncrementor.Next() );
                states[rawTransition.to] = toState;
            }

            alphabet.Add( rawTransition.argument );
            
            transitions.Add( new Transition( fromState, rawTransition.argument, toState ) );
        }

        return new DeterminedFiniteAutomata(
            alphabet,
            transitions,
            states.Values );
    }
}