using FiniteAutomatas.Domain.Models.Automatas;

namespace FiniteAutomatas.Domain.Convertors;

// ReSharper disable once InconsistentNaming
public static class IFiniteAutomataConvertorExtensions
{
    public static TOutput Convert<TOutput>(
        this IFiniteAutomata automata, 
        IFiniteAutomataConvertor<TOutput> convertor )
    {
        return convertor.Convert( automata );
    }
    
    public static TOutput Convert<TInput, TOutput>(
        this TInput automata, 
        IAutomataConvertor<TInput, TOutput> convertor )
        where TInput : IFiniteAutomata
    {
        return convertor.Convert( automata );
    }
}