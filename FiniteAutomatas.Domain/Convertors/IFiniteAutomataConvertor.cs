using FiniteAutomatas.Domain.Models.Automatas;

namespace FiniteAutomatas.Domain.Convertors;

public interface IFiniteAutomataConvertor<out TOutput>
{
    TOutput Convert( IFiniteAutomata automata );
}

public interface IAutomataConvertor<in TInput, out TOutput> 
    where TInput : IFiniteAutomata
{
    TOutput Convert( TInput automata );
}