using Grammar.Parsers;
using Grammar.Parsers.Implementation;
using Grammars.Common.Convertors;
using Grammars.Common.Grammars;
using Grammars.SLR.Convertors;

namespace Grammar.Tests.Convertors;

public class ToSlrOneGrammarConvertorTests
{
    private readonly ParsingSettings _settings = new();

    [TestCase( @"<S> -> a" )]
    [TestCase( @"<S> -> a | ε" )]
    [TestCase( @"
        <S> -> <A><B><C>
        <A> -> <A>a | ε
        <B> -> <B>b | ε
        <C> -> <C>c | ε
    " )]
    [TestCase( @"
        <S> -> <A><B><C>s
        <A> -> <A>a | ε
        <B> -> <B>b | ε
        <C> -> <C>c | ε
    " )]
    [TestCase( @"
        <S> -> <A><B><C>s
        <A> -> <A>a | a
        <B> -> <B>b | b
        <C> -> <C>c | c
    " )]
    [TestCase( @"
        <S> -> <A><B><C>s
        <A> -> a<A> | a
        <B> -> b<B> | b
        <C> -> c<C> | c
    " )]
    [TestCase( @"
        <S> -> common <A> end | <B> end
        <A> -> a
        <B> -> common b 
    " )]
    [TestCase( @"
        <A> -> <A>a | <B>b
        <B> -> b<B> | ε
    " )]
    public void Test( string rawGrammar )
    {
        // Arrange
        CommonGrammar grammar = new GrammarInMemoryStringParser( rawGrammar, _settings ).Parse();
        
        // Act & Assert
        Assert.DoesNotThrow( () => grammar.Convert( new ToSlrOneGrammarConvertor() ) );
    }
}