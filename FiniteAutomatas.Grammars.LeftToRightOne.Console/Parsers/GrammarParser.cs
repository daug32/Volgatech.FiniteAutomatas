using System.Diagnostics;
using FiniteAutomatas.Grammars.LeftRoRightOne.Models;
using FiniteAutomatas.Grammars.LeftRoRightOne.Models.ValueObjects;
using FiniteAutomatas.Grammars.LeftToRightOne.Console.Parsers.Implementation;

namespace FiniteAutomatas.Grammars.LeftToRightOne.Console.Parsers;

public class GrammarParser
{
    private const char CommentIdentifier = '#';
    private const string RuleNameSeparator = "->";
    private const char RuleValueSeparator = ',';

    private readonly RuleNameParser _ruleNameParser = new();
    private readonly RuleValueParser _ruleValueParser = new();

    public Grammar ParseFromFile( string fullFilePath )
    {
        if ( !File.Exists( fullFilePath ) )
        {
            throw new ArgumentException( $"File was not found. FilePath: {Path.GetFileName( fullFilePath )}" );
        }
        
        using var reader = new StreamReader( fullFilePath );
        
        var rules = new List<GrammarRule>();

        int lineNumber = 0;
        GrammarRule? lastRule = null;
        for ( string? line = reader.ReadLine(); line != null; line = reader.ReadLine() )
        {
            lineNumber++;
            GrammarRule? newRule = ParseLine( line, lineNumber, lastRule );
            if ( newRule is not null )
            {
                rules.Add( lastRule! );
                lastRule = newRule;
            }
        }

        return new Grammar( rules );
    }

    private GrammarRule? ParseLine( string line, int lineNumber, GrammarRule? lastRule )
    {
        // Empty line
        if ( String.IsNullOrWhiteSpace( line ) )
        {
            return null;
        }
            
        // Comment
        if ( IsComment( line ) )
        {
            return null;
        }

        GrammarLineParseResult lineParseResult = ParseGrammarRule( line );
        if ( !lineParseResult.HasData )
        {
            return null;
        }

        // Rule name was probably declared previously, rule values are enumerating
        if ( lineParseResult.RuleName is null )
        {
            // Did not declared any rule before, throw an exception
            if ( lastRule is null )
            {
                throw new FormatException( $"Rule values are enumerated without declaring a ruleName. Line: {lineNumber}" );
            }
                
            lastRule.Values.AddRange( lineParseResult.Rules ?? throw new UnreachableException() );
            return null;
        }

        var rule = new GrammarRule( lineParseResult.RuleName );
        if ( lineParseResult.Rules is not null )
        {
            rule.Values = lineParseResult.Rules;
        }

        return rule;
    }

    private static bool IsComment( string line )
    {
        foreach ( char symbol in line )
        {
            if ( Char.IsWhiteSpace( symbol ) )
            {
                continue;
            }

            return symbol == CommentIdentifier;
        }

        return false;
    }

    private GrammarLineParseResult ParseGrammarRule( string line )
    {
        var result = new GrammarLineParseResult();
        
        int ruleDeclarationIndex = line.IndexOf( RuleNameSeparator, StringComparison.Ordinal );

        int lineSymbolIndex = 0;
        if ( ruleDeclarationIndex >= 0 )
        {
            result.RuleName = _ruleNameParser.ParseFromLine( line, ruleDeclarationIndex, out int lastLineReadSymbolIndex );
            lineSymbolIndex = lastLineReadSymbolIndex + RuleNameSeparator.Length;
        }
        
        result.Rules = ParseRules( line, lineSymbolIndex );

        return result;
    }

    private List<RuleValue> ParseRules( string line, int lineSymbolIndex )
    {
        var possibleValues = new List<RuleValue>();

        bool isReadingValue = false;
        var lastRawValue = new List<char>();
        for ( ; lineSymbolIndex < line.Length; lineSymbolIndex++ )
        {
            char symbol = line[lineSymbolIndex];

            // Whitespaces do not count if don't read a rule value
            if ( !isReadingValue && Char.IsWhiteSpace( symbol ) )
            {
                continue;
            }

            // If a separator, commit current rule and create a new one
            if ( symbol == RuleValueSeparator )
            {
                isReadingValue = false;
                
                if ( lastRawValue.Any() )
                {
                    possibleValues.Add( _ruleValueParser.Parse( lastRawValue ) );
                    lastRawValue = new List<char>();
                }

                continue;
            }

            isReadingValue = true;

            // We are reading a rule value
            lastRawValue.Add( symbol );
        }

        if ( lastRawValue.Any() )
        {
            possibleValues.Add( _ruleValueParser.Parse( lastRawValue ) );
        }

        return possibleValues;
    }
}