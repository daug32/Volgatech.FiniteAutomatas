namespace Grammars.Common.Runners.Results;

public class RunResult
{
    public readonly string Sentence;
    public readonly RunResultType RunResultType;
    public readonly RunError? Error;

    public static RunResult Ok( string sentence ) => new RunResult( sentence, RunResultType.Ok, null );
    public static RunResult Fail( string sentence, RunError error ) => new RunResult( sentence, RunResultType.Error, error );

    private RunResult( string sentence, RunResultType runResultType, RunError? error )
    {
        Sentence = sentence;
        RunResultType = runResultType;
        Error = error;
    }
}