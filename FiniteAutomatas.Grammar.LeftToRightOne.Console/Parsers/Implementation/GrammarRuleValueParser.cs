using FiniteAutomatas.Grammar.LeftToRightOne.Console.Models;

namespace FiniteAutomatas.Grammar.LeftToRightOne.Console.Parsers.Implementation;

public class GrammarRuleValueParser
{
    public GrammarRuleValue Parse( List<char> symbols )
    {
        string ruleValue = new string( symbols.ToArray() );
        return new GrammarRuleValue( ruleValue );
    }
}