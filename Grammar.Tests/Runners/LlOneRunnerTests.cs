using Grammar.Parsers;
using Grammar.Parsers.Implementation;
using Grammar.Tests.Runners.Containers;
using Grammars.Common.Convertors;
using Grammars.Common.Convertors.Convertors;
using Grammars.LL.Convertors;
using Grammars.LL.Models;
using Grammars.LL.Runners.Results;

namespace Grammar.Tests.Runners;

public class LlOneRunnerTests
{
    private ParsingSettings _defaultParsingSettings = null!;

    private static readonly object[] _testCases = new RunnerTestData[][]
        {
            RunnerTestData.FromList(
                "<S> -> a",
                new[]
                {
                    new RunnerInputTestData( "a", true ),
                    new RunnerInputTestData( "", false ),
                    new RunnerInputTestData( "b", false ),
                    new RunnerInputTestData( "aa", false )
                } ),

            RunnerTestData.FromList(
                "<S> -> <S>a | ε",
                new[]
                {
                    new RunnerInputTestData( "", true ),
                    new RunnerInputTestData( "a", true ),
                    new RunnerInputTestData( "aaa", true ),
                    new RunnerInputTestData( "b", false )
                } ),

            RunnerTestData.FromList(
                @"
                <S> -> <A><B><C>
                <A> -> a | ε
                <B> -> b | ε
                <C> -> c | ε
            ",
                new[]
                {
                    new RunnerInputTestData( "a", true ),
                    new RunnerInputTestData( "ab", true ),
                    new RunnerInputTestData( "ac", true ),
                    new RunnerInputTestData( "abc", true ),
                    new RunnerInputTestData( "b", true ),
                    new RunnerInputTestData( "bc", true ),
                    new RunnerInputTestData( "c", true ),
                    new RunnerInputTestData( "", true ),

                    new RunnerInputTestData( "aa", false ),
                    new RunnerInputTestData( "abb", false ),
                    new RunnerInputTestData( "abcc", false )
                } ),

            RunnerTestData.FromList(
                @"
                <S> -> <A><B><C>d
                <A> -> <A>a | ε
                <B> -> <B>b | ε
                <C> -> <C>c | ε
            ",
                new[]
                {
                    new RunnerInputTestData( "aabbcc", false ),
                    new RunnerInputTestData( "aa", false ),
                    new RunnerInputTestData( "aabb", false ),
                    new RunnerInputTestData( "aacc", false ),
                    new RunnerInputTestData( "bb", false ),
                    new RunnerInputTestData( "bbcc", false ),
                    new RunnerInputTestData( "cc", false ),

                    new RunnerInputTestData( "a", true ),
                    new RunnerInputTestData( "ab", true ),
                    new RunnerInputTestData( "ac", true ),
                    new RunnerInputTestData( "abc", true ),
                    new RunnerInputTestData( "b", true ),
                    new RunnerInputTestData( "bc", true ),
                    new RunnerInputTestData( "c", true ),
                    new RunnerInputTestData( "", true )
                } )
        }.SelectMany( x => x )
        .Cast<object>()
        .ToArray();

    [SetUp]
    public void Setup()
    {
        _defaultParsingSettings = new ParsingSettings();
    }

    [TestCaseSource( nameof( _testCases ) )]
    public void RunnerTest( RunnerTestData testData )
    {
        LlOneGrammar grammar = new GrammarInMemoryStringParser( testData.Content, _defaultParsingSettings )
            .Parse()
            .Convert( new RemoveWhitespacesConvertor() )
            .Convert( new ToLlOneGrammarConvertor() );

        RunResult runResult = grammar.Run( testData.Input.Value );
        Assert.That(
            runResult.RunResultType == RunResultType.Ok,
            Is.EqualTo( testData.Input.IsSuccess ),
            $"InputString: {testData.Input.Value}\n"
            + $"ExpectedRunResult: {testData.Input.IsSuccess}\n"
            + $"ActualRunResult: {runResult.RunResultType}\n"
            + $"Grammar: {testData.Content}" );
    }
}