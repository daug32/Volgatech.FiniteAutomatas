using FiniteAutomatas.Grammar.LeftToRightOne.Console.Models;

namespace FiniteAutomatas.Grammar.LeftToRightOne.Console.Parsers;

public class GrammarRulesParser
{
    private readonly string _fullFilePath;
    private readonly string _ruleNameSeparator = "->";
    private readonly char _ruleValueSeparator = ',';

    public GrammarRulesParser( string fullFilePath )
    {
        if ( !File.Exists( fullFilePath ) )
        {
            throw new ArgumentException( $"File was not found. FilePath: {Path.GetFileName( fullFilePath )}" );
        }

        _fullFilePath = fullFilePath;
    }

    public Dictionary<GrammarRuleName, List<GrammarRule>> Parse()
    {
        var result = new Dictionary<GrammarRuleName, List<GrammarRule>>();
        using var reader = new StreamReader( _fullFilePath );

        string? line = reader.ReadLine();
        while ( !String.IsNullOrWhiteSpace( line ) )
        {
            TryParseRuleName( line, out int lastIndex );

            line = reader.ReadLine();
        }

        return result;
    }

    private GrammarRuleName? TryParseRuleName( string line, out int lastIndex )
    {
        System.Console.WriteLine( $"Line: \"{line}\"" );

        int ruleDeclarationIndex = line.IndexOf( _ruleNameSeparator, StringComparison.Ordinal );

        var rawRuleName = new List<char>( ruleDeclarationIndex + 1 );
        
        var possibleRawValues = new List<List<char>>();
        var lastRawValue = new List<char>();
        for ( int i = 0; i < line.Length; i++ )
        {
            char symbol = line[i];

            // Whitespaces do not count
            if ( Char.IsWhiteSpace( symbol ) )
            {
                continue;
            }

            // Skip if still read ruleName 
            if ( i < ruleDeclarationIndex )
            {
                rawRuleName.Add( symbol );
                continue;
            }

            if ( i < ruleDeclarationIndex + _ruleNameSeparator.Length )
            {
                continue;
            }

            // If a separator, commit current rule, create a new one
            if ( symbol == _ruleValueSeparator )
            {
                if ( lastRawValue.Any() )
                {
                    possibleRawValues.Add( lastRawValue );
                    lastRawValue = new List<char>();
                }

                continue;
            }
            
            // We are reading a rule value
            lastRawValue.Add( symbol );
        }

        if ( lastRawValue.Any() )
        {
            possibleRawValues.Add( lastRawValue );
        }

        var rules = possibleRawValues
            .Select( x => new string( x.ToArray() ) )
            .ToList();

        var ruleName = rawRuleName.Any()
            ? new string( rawRuleName.ToArray() )
            : "";
        System.Console.WriteLine( $"\tRuleName: \"{ruleName}\"" );
        foreach ( var rule in rules )
        {
            System.Console.WriteLine( $"\t\tRule: \"{rule}\"" );
        }

        lastIndex = 0;
        return null;
    }
}