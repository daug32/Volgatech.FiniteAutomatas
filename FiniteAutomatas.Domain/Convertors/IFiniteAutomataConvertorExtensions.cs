using FiniteAutomatas.Domain.Models.Automatas;

namespace FiniteAutomatas.Domain.Convertors;

// ReSharper disable once InconsistentNaming
public static class IFiniteAutomataConvertorExtensions
{
    public static TOutput Convert<TOutput, TAutomataType>(
        this IFiniteAutomata<TAutomataType> automata, 
        IFiniteAutomataConvertor<TAutomataType, TOutput> convertor )
    {
        return convertor.Convert( automata );
    }
    
    public static TOutput Convert<TInput, TInputType, TOutput>(
        this TInput automata, 
        IAutomataConvertor<TInput, TInputType, TOutput> convertor )
        where TInput : IFiniteAutomata<TInputType>
    {
        return convertor.Convert( automata );
    }
}