using FiniteAutomatas.Domain.Models.ValueObjects;
using FiniteAutomatas.Domain.Models.ValueObjects.Implementation;

namespace FiniteAutomatas.Domain.Models.Automatas.Extensions;

public static class FiniteAutomataExtensions
{
    public static FiniteAutomataRunResult Run( this IFiniteAutomata automata, IEnumerable<Argument> arguments )
    {
        State currentState = automata.AllStates.First( x => x.IsStart );

        foreach ( Argument argument in arguments )
        {
            HashSet<State> states = automata.Move( currentState, argument ); 
            if ( states.Count > 1 )
            {
                throw new ArgumentException( "Can only process DFA automatas" );
            }

            if ( states.Count == 0 )
            {
                return FiniteAutomataRunResult.FinishedOnError;
            }

            currentState = states.Single();
        }

        if ( currentState.IsEnd )
        {
            return FiniteAutomataRunResult.FinishedOnSuccess;
        }

        if ( currentState.IsError )
        {
            return FiniteAutomataRunResult.FinishedOnError;
        }

        return FiniteAutomataRunResult.FinishedOnIntermediate;
    }

    private static void StandardizeIds( this IFiniteAutomata automata )
    {
        var states = automata.AllStates.ToList();
        states.Sort( ( a, b ) =>
        {
            if ( a.IsStart )
            {
                return -1;
            }

            if ( b.IsStart )
            {
                return 1;
            }

            return a.Id.CompareTo( b.Id );
        } );

        var stateOldIdToNewId = new Dictionary<StateId, StateId>();
        
        var stateIdIncrementor = new StateIdIncrementer( new StateId( 0 ) );
        foreach ( State state in states )
        {
            stateOldIdToNewId[state.Id] = stateIdIncrementor.Next();
        }

        foreach ( Transition transition in automata.Transitions )
        {
            transition.From.Id = stateOldIdToNewId[transition.From.Id];
            transition.To.Id = stateOldIdToNewId[transition.To.Id];
        }
    }
}
