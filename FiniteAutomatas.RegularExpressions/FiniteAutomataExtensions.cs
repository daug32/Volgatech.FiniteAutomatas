using FiniteAutomatas.Domain.Models.Automatas;
using FiniteAutomatas.Domain.Models.Automatas.Extensions;
using FiniteAutomatas.Domain.Models.ValueObjects;

namespace FiniteAutomatas.RegularExpressions;

public static class FiniteAutomataExtensions
{
    public static FiniteAutomataRunResult RunRegex( this DeterminedFiniteAutomata<char> finiteAutomata, string regex )
    {
        return finiteAutomata.Run( regex.Select( x => new Argument<char>( x ) ) );
    }
}