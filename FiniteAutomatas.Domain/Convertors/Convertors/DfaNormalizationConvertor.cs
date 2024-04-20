using FiniteAutomatas.Domain.Convertors.Convertors.Implementation;
using FiniteAutomatas.Domain.Models.Automatas;

namespace FiniteAutomatas.Domain.Convertors.Convertors;

public class DfaNormalizationConvertor : IAutomataConvertor<FiniteAutomata>
{
    public FiniteAutomata Convert( FiniteAutomata automata )
    {
        return automata
            .Convert( new SetErrorStateOnEmptyTransitionsConvertor() )
            .Convert( new DfaMinimizationConvertor() );
    }
}