using FiniteAutomatas.Domain.Convertors.Convertors.NfaToDfas.Implementation;
using FiniteAutomatas.Domain.Models.Automatas;
using FiniteAutomatas.Domain.Models.ValueObjects;
using FiniteAutomatas.Domain.Models.ValueObjects.Implementation;

namespace FiniteAutomatas.Domain.Convertors.Convertors.NfaToDfas;

public class NfaToDfaConvertor<T> : IAutomataConvertor<NonDeterminedFiniteAutomata<T>, T, DeterminedFiniteAutomata<T>>
{
    public DeterminedFiniteAutomata<T> Convert( NonDeterminedFiniteAutomata<T> finiteAutomata )
    {
        // Result data
        var dfaTransitions = new List<(CollapsedState from, Argument<T> argument, CollapsedState to)>();
        var errorState = new State( new StateId( -1 ), isError: true );
        
        // For optimization
        Dictionary<State, EpsClosure> stateToEpsClosures = finiteAutomata
            .EpsClosure()
            .ToDictionary( 
                x => finiteAutomata.GetState( x.Key),
                x => new EpsClosure( finiteAutomata.GetStates( x.Value ) ) );
        stateToEpsClosures[errorState] = new EpsClosure( new HashSet<State>() { errorState } );

        // Algorithm data
        var alphabet = finiteAutomata.Alphabet.Where( x => x != Argument<T>.Epsilon ).ToHashSet();
        var queue = new Queue<CollapsedState>();
        queue.Enqueue( new CollapsedState( finiteAutomata.AllStates.First( x => x.IsStart ) ) );
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

            foreach ( Argument<T> argument in alphabet )
            {
                var achievableStatesIds = fromState.States
                    .SelectMany( state => finiteAutomata.Move(
                        state.Id,
                        argument,
                        stateToEpsClosures[state].Closures ) )
                    .ToHashSet();
                HashSet<State> achievableStates = finiteAutomata.GetStates( achievableStatesIds ).ToHashSet();
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

        DeterminedFiniteAutomata<T> dfa = BuildDfa( dfaTransitions );
        return dfa;
    }

    private static DeterminedFiniteAutomata<T> BuildDfa( List<(CollapsedState from, Argument<T> argument, CollapsedState to)> rawTransitions )
    {
        var stateIdIncrementor = new StateIdIncrementer( new StateId( 0 ) );
        
        var states = new Dictionary<CollapsedState, State>();
        var transitions = new List<Transition<T>>();
        var alphabet = new HashSet<Argument<T>>();
        foreach ( (CollapsedState from, Argument<T> argument, CollapsedState to) rawTransition in rawTransitions )
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
            
            transitions.Add( new Transition<T>( fromState.Id, rawTransition.argument, toState.Id ) );
        }

        return new DeterminedFiniteAutomata<T>(
            alphabet,
            transitions,
            states.Values );
    }
}