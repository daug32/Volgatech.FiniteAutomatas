using Grammar.Parsers;
using Grammar.Parsers.Implementation;
using Grammar.Tests.Runners.Containers;
using Grammars.Common.Convertors;
using Grammars.Common.Convertors.Convertors;
using Grammars.LL.Convertors;
using Grammars.LL.Models;
using Grammars.LL.Runners.Results;
using LinqExtensions;

namespace Grammar.Tests.Runners;

public class LlOneRunnerTests
{
    private ParsingSettings _defaultParsingSettings = null!;

    private static readonly object[] _testCases = new List<RunnerTestData>()
        .WithMany( RunnerTestData.FromList(
            "<S> -> a",
            new[]
            {
                new RunnerInputTestData( "a", true ),
                new RunnerInputTestData( "", false ),
                new RunnerInputTestData( "b", false ),
                new RunnerInputTestData( "aa", false )
            } ) )
        .WithMany( RunnerTestData.FromList(
            "<S> -> word",
            new[]
            {
                new RunnerInputTestData( "word", true ),
                new RunnerInputTestData( "w o r d", false ),
            } ) )
        .WithMany( RunnerTestData.FromList(
            "<S> -> <S>a | ε",
            new[]
            {
                new RunnerInputTestData( "", true ),
                new RunnerInputTestData( "a", true ),
                new RunnerInputTestData( "aaa", true ),
                new RunnerInputTestData( "b", false )
            } ) )
        .WithMany( RunnerTestData.FromList(
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
            } ) )
        .WithMany( RunnerTestData.FromList(
            @"
                <S> -> <A><B><C>d
                <A> -> <A>a | ε
                <B> -> <B>b | ε
                <C> -> <C>c | ε
            ",
            new[]
            {
                new RunnerInputTestData( "aabbcc", true ),
                new RunnerInputTestData( "aa", true ),
                new RunnerInputTestData( "aabb", true ),
                new RunnerInputTestData( "aacc", true ),
                new RunnerInputTestData( "bb", true ),
                new RunnerInputTestData( "bbcc", true ),
                new RunnerInputTestData( "cc", true ),

                new RunnerInputTestData( "a", true ),
                new RunnerInputTestData( "ab", true ),
                new RunnerInputTestData( "ac", true ),
                new RunnerInputTestData( "abc", true ),
                new RunnerInputTestData( "b", true ),
                new RunnerInputTestData( "bc", true ),
                new RunnerInputTestData( "c", true ),
                new RunnerInputTestData( "", true )
            } ) )
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
        // Arrange
        LlOneGrammar grammar = new GrammarInMemoryStringParser( testData.Content, _defaultParsingSettings )
            .Parse()
            .Convert( new RemoveWhitespacesConvertor() )
            .Convert( new ToLlOneGrammarConvertor() );

        // Act
        RunResult runResult = grammar.Run( testData.Input.Value );

        // Assert
        string assertMessage = $"InputString: {testData.Input.Value}\n"
                               + $"ExpectedRunResult: {testData.Input.IsSuccess}\n"
                               + $"ActualRunResult: {runResult.RunResultType}\n"
                               + $"Grammar: {testData.Content}";

        Assert.That(
            runResult.RunResultType == RunResultType.Ok,
            Is.EqualTo( testData.Input.IsSuccess ),
            assertMessage );

        Assert.Pass( assertMessage );
    }
}