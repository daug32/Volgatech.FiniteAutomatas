using Grammar.Common.Tests.Extensions.Containers;
using Grammar.Parsers;
using Grammar.Parsers.Implementation;
using Grammars.Common;
using Grammars.Common.Extensions.Grammar;
using Grammars.Common.ValueObjects;
using Grammars.Common.ValueObjects.Symbols;

namespace Grammar.Common.Tests.Extensions;

public class GrammarFollowSetExtensionsTests
{
    private static readonly ParsingSettings _defaultSettings = new();

    private static readonly object[] _testData =
    {
        new FirstFollowTestData(
            new RuleName( "S" ),
            @"<S> -> <A> s",
            new[]
            {
                RuleSymbol.TerminalSymbol( TerminalSymbol.End() )
            } ),
        new FirstFollowTestData(
            new RuleName( "A" ),
            @"
                <S> -> <A> a
                <A> -> b
            ",
            new[]
            {
                RuleSymbol.TerminalSymbol( TerminalSymbol.Word( "a" ) )
            } ),
        new FirstFollowTestData(
            new RuleName( "S" ),
            @"
                <S> -> a<B><D>h
                <B> -> c<C>
                <C> -> b<C> | ε
                <D> -> <E><F>
                <E> -> g | ε
                <F> -> f | ε
            ",
            new[]
            {
                RuleSymbol.TerminalSymbol( TerminalSymbol.EmptySymbol() )
            } ),
        new FirstFollowTestData(
            new RuleName( "E" ),
            @"
                <S> -> a<B><D>h
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
            } )
    };

    [TestCaseSource( nameof( _testData ) )]
    public void FindFollowSet( FirstFollowTestData testData )
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