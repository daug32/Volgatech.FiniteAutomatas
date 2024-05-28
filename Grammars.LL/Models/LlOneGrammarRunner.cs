namespace Grammars.LL.Models;

public class LlOneGrammarRunner
{
    private readonly LlOneGrammar _grammar;

    public LlOneGrammarRunner( LlOneGrammar grammar )
    {
        _grammar = grammar;
    }

    public RunResult Run( string sentence )
    {
        return RunResult.Fail( sentence, RunError.InvalidSentence( 1 ) );
    }
}