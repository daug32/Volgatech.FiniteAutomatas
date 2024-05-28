using Grammar.Parsers.Implementation.Implementation;
using Grammar.Parsers.Implementation.Implementation.Models;
using Grammars.Common;
using Grammars.Common.Grammars;

namespace Grammar.Parsers.Implementation;

public class GrammarInMemoryStringParser
{
    private readonly string[] _content;
    private readonly LineParser _lineParser;
    private readonly GrammarBuilder _grammarBuilder;

    public GrammarInMemoryStringParser( string content, ParsingSettings settings )
    {
        _content = content.Split( '\n' );
        _lineParser = new LineParser( settings );
        _grammarBuilder = new GrammarBuilder();
    }

    public CommonGrammar Parse()
    {
        var rules = new List<GrammarRuleParseResult>();

        var lineNumber = 0;
        GrammarRuleParseResult? lastRule = null;
        foreach( string line in _content )
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