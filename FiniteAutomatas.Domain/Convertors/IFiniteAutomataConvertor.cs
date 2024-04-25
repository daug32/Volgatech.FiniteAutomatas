using FiniteAutomatas.Domain.Models.Automatas;

namespace FiniteAutomatas.Domain.Convertors;

public interface IFiniteAutomataConvertor<TInputType, out TOutput>
{
    TOutput Convert( IAutomata<TInputType> automata );
}

public interface IAutomataConvertor<in TInput, TInputType, out TOutput> 
    where TInput : IAutomata<TInputType>
{
    TOutput Convert( TInput automata );
}