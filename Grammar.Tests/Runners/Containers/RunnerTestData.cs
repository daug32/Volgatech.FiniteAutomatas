namespace Grammar.Tests.Runners.Containers;

public class RunnerTestData
{
    public readonly RunnerInputTestData Input;
    public readonly string Content;

    public RunnerTestData(
        RunnerInputTestData input,
        string content)
    {
        Content = content;
        Input = input;
    }

    public static RunnerTestData[] FromList(
        string content,
        IEnumerable<RunnerInputTestData> testData ) => testData.Select( x => new RunnerTestData( x, content ) ).ToArray();
}