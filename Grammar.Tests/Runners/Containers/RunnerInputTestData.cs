namespace Grammar.Tests.Runners.Containers;

public class RunnerInputTestData
{
    public readonly string Value;
    public readonly bool IsSuccess;

    public RunnerInputTestData(string value, bool isSuccess)
    {
        Value = value;
        IsSuccess = isSuccess;
    }
}