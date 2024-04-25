using System.Diagnostics;
using FiniteAutomatas.Grammar.LeftToRightOne.Console.Models;
using FiniteAutomatas.Grammar.LeftToRightOne.Console.Parsers.Implementation;

namespace FiniteAutomatas.Grammar.LeftToRightOne.Console.Parsers;

public class GrammarRulesParser
{
    private const char CommentIdentifier = '#';
    private const string RuleNameSeparator = "->";
    private const char RuleValueSeparator = ',';

    private readonly GrammarRuleNameParser _ruleNameParser = new();
    private readonly GrammarRuleValueParser _ruleValueParser = new();

    private readonly string _fullFilePath; 

    public GrammarRulesParser( string fullFilePath )
    {
        if ( !File.Exists( fullFilePath ) )
        {
            throw new ArgumentException( $"File was not found. FilePath: {Path.GetFileName( fullFilePath )}" );
        }

        _fullFilePath = fullFilePath;
    }

    public Dictionary<GrammarRuleName, GrammarRule> Parse()
    {
        var result = new List<GrammarRule>();
        using var reader = new StreamReader( _fullFilePath );

        GrammarRule? lastRule = null;

        int lineNumber = 0;
        for ( string? line = reader.ReadLine(); line != null; line = reader.ReadLine() )
        {
            lineNumber++;

            // Empty line
            if ( String.IsNullOrWhiteSpace( line ) )
            {
                continue;
            }
            
            // Comment
            if ( IsComment( line ) )
            {
                continue;
            }

            GrammarRuleLineParseResult lineParseResult = ParseGrammarRule( line );
            if ( !lineParseResult.HasData )
            {
                continue;
            }

            // Rule name was declared previously, rule values are enumerating
            if ( lineParseResult.RuleName is null )
            {
                if ( lastRule is null )
                {
                    throw new FormatException( $"Rule values are enumerated without declaring a ruleName. Line: {lineNumber}" );
                }
                
                lastRule.Values.AddRange( lineParseResult.Rules ?? throw new UnreachableException() );
                continue;
            }

            var rule = new GrammarRule( lineParseResult.RuleName );
            if ( lineParseResult.Rules is not null )
            {
                rule.Values = lineParseResult.Rules;
            }

            result.Add( rule );
            lastRule = rule;
        }

        return result.ToDictionary(
            x => x.Name, 
            x => x );
    }

    private GrammarRuleLineParseResult ParseGrammarRule( string line )
    {
        var result = new GrammarRuleLineParseResult();
        
        int ruleDeclarationIndex = line.IndexOf( RuleNameSeparator, StringComparison.Ordinal );

        int lineSymbolIndex = 0;
        if ( ruleDeclarationIndex >= 0 )
        {
            result.RuleName = _ruleNameParser.Parse( line, ruleDeclarationIndex, out int lastLineReadSymbolIndex );
            lineSymbolIndex = lastLineReadSymbolIndex + RuleNameSeparator.Length;
        }
        
        result.Rules = ParseRules( line, lineSymbolIndex );

        return result;
    }

    private bool IsComment( string line )
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

    private List<GrammarRuleValue> ParseRules( string line, int lineSymbolIndex )
    {
        var possibleValues = new List<GrammarRuleValue>();

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