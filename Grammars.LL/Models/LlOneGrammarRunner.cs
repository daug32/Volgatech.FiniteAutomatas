namespace Grammars.LL.Models;

public class LlOneGrammarRunner
{
    private readonly LlOneGrammar _llOneGrammar;

    public LlOneGrammarRunner( LlOneGrammar llOneGrammar )
    {
        _llOneGrammar = llOneGrammar;
    }

    public RunResult Run( string sentence )
    {
        return RunResult.Fail( sentence, new RunError( 1 ) );
    }
}