using Grammar.Parsers.Implementation.Implementation;
using Grammar.Parsers.Implementation.Implementation.Models;
using Grammars.Common.Grammars;

namespace Grammar.Parsers.Implementation;

public class GrammarFileParser : IGrammarParser
{
    private readonly string _filePath;

    private readonly LineParser _lineParser;
    private readonly GrammarBuilder _grammarBuilder;

    public GrammarFileParser(
        string filePath,
        ParsingSettings settings )
    {
        _filePath = !File.Exists( filePath )
            ? throw new ArgumentException( $"File was not found. FilePath: {Path.GetFullPath( filePath )}" )
            : filePath;

        _grammarBuilder = new GrammarBuilder();
        _lineParser = new LineParser( settings );
    }

    public CommonGrammar Parse()
    {
        using var reader = new StreamReader( _filePath );

        var rules = new List<GrammarRuleParseResult>();

        var lineNumber = 0;
        GrammarRuleParseResult? lastRule = null;
        for ( string? line = reader.ReadLine(); line != null; line = reader.ReadLine() )
        {
            lineNumber++;

            GrammarRuleParseResult? newRule;
            try
            {
                newRule = _lineParser.ParseLine( line, lineNumber, lastRule );
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

        return _grammarBuilder.BuildGrammar( rules );
    }
}