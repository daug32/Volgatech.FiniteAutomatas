using System.Diagnostics;
using Grammar.Parsers.Implementation.Implementation;
using Grammar.Parsers.Implementation.Implementation.Models;
using Grammars.Common.ValueObjects;

namespace Grammar.Parsers.Implementation;

internal class LineParser
{
    private readonly ParsingSettings _settings;

    private readonly RuleNameParser _ruleNameParser;
    private readonly RuleDefinitionParser _ruleDefinitionParser;

    public LineParser( ParsingSettings settings )
    {
        _settings = settings;

        _ruleNameParser = new RuleNameParser();
        _ruleDefinitionParser = new RuleDefinitionParser( settings );
    }

    public GrammarRuleParseResult? ParseLine( string line, int lineNumber, GrammarRuleParseResult? lastRule )
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

    private bool IsComment( string line )
    {
        foreach ( char symbol in line )
        {
            if ( Char.IsWhiteSpace( symbol ) )
            {
                continue;
            }

            return symbol == _settings.CommentIdentifier;
        }

        return false;
    }

    private GrammarLineParseResult ParseGrammarRule( string line )
    {
        var result = new GrammarLineParseResult();

        int ruleDeclarationIndex = line.IndexOf( _settings.RuleNameSeparator, StringComparison.Ordinal );

        var index = 0;
        if ( ruleDeclarationIndex >= 0 )
        {
            result.RuleName = _ruleNameParser.ParseFromLine( line, ruleDeclarationIndex, out int lastLineReadSymbolIndex );
            index = lastLineReadSymbolIndex + _settings.RuleNameSeparator.Length;
        }

        result.Rules = ParseRules( line, index );

        return result;
    }

    // "<S> -> BEGIN <exp> END. | BEGIN END." => { { BEGIN, <exp>, END. }, { BEGIN, END. } }  
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
            if ( symbol == _settings.RuleDefinitionsSeparator )
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