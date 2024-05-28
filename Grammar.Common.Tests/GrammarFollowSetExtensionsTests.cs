using Grammar.Parsers;
using Grammar.Parsers.Implementation;
using Grammars.Common;
using Grammars.Common.Extensions.Grammar;
using Grammars.Common.ValueObjects;
using Grammars.Common.ValueObjects.Symbols;

namespace Grammar.Common.Tests;

public class GrammarFollowSetExtensionsTests
{
    [TestCase(
        @"<S> -> <A> s",
        "S",
        new[] { "$" } )]
    [TestCase(
        @"
            <S> -> <A> a
            <A> -> b
        ",
        "A",
        new[] { "a" } )]
    [TestCase(
        @"
            <S> -> a<B><D>h
            <B> -> c<C>
            <C> -> b<C> | ε
            <D> -> <E><F>
            <E> -> g | ε
            <F> -> f | ε
        ",
        "S",
        new[] { "ε" } )]
    [TestCase(
        @"
            <S> -> a<B><D>h
            <B> -> c<C>
            <C> -> b<C> | ε
            <D> -> <E><F>
            <E> -> g | ε
            <F> -> f | ε
        ",
        "E",
        new[] { "f", "h" } )]
    public void FindFollowSet(
        string rawGrammar,
        string rawRuleName,
        string[] rawExpectedSymbols )
    {
        // Arrange
        var targetRuleName = new RuleName( rawRuleName );
        CommonGrammar grammar = new GrammarInMemoryStringParser( rawGrammar, new ParsingSettings() ).Parse();
        var expectedSymbols = ConvertToRuleSymbols( rawExpectedSymbols );

        // Act
        var actualSymbols = grammar.GetFirstSet( targetRuleName ).GuidingSymbols.ToHashSet();

        // Assert
        Assert.That(
            actualSymbols.Count,
            Is.EqualTo( expectedSymbols.Count ),
            "Number of actual symbols is not equal to the expected number. "
            + $"ActualSymbols: {SerializeRuleSymbols( actualSymbols )}"
            + $"ExpectedSymbols: {SerializeRuleSymbols( expectedSymbols )}" );

        foreach ( RuleSymbol actualSymbol in actualSymbols )
        {
            bool hasSymbol = expectedSymbols.Contains( actualSymbol );
            Assert.IsTrue(
                hasSymbol,
                "Symbol was not found. "
                + $"Symbol: \"{actualSymbol}\" "
                + $"ActualSymbols: {SerializeRuleSymbols( actualSymbols )} "
                + $"ExpectedSymbols: {SerializeRuleSymbols( expectedSymbols )}" );
        }
    }

    private HashSet<RuleSymbol> ConvertToRuleSymbols( string[] symbols )
    {
        return symbols
            .Select( x =>
            {
                var defaultSettings = new ParsingSettings();
                if ( x == defaultSettings.EmptySymbol.ToString() )
                {
                    return RuleSymbol.TerminalSymbol( TerminalSymbol.EmptySymbol() );
                }

                if ( x == defaultSettings.EndSymbol.ToString() )
                {
                    return RuleSymbol.TerminalSymbol( TerminalSymbol.End() );
                }

                return RuleSymbol.TerminalSymbol( TerminalSymbol.Word( x ) );
            } )
            .ToHashSet();
    }

    private string SerializeRuleSymbols( IEnumerable<RuleSymbol> items )
    {
        return String.Join( ",", items.Select( x => $"\"{x}\"" ) );
    }
}