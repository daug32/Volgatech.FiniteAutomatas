using Grammars.Grammars.LeftRoRightOne.Models.ValueObjects;
using LinqExtensions;

namespace Grammars.LL.Console.Parsers.Implementation;

public class RuleValueParser
{
    public RuleValue Parse( List<char> symbols )
    {
        var items = new List<RuleValueItem>();

        bool isNonTerminalSymbolParsing = false;
        List<char> lastWord = new();
        foreach ( char symbol in symbols )
        {
            if ( Char.IsWhiteSpace( symbol ) )
            {
                // Non terminal symbol can have spaces in their names
                if ( isNonTerminalSymbolParsing )
                {
                    lastWord.Add( ' ' );
                    continue;
                }

                // If we already parsed a terminal word, commit
                if ( lastWord.Any() )
                {
                    items.Add( RuleValueItem.TerminalSymbol( lastWord.ConvertToString() ) );
                    lastWord.Clear();
                }

                // Commit whitespace symbol
                items.Add( RuleValueItem.WhiteSpace() );
                continue;
            }

            // We are starting to parse a non terminal symbol
            if ( symbol == RuleName.RuleNameOpenSymbol )
            {
                // We already started to parse a non terminal symbol, throw exception
                if ( isNonTerminalSymbolParsing )
                {
                    throw new ArgumentException( "Rule name is already parsing" );
                }

                // If we had any terminal symbols before, commit them
                if ( lastWord.Any() )
                {
                    items.Add( RuleValueItem.TerminalSymbol( lastWord.ConvertToString() ) );
                    lastWord.Clear();
                }
                
                isNonTerminalSymbolParsing = true;
                
                continue;
            }

            // We are finishing to parse a non terminal symbol
            if ( symbol == RuleName.RuleNameCloseSymbol )
            {
                // We didn't start to parse a non terminal, throw exception
                if ( !isNonTerminalSymbolParsing )
                {
                    throw new ArgumentException( "Rule name is not parsing" );
                }

                // Non terminal symbol must have a non empty identifier
                if ( !lastWord.Any() )
                {
                    throw new ArgumentException( "Non terminal symbol can not be empty" );
                } 

                // Commit non terminal
                items.Add( RuleValueItem.NonTerminalSymbol( new RuleName( lastWord.ConvertToString() ) ) );
                lastWord.Clear();

                isNonTerminalSymbolParsing = false;
                
                continue;
            }

            // This is a regular symbol, add it to the last word
            lastWord.Add( symbol );
        }

        if ( isNonTerminalSymbolParsing )
        {
            throw new ArgumentException( "Started to parse a non terminal symbol but didn't find an end" );
        }
        
        if ( lastWord.Any() )
        {
            items.Add( RuleValueItem.TerminalSymbol( lastWord.ConvertToString() ) );
            lastWord.Clear();
        }
        
        return new RuleValue( items );
    }
}