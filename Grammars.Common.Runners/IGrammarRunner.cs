using Grammars.Common.Runners.Results;

namespace Grammars.Common.Runners;

public interface IGrammarRunner
{
    public RunResult Run( string sentence );
}