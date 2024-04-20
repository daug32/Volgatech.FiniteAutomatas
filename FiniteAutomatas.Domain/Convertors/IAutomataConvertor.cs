using FiniteAutomatas.Domain.Models.Automatas;

namespace FiniteAutomatas.Domain.Convertors;

public interface IAutomataConvertor<out TOutput>
{
    TOutput Convert( FiniteAutomata automata );
}

public interface IAutomataConvertor<in TInput, out TOutput> 
    where TInput : FiniteAutomata
{
    TOutput Convert( TInput automata );
}