using FiniteAutomatas.Domain.Models.Automatas;

namespace FiniteAutomatas.Domain.Convertors;

public static class FiniteAutomataConvertorExtensions
{
    public static TOutput Convert<TOutput>(
        this IFiniteAutomata automata, 
        IAutomataConvertor<TOutput> convertor )
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