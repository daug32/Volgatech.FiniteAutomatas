using Grammars.LL.Models;
using Grammars.LL.Runners.Results;

namespace Grammars.LL.Runners;

public class LlOneGrammarRunner
{
    private readonly LlOneGrammar _grammar;
    private readonly ParsingTableCreator _tableCreator;

    public LlOneGrammarRunner( LlOneGrammar grammar )
    {
        _grammar = grammar;
        _tableCreator = new ParsingTableCreator();
    }

    public RunResult Run( string sentence )
    {
        ParsingTable table = _tableCreator.Create( _grammar );
        
        return RunResult.Fail( sentence, RunError.InvalidSentence( 1 ) );
    }
}