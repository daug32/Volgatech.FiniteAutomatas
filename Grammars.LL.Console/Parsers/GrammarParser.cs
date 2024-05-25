using System.Diagnostics;
using Grammars.Grammars.LeftRoRightOne.Models;
using Grammars.Grammars.LeftRoRightOne.Models.ValueObjects;
using Grammars.LL.Console.Parsers.Implementation;
using Grammars.LL.Console.Parsers.Implementation.Models;

namespace Grammars.LL.Console.Parsers;

public class GrammarParser
{
    private const char CommentIdentifier = '#';
    private const string RuleNameSeparator = "->";
    private const char RuleValueSeparator = ',';

    private readonly RuleNameParser _ruleNameParser = new();
    private readonly RuleDefinitionParser _ruleDefinitionParser = new();

    public LlOneGrammar ParseFile( string fullFilePath )
    {
        if ( !File.Exists( fullFilePath ) )
        {
            throw new ArgumentException( $"File was not found. FilePath: {Path.GetFullPath( fullFilePath )}" );
        }

        using var reader = new StreamReader( fullFilePath );

        var rules = new List<GrammarRuleParseResult>();

        var lineNumber = 0;
        GrammarRuleParseResult? lastRule = null;
        for ( string? line = reader.ReadLine(); line != null; line = reader.ReadLine() )
        {
            lineNumber++;

            GrammarRuleParseResult? newRule;
            try
            {
                newRule = ParseLine( line, lineNumber, lastRule );
            }
            catch ( Exception ex )
            {
                throw new AggregateException( $"\nLine({lineNumber}): \"{line}\"\n", ex );
            }

            if ( newRule is not null )
            {
                if ( lastRule is not null )
                {
                    rules.Add( lastRule! );
                }

                lastRule = newRule;
            }
        }

        if ( lastRule is not null )
        {
            rules.Add( lastRule );
        }

        return new LlOneGrammar(
            rules.First().RuleName,
            rules.Select( x => new GrammarRule( x.RuleName, x.RuleDefinitions ) ) );
    }

    private GrammarRuleParseResult? ParseLine( string line, int lineNumber, GrammarRuleParseResult? lastRule )
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

            lastRule.RuleDefinitions.AddRange( lineParseResult.Rules ?? throw new UnreachableException() );
            return null;
        }

        var rule = new GrammarRuleParseResult( lineParseResult.RuleName );
        if ( lineParseResult.Rules is not null )
        {
            rule.RuleDefinitions = lineParseResult.Rules;
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

        var index = 0;
        if ( ruleDeclarationIndex >= 0 )
        {
            result.RuleName = _ruleNameParser.ParseFromLine( line, ruleDeclarationIndex, out int lastLineReadSymbolIndex );
            index = lastLineReadSymbolIndex + RuleNameSeparator.Length;
        }

        result.Rules = ParseRules( line, index );

        return result;
    }

    // "S -> BEGIN <exp> END., BEGIN END." => { {BEGIN, <exp>, END, .}, {BEGIN, END, .} }  
    private List<RuleDefinition> ParseRules( string line, int startIndex )
    {
        var possibleValues = new List<RuleDefinition>();

        var isReadingValue = false;
        var lastRawValue = new List<char>();
        for ( ; startIndex < line.Length; startIndex++ )
        {
            char symbol = line[startIndex];
            
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
                    possibleValues.Add( _ruleDefinitionParser.Parse( lastRawValue ) );
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
            possibleValues.Add( _ruleDefinitionParser.Parse( lastRawValue ) );
        }

        return possibleValues;
    }
}