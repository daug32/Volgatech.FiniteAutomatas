using FiniteAutomatas.Domain.Automatas;

namespace FiniteAutomatas.Visualizations;

public class Visualizer
{
    private readonly FiniteAutomata _automata;
    
    public Visualizer( FiniteAutomata automata )
    {
        _automata = automata;
    }
}