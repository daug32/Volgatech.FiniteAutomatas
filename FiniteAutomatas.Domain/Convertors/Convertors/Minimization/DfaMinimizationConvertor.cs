using FiniteAutomatas.Domain.Convertors.Convertors.Minimization.Implementation;
using FiniteAutomatas.Domain.Models.Automatas;

namespace FiniteAutomatas.Domain.Convertors.Convertors.Minimization;

public class DfaMinimizationConvertor<T> : IAutomataConvertor<DeterminedFiniteAutomata<T>, T, DeterminedFiniteAutomata<T>>
{
    public DeterminedFiniteAutomata<T> Convert( DeterminedFiniteAutomata<T> finiteAutomata )
    {
        DeterminedFiniteAutomata<T> dfa = new DfaMinimizer<T>( finiteAutomata ).Minimize();
        return dfa;
    }
}