using FiniteAutomatas.Domain.Models.ValueObjects;

namespace FiniteAutomatas.Domain.Models.Automatas.Extensions;

public static class FiniteAutomataExtensions
{
    public static bool RunForAllSymbols( this FiniteAutomata automata, IEnumerable<Argument> arguments )
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
                return false;
            }

            currentState = states.Single();
        }

        return currentState.IsEnd;
    }
}