using FiniteAutomatas.Domain.Convertors.Convertors.Minimization.Implementation;
using FiniteAutomatas.Domain.Models.Automatas;

namespace FiniteAutomatas.Domain.Convertors.Convertors.Minimization;

public class DfaMinimizationConvertor : IAutomataConvertor<DeterminedFiniteAutomata, DeterminedFiniteAutomata>
{
    public DeterminedFiniteAutomata Convert( DeterminedFiniteAutomata automata )
    {
        return new DfaMinimizer( automata ).Minimize();
    }
}