using FiniteAutomatas.Grammars.LeftRoRightOne.Models.ValueObjects;
using LinqExtensions;

namespace FiniteAutomatas.Grammars.LeftToRightOne.Console.Parsers.Implementation;

public class RuleNameParser
{
    public RuleName? ParseFromLine( string line, int ruleDeclarationIndex, out int lastLineReadSymbolIndex )
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

        if ( !rawRuleName.Any() )
        {
            return null;
        }

        return new RuleName( rawRuleName.ConvertToString() );
    }
}