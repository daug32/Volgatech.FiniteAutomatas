using FiniteAutomatas.Domain.Models.Automatas;
using FiniteAutomatas.Domain.Models.ValueObjects;

namespace FiniteAutomatas.RegularExpressions.Implementation.Utils;

internal class FiniteAutomataCreator
{
    public static FiniteAutomata ForSymbol( Argument argument )
    {
        var start = new State( "0", isStart: true );
        var end = new State( "1", isEnd: true );

        return new FiniteAutomata(
            alphabet: new[] { argument },
            transitions: new[]
            {
                new Transition(
                    from: start,
                    to: end,
                    argument: argument )
            },
            allStates: new[] { start, end } );
    }
}