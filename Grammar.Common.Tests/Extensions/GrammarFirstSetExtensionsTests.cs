using Grammar.Common.Tests.Extensions.Containers;
using Grammar.Parsers;
using Grammar.Parsers.Implementation;
using Grammars.Common;
using Grammars.Common.Grammars;
using Grammars.Common.Grammars.Extensions;
using Grammars.Common.Grammars.ValueObjects;
using Grammars.Common.Grammars.ValueObjects.Symbols;

namespace Grammar.Common.Tests.Extensions;

public class GrammarFirstSetExtensionsTests
{
    private static readonly ParsingSettings _defaultSettings = new();

    private static readonly object[] _testCases = {
        new FirstFollowTestData(
            new RuleName( "A" ),
            @"
                <S> -> <A> s $
                <A> -> <B> a | a
                <B> -> b
            ",
            new[]
            {
                RuleSymbol.TerminalSymbol( TerminalSymbol.Word( "a" ) ),
                RuleSymbol.TerminalSymbol( TerminalSymbol.Word( "b" ) ), 
            } ),
        new FirstFollowTestData(
            new RuleName( "S" ),
            @"
                <S> -> <A>$
                <A> -> <B>
                <B> -> b
            ",
            new[] { RuleSymbol.TerminalSymbol( TerminalSymbol.Word( "b" ) ) } ),
        new FirstFollowTestData(
            new RuleName( "A" ),
            @"<A> -> <A>a | ε",
            new[]
            {
                RuleSymbol.TerminalSymbol( TerminalSymbol.Word( "a" ) ),
                RuleSymbol.TerminalSymbol( TerminalSymbol.EmptySymbol() ),
            } ),
        new FirstFollowTestData(
            new RuleName( "S" ),
            @"<S> -> s | ε",
            new[]
            {
                RuleSymbol.TerminalSymbol( TerminalSymbol.EmptySymbol() ),
                RuleSymbol.TerminalSymbol( TerminalSymbol.Word( "s" ) )
            } ),
        new FirstFollowTestData(
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
            } ),
    };

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