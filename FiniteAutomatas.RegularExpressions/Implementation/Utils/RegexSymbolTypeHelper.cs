namespace FiniteAutomatas.RegularExpressions.Implementation.Utils;

internal static class RegexSymbolTypeHelper
{
    public static readonly HashSet<char> SpecialSymbols = new( "()|+*" );
}