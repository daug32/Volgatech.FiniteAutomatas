using Grammars.Common.Grammars.ValueObjects;
using Grammars.Common.Grammars.ValueObjects.RuleDefinitions;
using Grammars.Common.Grammars.ValueObjects.Symbols;
using LinqExtensions;

namespace Grammar.Parsers.Implementation.Implementation;

public class RuleDefinitionParser
{
    private readonly ParsingSettings _settings;
    
    // "BEGIN <exp> END." -> { BEGIN, <exp>, END, . }
    public RuleDefinitionParser( ParsingSettings settings )
    {
        _settings = settings;
    }

    public RuleDefinition Parse( List<char> symbols )
    {
        var items = new List<RuleSymbol>();

        bool isNonTerminalSymbolParsing = false;
        List<char> lastWord = new();
        for ( var i = 0; i < symbols.Count; i++ )
        {
            char symbol = symbols[i];

            if ( symbol == _settings.EndSymbol )
            {
                if ( isNonTerminalSymbolParsing )
                {
                    throw new ArgumentException( "End symbol can not be used in the non terminal symbol name" );
                }
                
                if ( lastWord.Any() )
                {
                    items.Add(
                        RuleSymbol.TerminalSymbol(
                            TerminalSymbol.Word(
                                lastWord.ConvertToString() ) ) );
                    lastWord.Clear();
                }
                
                items.Add( RuleSymbol.TerminalSymbol( TerminalSymbol.End() ) );
                
                continue;
            }

            if ( symbol == _settings.EmptySymbol )
            {
                if ( isNonTerminalSymbolParsing )
                {
                    throw new ArgumentException( "Empty symbol can not be used in the non terminal symbol name" );
                }
                
                if ( lastWord.Any() )
                {
                    items.Add(
                        RuleSymbol.TerminalSymbol(
                            TerminalSymbol.Word(
                                lastWord.ConvertToString() ) ) );
                    lastWord.Clear();
                }
                
                items.Add( RuleSymbol.TerminalSymbol( TerminalSymbol.EmptySymbol() ) );
                
                continue;
            }

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
                    items.Add(
                        RuleSymbol.TerminalSymbol(
                            TerminalSymbol.Word( lastWord.ConvertToString() ) ) );
                    lastWord.Clear();
                }

                // Commit whitespace symbol
                items.Add( 
                    RuleSymbol.TerminalSymbol(
                        TerminalSymbol.WhiteSpace() ) );
                continue;
            }

            // We are starting to parse a non terminal symbol
            if ( symbol == _settings.RuleNameOpenSymbol )
            {
                // We already started to parse a non terminal symbol, throw exception
                if ( isNonTerminalSymbolParsing )
                {
                    throw new ArgumentException( "Rule name is already parsing" );
                }

                // If we had any terminal symbols before, commit them
                if ( lastWord.Any() )
                {
                    items.Add( 
                        RuleSymbol.TerminalSymbol(
                            TerminalSymbol.Word( lastWord.ConvertToString() ) ) );
                    lastWord.Clear();
                }

                isNonTerminalSymbolParsing = true;

                continue;
            }

            // We are finishing to parse a non terminal symbol
            if ( symbol == _settings.RuleNameCloseSymbol )
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
                items.Add( 
                    RuleSymbol.NonTerminalSymbol(
                        new RuleName( 
                            lastWord.ConvertToString() ) ) );
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
            items.Add( 
                RuleSymbol.TerminalSymbol(
                    TerminalSymbol.Word( lastWord.ConvertToString() ) ) );
            lastWord.Clear();
        }

        // Similar to trim function 
        // If last symbols is a whitespace, remove it
        // "a b | a c" -> { {a, b}, {a, c} }
        if ( items.Last().Type == RuleSymbolType.TerminalSymbol && 
             items.Last().Symbol!.Type == TerminalSymbolType.WhiteSpace )
        {
            items.RemoveAt( items.Count - 1 );
        }
        
        return new RuleDefinition( items );
    }
}