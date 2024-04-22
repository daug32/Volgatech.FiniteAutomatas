using FiniteAutomatas.Domain.Convertors;
using FiniteAutomatas.Domain.Convertors.Convertors;
using FiniteAutomatas.Domain.Convertors.Convertors.Minimization;
using FiniteAutomatas.Domain.Models.Automatas;
using FiniteAutomatas.Domain.Models.Automatas.Extensions;
using FiniteAutomatas.Domain.Models.ValueObjects;

namespace FiniteAutomatas.RegularExpressions.Tests;

public class RegexTests
{
    public class RegexTestData
    {
        public readonly string Regex;
        public readonly string[] SuccessTests;
        public readonly string[] FailTests;

        public RegexTestData( string regex, string[] successTests, string[] failTests )
        {
            Regex = regex;
            SuccessTests = successTests;
            FailTests = failTests;
        }
    }

    private static readonly List<RegexTestData> _regexTestData = new()
    {
        new RegexTestData( "a", new[] { "a" }, new[] { "", "ab", "b" } ),
        new RegexTestData( "(a)", new[] { "a" }, new[] { "", "ab", "b" } ),
        new RegexTestData( "((a))", new[] { "a" }, new[] { "", "ab", "b" } ),

        new RegexTestData( "ab", new[] { "ab" }, new[] { "", "ac", "ba", "a", "b" } ),
        new RegexTestData( "a(b)", new[] { "ab" }, new[] { "", "ac", "ba", "a", "b" } ),
        new RegexTestData( "abc", new[] { "abc" }, new[] { "", "a", "b", "c", "ab", "ac", "abd" } ),
        new RegexTestData( "(a)bc", new[] { "abc" }, new[] { "", "a", "b", "c", "ab", "ac", "abd" } ),
        new RegexTestData( "(ab)c", new[] { "abc" }, new[] { "", "a", "b", "c", "ab", "ac", "abd" } ),
        new RegexTestData( "a(bc)", new[] { "abc" }, new[] { "", "a", "b", "c", "ab", "ac", "abd" } ),

        new RegexTestData( "a*", new[] { "", "a", "aa" }, new[] { "ab", "b" } ),
        new RegexTestData( "a+", new[] { "a", "aa", "aaa" }, new[] { "", "ab", "b" } ),
        new RegexTestData( "a*b*c*", new[] { "abc", "aabbcc", "aaaa", "bbbb", "cccc", "" }, new[] { "d", "dddd", "abcabc" } ),
        new RegexTestData( "(x|y)*", new[] { "", "xxxx", "yyyy", "xyyyyxxyxx" }, new[] { "a", "bc" } ),

        new RegexTestData( "a|b", new[] { "a", "b" }, new[] { "abc", "", "d" } ),
        new RegexTestData( "a|bc|d", new[] { "a", "bc", "d" }, new[] { "ab", "ac", "ad", "", "bd", "abcd" } ),

        new RegexTestData(
            regex: "(x|y)*(ab|ac*)*|(x|y)*(a*b*c)*|(x|y)(ab)*",
            successTests: new[]
            {
                // (x|y)*(ab|ac*)*
                "xyabacc", "abacc", "ab", "a", "accc", "",
                // (x|y)*(a*b*c)*
                "xyaabbccc", "xy", "abc", "x", "ac", "bc", "c", "",
                // (x|y)(ab)*
                "xabab", "x", "y", "ab"
            },
            failTests: new[]
            {
                "b"
            }),

        new RegexTestData(
            regex: "(a*b*c*)*b(a*b*c*)*",
            successTests: new[]
            {
                "b",
                "abc",
                "abcb",
                "aabbccaabbccb",
                "acb",
                "babc"
            },
            failTests: new[]
            {
                "",
                "ac"
            } )
    };

    [TestCaseSource( nameof( _regexTestData ) )]
    public void Test( RegexTestData testData )
    {
        // Arrange
        FiniteAutomata dfa = null!;
        Assert.DoesNotThrow( 
            () => dfa = new RegexToNfaParser()
                .Parse( testData.Regex )
                .Convert( new NfaToDfaConvertor() )
                .Convert( new SetErrorStateOnEmptyTransitionsConvertor() )
                .Convert( new DfaMinimizationConvertor() ),
            $"Regex: {testData.Regex}" );

        // Act & Assert
        Assert.Multiple( () =>
        {
            foreach ( string successTest in testData.SuccessTests )
            {
                var result = FiniteAutomataRunResult.Unknown;
                Assert.DoesNotThrow(
                    () => result = dfa.RunForAllSymbols( successTest.Select( x => new Argument( x.ToString() ) ) ),
                    $"Regex: {testData.Regex}, Test: {successTest}" );
                Assert.That(
                    result.IsSuccess(),
                    Is.True,
                    $"Regex: {testData.Regex}, Test: {successTest}. Must be success" );
            }

            foreach ( string failTest in testData.FailTests )
            {
                var result = FiniteAutomataRunResult.Unknown;
                Assert.DoesNotThrow( 
                    () => result = dfa.RunForAllSymbols( failTest.Select( x => new Argument( x.ToString() ) ) ),
                    $"Regex: {testData.Regex}, Test: {failTest}" );
                Assert.That(
                    result.IsSuccess(),
                    Is.False,
                    $"Regex: {testData.Regex}, Test: {failTest}. Must be failed" );
            }
        } );
        
        Assert.Pass( $"Regex: {testData.Regex}" );
    }
}