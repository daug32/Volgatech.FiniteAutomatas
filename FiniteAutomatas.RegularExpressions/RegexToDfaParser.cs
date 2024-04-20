using FiniteAutomatas.Domain.Convertors;
using FiniteAutomatas.Domain.Convertors.Convertors;
using FiniteAutomatas.Domain.Models.Automatas;

namespace FiniteAutomatas.RegularExpressions;

public class RegexToDfaParser 
{
    private readonly RegexToNfaParser _regexToNfaParser = new();

    public FiniteAutomata Parse( string regex )
    {
        return _regexToNfaParser
            .Parse( regex )
            .Convert( new NfaToDfaConvertor() )
            .Convert( new SetErrorStateOnEmptyTransitionsConvertor() );
    }
}