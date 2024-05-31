using Grammar.Parsers;
using Grammar.Parsers.Implementation;
using Grammar.Tests.Extensions.Containers;
using Grammars.Common.Grammars;
using Grammars.Common.Grammars.Extensions;
using Grammars.Common.Grammars.ValueObjects.RuleNames;
using Grammars.Common.Grammars.ValueObjects.Symbols;
using LinqExtensions;

namespace Grammar.Tests.Extensions;

public class GrammarFirstSetExtensionsTests
{
    private static readonly ParsingSettings _defaultSettings = new();

    private static readonly object[] _testCases = new List<FirstFollowTestData>()
        .With( new FirstFollowTestData(
            new RuleName( "A" ),
            @"
                <S> -> <A> s $
                <A> -> <B> a | a
                <B> -> b
            ",
            new[]
            {
                RuleSymbol.TerminalSymbol( TerminalSymbol.Word( "a" ) ),
                RuleSymbol.TerminalSymbol( TerminalSymbol.Word( "b" ) )
            } ) )
        .With( new FirstFollowTestData(
            new RuleName( "S" ),
            @"
                <S> -> <A>$
                <A> -> <B>
                <B> -> b
            ",
            new[]
            {
                RuleSymbol.TerminalSymbol( TerminalSymbol.Word( "b" ) )
            } ) )
        .With( new FirstFollowTestData(
            new RuleName( "A" ),
            @"<A> -> <A>a | ε",
            new[]
            {
                RuleSymbol.TerminalSymbol( TerminalSymbol.Word( "a" ) ),
                RuleSymbol.TerminalSymbol( TerminalSymbol.EmptySymbol() ),
            } ) )
        .With( new FirstFollowTestData(
            new RuleName( "S" ),
            @"<S> -> s | ε",
            new[]
            {
                RuleSymbol.TerminalSymbol( TerminalSymbol.EmptySymbol() ),
                RuleSymbol.TerminalSymbol( TerminalSymbol.Word( "s" ) )
            } ) )
        .With( new FirstFollowTestData(
            new RuleName( "A" ),
            @"
                <A> -> <B><C>$
                <B> -> b | ε
                <C> -> c | ε
            ",
            new[]
            {
                RuleSymbol.TerminalSymbol( TerminalSymbol.Word( "b" ) ),
                RuleSymbol.TerminalSymbol( TerminalSymbol.Word( "c" ) ),
                RuleSymbol.TerminalSymbol( TerminalSymbol.End() )
            } ) )
        .With( new FirstFollowTestData(
            new RuleName( "S" ),
            @"
                <S> -> a<B><D>h$
                <B> -> c<C>
                <C> -> b<C> | ε
                <D> -> <E><F>
                <E> -> g | ε
                <F> -> f | ε
            ",
            new[]
            {
                RuleSymbol.TerminalSymbol( TerminalSymbol.Word( "a" ) )
            } ) )
        .With( new FirstFollowTestData(
            new RuleName( "B" ),
            @"
                <S> -> a<B><D>h$
                <B> -> c<C>
                <C> -> b<C> | ε
                <D> -> <E><F>
                <E> -> g | ε
                <F> -> f | ε
            ",
            new[]
            {
                RuleSymbol.TerminalSymbol( TerminalSymbol.Word( "c" ) )
            } ) )
        .With( new FirstFollowTestData(
            new RuleName( "C" ),
            @"
                <S> -> a<B><D>h$
                <B> -> c<C>
                <C> -> b<C> | ε
                <D> -> <E><F>
                <E> -> g | ε
                <F> -> f | ε
            ",
            new[]
            {
                RuleSymbol.TerminalSymbol( TerminalSymbol.Word( "b" ) ),
                RuleSymbol.TerminalSymbol( TerminalSymbol.EmptySymbol() )
            } ) )
        .With( new FirstFollowTestData(
            new RuleName( "D" ),
            @"
                <S> -> a<B><D>h$
                <B> -> c<C>
                <C> -> b<C> | ε
                <D> -> <E><F>
                <E> -> g | ε
                <F> -> f | ε
            ",
            new[]
            {
                RuleSymbol.TerminalSymbol( TerminalSymbol.Word( "g" ) ),
                RuleSymbol.TerminalSymbol( TerminalSymbol.Word( "f" ) ),
                RuleSymbol.TerminalSymbol( TerminalSymbol.EmptySymbol() )
            } ) )
        .With( new FirstFollowTestData(
            new RuleName( "E" ),
            @"
                <S> -> a<B><D>h$
                <B> -> c<C>
                <C> -> b<C> | ε
                <D> -> <E><F>
                <E> -> g | ε
                <F> -> f | ε
            ",
            new[]
            {
                RuleSymbol.TerminalSymbol( TerminalSymbol.Word( "g" ) ),
                RuleSymbol.TerminalSymbol( TerminalSymbol.EmptySymbol() )
            } ) )
        .With( new FirstFollowTestData(
            new RuleName( "F" ),
            @"
                <S> -> a<B><D>h$
                <B> -> c<C>
                <C> -> b<C> | ε
                <D> -> <E><F>
                <E> -> g | ε
                <F> -> f | ε
            ",
            new[]
            {
                RuleSymbol.TerminalSymbol( TerminalSymbol.Word( "f" ) ),
                RuleSymbol.TerminalSymbol( TerminalSymbol.EmptySymbol() )
            } ) )
        .Cast<object>()
        .ToArray();

    [TestCaseSource( nameof( _testCases ) )]
    public void FindFirstSet( FirstFollowTestData testData )
    {
        // Arrange
        CommonGrammar grammar = new GrammarInMemoryStringParser( testData.RawGrammar, _defaultSettings ).Parse();

        // Act
        var actualSymbols = grammar.GetFirstSet( testData.TargetRuleName ).GuidingSymbols.ToHashSet();

        // Assert
        Assert.That(
            actualSymbols.Count,
            Is.EqualTo( testData.ExpectedRuleSymbols.Count ),
            "Number of actual symbols is not equal to the expected number\n"
            + $"ActualSymbols: {SerializeRuleSymbols( actualSymbols )}\n"
            + $"ExpectedSymbols: {SerializeRuleSymbols( testData.ExpectedRuleSymbols )}" );

        foreach ( RuleSymbol actualSymbol in actualSymbols )
        {
            bool hasSymbol = testData.ExpectedRuleSymbols.Contains( actualSymbol );
            Assert.IsTrue(
                hasSymbol,
                "Symbol was not found\n"
                + $"Symbol: \"{actualSymbol}\"\n"
                + $"ActualSymbols: {SerializeRuleSymbols( actualSymbols )}\n"
                + $"ExpectedSymbols: {SerializeRuleSymbols( testData.ExpectedRuleSymbols )}" );
        }
    }

    private string SerializeRuleSymbols( IEnumerable<RuleSymbol> items )
    {
        return String.Join( ",", items.Select( x => $"\"{x}\"" ) );
    }
}