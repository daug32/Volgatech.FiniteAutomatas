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
        .WithMany( RunnerTestData.FromList(
            @"
                <entry> -> <expression> $
                <value> -> id | !float | !int
                <expression> -> <expression_enum> <expression_summ>
                <expression_enum> -> <expression_val> <expression_multi>
                <expression_summ> -> <summ_op> <expression_enum> <expression_summ> | ε
                <expression_multi> -> <multi_op> <expression_val> <expression_multi> | ε
                <expression_val> -> - <expression_val> | <value> | ( <expression> )
                <multi_op> -> * | /
                <summ_op> -> - | +
            ",
            new[]
            {
                new RunnerInputTestData( "!float * id", true ),
                new RunnerInputTestData( "!float / id", true ),
                new RunnerInputTestData( "* * id id", false ),
            } ) )
        .WithMany( RunnerTestData.FromList(
            @"
                <S> -> <A> <B> <C> $
                <A> -> <A> a | ε
                <B> -> <B> b | ε
                <C> -> c <C> | ε
            ",
            new[]
            {
                new RunnerInputTestData( "a a a b b c c", true ),
                new RunnerInputTestData( "a a a a a a a a a a a a a b a", false ),
            } ) )
        .WithMany( RunnerTestData.FromList(
            @"
                <F> -> <S> $
                <S> -> id = <E> | while <E> do <S>
                <E> -> <E> + <E> | id
            ",
            new[]
            {
                new RunnerInputTestData( "while id + id do while id do id = id", true ),
                new RunnerInputTestData( "while id do id = id", true ),
                new RunnerInputTestData( "while while id = id do id = id + id", false ),
            } ) )
        .WithMany( RunnerTestData.FromList(
            @"
                <F> -> <S> $
                <S> -> if <A> then <S> | if <A> then <S> else <S> | <B>
                <A> -> <A> + <A> | id 
                <B> -> id = <A>
            ",
            new[]
            {
                new RunnerInputTestData( "if id + id then id = id", true ),
                new RunnerInputTestData( "if id then id = id else id = id = id", false ),
            } ) )
        .WithMany( RunnerTestData.FromList(
            @"
                <F> -> <S> $
                <S> -> <S> a | ( <S> ) | b
            ",
            new[]
            {
                new RunnerInputTestData( "( b ) a a a a", true ),
                new RunnerInputTestData( "a a a", false ),
            } ) )
        .WithMany( RunnerTestData.FromList(
            @"
                <F> -> <S> $
                <S> -> a | ( <S> ) | a <S> | b
            ",
            new[]
            {
                new RunnerInputTestData( "( ( a ) )", true ),
                new RunnerInputTestData( "( ( a a b ) ) b", false ),
            } ) )
        .WithMany( RunnerTestData.FromList(
            @"
                <S> -> type <F> $
                <F> -> <I> = <T> | <I> = <T> ; <F>
                <T> -> int | record <G> end
                <G> -> <I> : <T> | <I> : <T> ; <G>
                <I> -> a | b | c
            ",
            new[]
            {
                new RunnerInputTestData( "type a = int ; b = record a : int end", true ),
                new RunnerInputTestData( "type a = int ; b = record a : int ; record a : int end", false ),
                new RunnerInputTestData( "type a = int ; b = record a : int ; record a : int end end end", false ),
            } ) )
        .WithMany( RunnerTestData.FromList(
            @"
                <F> -> function <I> ( <I> ) <G> end $
                <G> -> <I> := <E> | <I> := <E> ; <G>
                <E> -> <E> * <I> | <E> + <I> | <I>
                <I> -> a | b
            ",
            new[]
            {
                new RunnerInputTestData( "function a ( a ) a := b + b * b end", true ),
                new RunnerInputTestData( "function a ( b := b + b ) end", false ),
            } ) )
        .WithMany( RunnerTestData.FromList(
            @"
                <F> -> <S> $
                <S> -> <S> <A> | x
                <A> -> <A> <S> | y
            ",
            new[]
            {
                new RunnerInputTestData( "x y x y", true ),
                new RunnerInputTestData( "x y x", true ),
                new RunnerInputTestData( "x y y", true ),
                new RunnerInputTestData( "x x x x", false ),
            } ) )
        .WithMany( RunnerTestData.FromList(
            @"
                <F> -> <M1> $
                <M1> -> f <M1> | f <B> | a <B> | a d | f a
                <B> -> <B> a | <B> b | c
            ",
            new[]
            {
                new RunnerInputTestData( "f f c b a", true ),
                new RunnerInputTestData( "a f", false ),
                new RunnerInputTestData( "f a d", false ),
                new RunnerInputTestData( "f f f a c d", false ),
            } ) )
        .WithMany( RunnerTestData.FromList(
            @"
                <F> -> <M1> $
                <M1> -> f <M1> | f <B> | a <B> | a d | f a
                <B> -> <B> a | <B> b | c
            ",
            new[]
            {
                new RunnerInputTestData( "f f c b a", true ),
                new RunnerInputTestData( "a f", false ),
                new RunnerInputTestData( "f a d", false ),
                new RunnerInputTestData( "f f f a c d", false ),
            } ) )
        .WithMany( RunnerTestData.FromList(
            @"
                <F> -> <S> $
                <S> -> <S> a | <S> b | c | a d
            ",
            new[]
            {
                new RunnerInputTestData( "a d b b a a", true ),
                new RunnerInputTestData( "c a b a", true ),
                new RunnerInputTestData( "a c", false ),
                new RunnerInputTestData( "a a a a a a a a d d", false ),
                new RunnerInputTestData( "d a b a", false ),
            } ) )
        .WithMany( RunnerTestData.FromList(
            @"
                <F> -> <M1> $
                <M1> -> f m <M1> | f <M2> | a <M2> | a d | f m
                <M2> -> <M2> a | <M2> b | c | ε
            ",
            new[]
            {
                new RunnerInputTestData( "f m f", true ),
                new RunnerInputTestData( "f m f m f m f m", true ),
                new RunnerInputTestData( "a d", true ),
                new RunnerInputTestData( "f b a a ", true ),
                new RunnerInputTestData( "f m f c c", false ),
                new RunnerInputTestData( "f m f m a d f m", false ),
                new RunnerInputTestData( "r r r", false ),
            } ) )
        .WithMany( RunnerTestData.FromList(
            @"
                <PROG> -> begin d ; <X> end
                <X> -> d ; <X> |  s <Y>
                <Y> -> ε | ; s <Y>
            ",
            new[]
            {
                new RunnerInputTestData( "begin d ; d ; s end", true ),
                new RunnerInputTestData( "begin d ; d ; s ; s end", true ),
                new RunnerInputTestData( "begin d ; s ; d ; end", false ),
            } ) )
        .Cast<object>()
        .ToArray();
}