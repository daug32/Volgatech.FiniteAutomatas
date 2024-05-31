using FiniteAutomatas.Domain.Models.ValueObjects;

namespace FiniteAutomatas.Domain.Models.Automatas.Extensions;

public static class FiniteAutomataExtensions
{
    public static FiniteAutomataRunResult Run<T>( this IAutomata<T> automata, IEnumerable<Argument<T>> arguments )
    {
        State currentState = automata.AllStates.First( x => x.IsStart );

        foreach ( Argument<T> argument in arguments )
        {
            HashSet<StateId> states = automata.Move( currentState.Id, argument ); 
            if ( states.Count > 1 )
            {
                throw new ArgumentException( "Can only process DFA automatas" );
            }

            if ( states.Count == 0 )
            {
                return FiniteAutomataRunResult.FinishedOnError;
            }

            currentState = automata.GetState( states.Single() );
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
}
