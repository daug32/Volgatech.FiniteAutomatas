using FiniteAutomatas.Domain.Models.Automatas;
using FiniteAutomatas.Domain.Models.Automatas.Extensions;
using FiniteAutomatas.Domain.Models.ValueObjects;

namespace FiniteAutomatas.RegularExpressions;

public static class FiniteAutomataExtensions
{
    public static FiniteAutomataRunResult RunRegex( this DeterminedFiniteAutomata<char> automata, string regex )
    {
        return automata.Run( regex.Select( x => new Argument<char>( x ) ) );
    }
}