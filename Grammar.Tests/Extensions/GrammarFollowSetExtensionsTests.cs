using Grammar.Parsers;
using Grammar.Parsers.Implementation;
using Grammar.Tests.Extensions.Containers;
using Grammars.Common;
using Grammars.Common.Grammars;
using Grammars.Common.Grammars.Extensions;
using Grammars.Common.Grammars.ValueObjects;
using Grammars.Common.Grammars.ValueObjects.Symbols;
using LinqExtensions;

namespace Grammar.Tests.Extensions;

public class GrammarFollowSetExtensionsTests
{
    private static readonly ParsingSettings _defaultSettings = new();

    private static readonly object[] _testData = new List<FirstFollowTestData>()
        .With( new FirstFollowTestData(
            new RuleName( "S" ),
            @"
                <S> -> <A>s$
                <A> -> a
            ",
            new[]
            {
                RuleSymbol.TerminalSymbol( TerminalSymbol.End() )
            } ) )
        .With( new FirstFollowTestData(
            new RuleName( "A" ),
            @"
                <S> -> <A>a$
                <A> -> b
            ",
            new[]
            {
                RuleSymbol.TerminalSymbol( TerminalSymbol.Word( "a" ) )
            } ) )
        .With( new FirstFollowTestData(
            new RuleName( "A" ),
            @"
                <S> -> <A>$
                <A> -> <A>a | ε
            ",
            new[]
            {
                RuleSymbol.TerminalSymbol( TerminalSymbol.Word( "a" ) ),
                RuleSymbol.TerminalSymbol( TerminalSymbol.End() ),
            } ) )
        .With( new FirstFollowTestData(
            new RuleName( "A" ),
            @"<A> -> <A>a$ | ε$",
            new[]
            {
                RuleSymbol.TerminalSymbol( TerminalSymbol.Word( "a" ) ),
                RuleSymbol.TerminalSymbol( TerminalSymbol.End() ),
            } ) )
        .With( new FirstFollowTestData(
            new RuleName( "A" ),
            @"<A> -> ε$ | <A>a$",
            new[]
            {
                RuleSymbol.TerminalSymbol( TerminalSymbol.Word( "a" ) ),
                RuleSymbol.TerminalSymbol( TerminalSymbol.End() ),
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
                RuleSymbol.TerminalSymbol( TerminalSymbol.End() )
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
                RuleSymbol.TerminalSymbol( TerminalSymbol.Word( "g" ) ),
                RuleSymbol.TerminalSymbol( TerminalSymbol.Word( "f" ) ),
                RuleSymbol.TerminalSymbol( TerminalSymbol.Word( "h" ) )
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
                RuleSymbol.TerminalSymbol( TerminalSymbol.Word( "h" ) )
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
                RuleSymbol.TerminalSymbol( TerminalSymbol.Word( "f" ) ),
                RuleSymbol.TerminalSymbol( TerminalSymbol.Word( "h" ) )
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
                RuleSymbol.TerminalSymbol( TerminalSymbol.Word( "h" ) )
            } ) )
        .With( new FirstFollowTestData(
            new RuleName( "E`" ),
            @"
                <E> -> <T><E`>$
                <E`> -> +<T><E`> | ε
                <T> -> <F><T'>
                <T'> -> *<F><T'> | ε
                <F> -> id | (<E>)
            ",
            new[]
            {
                RuleSymbol.TerminalSymbol( TerminalSymbol.End() ),
                RuleSymbol.TerminalSymbol( TerminalSymbol.Word( ")" ) ),
            } ) )
        .Cast<object>()
        .ToArray();

    [TestCaseSource( nameof( _testData ) )]
    public void FindFollowSet( FirstFollowTestData testData )
    {
        // Arrange
        CommonGrammar grammar = new GrammarInMemoryStringParser( testData.RawGrammar, _defaultSettings ).Parse();

        // Act
        var actualSymbols = grammar.GetFollowSet( testData.TargetRuleName ).GuidingSymbols.ToHashSet();

        // Assert
        Assert.That(
            actualSymbols.Count,
            Is.EqualTo( testData.ExpectedRuleSymbols.Count ),
            "Number of actual symbols is not equal to the expected number\n"
            + $"RuleName: {testData.TargetRuleName}\n"
            + $"ActualSymbols: {SerializeRuleSymbols( actualSymbols )}\n"
            + $"ExpectedSymbols: {SerializeRuleSymbols( testData.ExpectedRuleSymbols )}" );

        foreach ( RuleSymbol actualSymbol in actualSymbols )
        {
            bool hasSymbol = testData.ExpectedRuleSymbols.Contains( actualSymbol );
            Assert.IsTrue(
                hasSymbol,
                "Symbol was not found\n"
                + $"RuleName: {testData.TargetRuleName}\n"
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