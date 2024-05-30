using Grammar.Parsers;
using Grammar.Parsers.Implementation;
using Grammars.Common.Convertors;
using Grammars.Common.Convertors.Convertors;
using Grammars.LL.Convertors;
using Grammars.LL.Models;
using Grammars.LL.Runners.Results;

namespace Grammar.Common.Tests.Runners;

public class LlOneRunnerTests
{
    private ParsingSettings _defaultParsingSettings = null!;

    private static readonly object[] _testCases =
    {
        new RunnerTestData(
            new[]
            {
                new RunnerInputTestData("a", true),
                new RunnerInputTestData("", false),
                new RunnerInputTestData("b", false),
                new RunnerInputTestData("aa", false),
            },
            "<S> -> a"),
        new RunnerTestData(
            new[]
            {
                new RunnerInputTestData("", true),
                new RunnerInputTestData("a", true),
                new RunnerInputTestData("aaa", true),
                new RunnerInputTestData("b", false),
            },
            "<S> -> <S>a | ε"),
        new RunnerTestData(
            new[]
            {
                new RunnerInputTestData("a", true),
                new RunnerInputTestData("ab", true),
                new RunnerInputTestData("ac", true),
                new RunnerInputTestData("abc", true),
                new RunnerInputTestData("b", true),
                new RunnerInputTestData("bc", true),
                new RunnerInputTestData("c", true),
                new RunnerInputTestData("", true),

                new RunnerInputTestData("aa", false),
                new RunnerInputTestData("abb", false),
                new RunnerInputTestData("abcc", false),
            },
            @"
                <S> -> <A><B><C>
                <A> -> a | ε
                <B> -> b | ε
                <C> -> c | ε
            "),
        new RunnerTestData(
            new[]
            {
                new RunnerInputTestData("aabbcc", false),
                new RunnerInputTestData("aa", false),
                new RunnerInputTestData("aabb", false),
                new RunnerInputTestData("aacc", false),
                new RunnerInputTestData("bb", false),
                new RunnerInputTestData("bbcc", false),
                new RunnerInputTestData("cc", false),

                new RunnerInputTestData("a", true),
                new RunnerInputTestData("ab", true),
                new RunnerInputTestData("ac", true),
                new RunnerInputTestData("abc", true),
                new RunnerInputTestData("b", true),
                new RunnerInputTestData("bc", true),
                new RunnerInputTestData("c", true),
                new RunnerInputTestData("", true),
            },
            @"
                <S> -> <A><B><C>d
                <A> -> <A>a | ε
                <B> -> <B>b | ε
                <C> -> <C>c | ε
            " )
    };

    [SetUp]
    public void Setup()
    {
        _defaultParsingSettings = new ParsingSettings();
    }

    [TestCaseSource(nameof(_testCases))]
    public void A(RunnerTestData testData)
    {
        LlOneGrammar grammar = new GrammarInMemoryStringParser(testData.Content, _defaultParsingSettings)
            .Parse()
            .Convert(new RemoveWhitespacesConvertor())
            .Convert(new ToLlOneGrammarConvertor());

        Assert.Multiple(() =>
        {
            foreach (RunnerInputTestData dataInput in testData.Inputs)
            {
                RunResult runResult = grammar.Run(dataInput.Input);
                Assert.AreEqual(
                    dataInput.IsSuccess,
                    runResult.RunResultType == RunResultType.Ok,
                    $"InputString: {dataInput.Input}\n" +
                    $"ExpectedRunResult: {dataInput.IsSuccess}\n" +
                    $"ActualRunResult: {runResult.RunResultType}\n" +
                    $"Grammar: {testData.Content}");
            }
        });
    }
}

public class RunnerTestData
{
    public List<RunnerInputTestData> Inputs;
    public string Content;

    public RunnerTestData(
        RunnerInputTestData input,
        string content)
        : this(new[] { input }, content)
    {
    }

    public RunnerTestData(
        IEnumerable<RunnerInputTestData> inputs,
        string content)
    {
        Content = content;
        Inputs = inputs.ToList();
    }
}

public class RunnerInputTestData
{
    public string Input;
    public bool IsSuccess;

    public RunnerInputTestData(string input, bool isSuccess)
    {
        Input = input;
        IsSuccess = isSuccess;
    }
}