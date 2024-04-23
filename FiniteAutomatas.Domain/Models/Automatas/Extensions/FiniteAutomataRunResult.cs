namespace FiniteAutomatas.Domain.Models.Automatas.Extensions;

public enum FiniteAutomataRunResult
{
    Unknown,
    FinishedOnSuccess,
    FinishedOnIntermediate,
    FinishedOnError,
}

public static class FiniteAutomataRunResultExtensions
{
    public static bool IsSuccess( this FiniteAutomataRunResult result ) => result == FiniteAutomataRunResult.FinishedOnSuccess;
}