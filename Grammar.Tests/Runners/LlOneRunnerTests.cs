using Grammar.Parsers;
using Grammar.Parsers.Implementation;
using Grammar.Tests.Runners.Containers;
using Grammars.Common.Convertors;
using Grammars.Common.Convertors.Convertors;
using Grammars.Common.Grammars.ValueObjects.Symbols;
using Grammars.LL.Convertors;
using Grammars.LL.Models;
using Grammars.LL.Runners;
using Grammars.LL.Runners.Results;
using LinqExtensions;

namespace Grammar.Tests.Runners;

[TestFixture]
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
            "<S> -> a<S> | ε",
            new[]
            {
                new RunnerInputTestData( "", true ),
                new RunnerInputTestData( "a", true ),
                new RunnerInputTestData( "a a a", true ),
                new RunnerInputTestData( "b", false )
            } ) )
        .WithMany( RunnerTestData.FromList(
            "<S> -> <S>a | ε",
            new[]
            {
                new RunnerInputTestData( "", true ),
                new RunnerInputTestData( "a", true ),
                new RunnerInputTestData( "a a a", true ),
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
                new RunnerInputTestData( "a b", true ),
                new RunnerInputTestData( "a c", true ),
                new RunnerInputTestData( "a b c", true ),
                new RunnerInputTestData( "b", true ),
                new RunnerInputTestData( "b c", true ),
                new RunnerInputTestData( "c", true ),
                new RunnerInputTestData( "", true ),

                new RunnerInputTestData( "a a", false ),
                new RunnerInputTestData( "a b b", false ),
                new RunnerInputTestData( "a b c c", false )
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
                new RunnerInputTestData( "a a b b c c d", true ),
                new RunnerInputTestData( "a a d", true ),
                new RunnerInputTestData( "a a b b d", true ),
                new RunnerInputTestData( "a a c c d", true ),
                new RunnerInputTestData( "b b d", true ),
                new RunnerInputTestData( "b b c c d", true ),
                new RunnerInputTestData( "c c d", true ),

                new RunnerInputTestData( "a d", true ),
                new RunnerInputTestData( "a b d", true ),
                new RunnerInputTestData( "a c d", true ),
                new RunnerInputTestData( "a b c d", true ),
                new RunnerInputTestData( "b d", true ),
                new RunnerInputTestData( "b c d", true ),
                new RunnerInputTestData( "c d", true ),
                new RunnerInputTestData( "d", true ),
                
                new RunnerInputTestData( "a", false ),
                new RunnerInputTestData( "b", false ),
                new RunnerInputTestData( "c", false ),
                new RunnerInputTestData( "", false )
            } ) )
        .WithMany( RunnerTestData.FromList(
            @"
                <S> -> <A><B>s
                <A> -> <A>a | ε
                <B> -> <B>b | ε
            ",
            new[]
            {
                new RunnerInputTestData( "a a a s", true ),
                new RunnerInputTestData( "a s", true ),
                new RunnerInputTestData( "s", true ),
            } ) )
        .Cast<object>()
        .ToArray();

    [SetUp]
    public void Setup()
    {
        _defaultParsingSettings = new ParsingSettings();
    }

    [Test]
    [MaxTime( 2000 )]
    [Parallelizable( ParallelScope.All )]
    [TestCaseSource( nameof( _testCases ) )]
    public void RunnerTest( RunnerTestData testData )
    {
        TestContext.WriteLine( 
            $"InputString: {testData.Input.Value}\n"
            + $"ExpectedRunResult: {testData.Input.IsSuccess}\n"
            + $"Original grammar: {testData.Content}" );
        
        // Arrange
        LlOneGrammar grammar = new GrammarInMemoryStringParser( testData.Content, _defaultParsingSettings )
            .Parse()
            .Convert( new RemoveWhitespacesConvertor() )
            .Convert( new ToLlOneGrammarConvertor() );

        var runner = new LlOneGrammarRunner( grammar );

        // Act
        RunResult runResult = runner.Run( testData.Input.Value );

        // Assert
        string serializedLlGrammar = String.Join( 
            "\n",
            grammar.Rules.Values.SelectMany( rule =>
                rule.Definitions.Select( definition =>
                {
                    string serializedSymbols = String.Join( 
                        " ", 
                        definition.Symbols.Select( x => x.Type == RuleSymbolType.NonTerminalSymbol 
                            ? $"<{x}>"
                            : x.ToString() ) );
                    return $"<{rule.Name}> -> {serializedSymbols}";
                } ) ) );
        
        Assert.That(
            runResult.RunResultType == RunResultType.Ok,
            Is.EqualTo( testData.Input.IsSuccess ),
            $"ActualRunResult: {runResult.RunResultType}\n"
            + $"LL grammar: {serializedLlGrammar}" );
    }
}