using FiniteAutomatas.Grammar.LeftToRightOne.Console.Models;

namespace FiniteAutomatas.Grammar.LeftToRightOne.Console.Parsers.Implementation;

public class GrammarRuleNameParser
{
    public GrammarRuleName? Parse( string line, int ruleDeclarationIndex, out int lastLineReadSymbolIndex )
    {
        var rawRuleName = new List<char>( ruleDeclarationIndex + 1 );
        for ( lastLineReadSymbolIndex = 0; lastLineReadSymbolIndex < ruleDeclarationIndex; lastLineReadSymbolIndex++ )
        {
            char symbol = line[lastLineReadSymbolIndex];

            // Whitespaces do not count
            if ( Char.IsWhiteSpace( symbol ) )
            {
                continue;
            }

            rawRuleName.Add( symbol );
        }

        return new GrammarRuleName( new string( rawRuleName.ToArray() ) );
    }
}