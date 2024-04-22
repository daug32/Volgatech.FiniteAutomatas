using FiniteAutomatas.Domain.Models.ValueObjects;

namespace FiniteAutomatas.Domain.Models.Automatas.Extensions;

public static class FiniteAutomataExtensions
{
    public static FiniteAutomataRunResult RunForAllSymbols( this FiniteAutomata automata, IEnumerable<Argument> arguments )
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
}

public enum FiniteAutomataRunResult
{
    Unknown,
    FinishedOnSuccess,
    FinishedOnIntermediate,
    FinishedOnError,
}

public static class FiniteAutomataRunResultExtensions
{
    public static bool IsSuccess( this FiniteAutomataRunResult result ) => result == FiniteAutomataRunResult.FinishedOnSuccess;
}