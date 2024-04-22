using FiniteAutomatas.Domain.Convertors.Convertors.Minimization.Implementation;
using FiniteAutomatas.Domain.Models.Automatas;

namespace FiniteAutomatas.Domain.Convertors.Convertors.Minimization;

public class DfaMinimizationConvertor : IAutomataConvertor<FiniteAutomata>
{
    public FiniteAutomata Convert( FiniteAutomata automata )
    {
        return new DfaMinimizer( automata ).Minimize();
    }
}