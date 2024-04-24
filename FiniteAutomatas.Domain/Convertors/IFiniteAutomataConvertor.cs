using FiniteAutomatas.Domain.Models.Automatas;

namespace FiniteAutomatas.Domain.Convertors;

public interface IFiniteAutomataConvertor<TInputType, out TOutput>
{
    TOutput Convert( IFiniteAutomata<TInputType> automata );
}

public interface IAutomataConvertor<in TInput, TInputType, out TOutput> 
    where TInput : IFiniteAutomata<TInputType>
{
    TOutput Convert( TInput automata );
}