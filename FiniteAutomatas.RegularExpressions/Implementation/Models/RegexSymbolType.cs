namespace FiniteAutomatas.RegularExpressions.Implementation.Models;

internal enum RegexSymbolType
{
    // a, 3, -...
    Symbol = 0,
    
    // ab
    And,

    // a | b
    Or,

    // a* 
    ZeroOrMore,

    // a+
    OneOrMore,

    // (
    OpenBrace,

    // )
    CloseBrace
}