using FiniteAutomatas.Domain.Models.ValueObjects;

namespace FiniteAutomatas.Domain.Models.Automatas;

public class Mure : FiniteAutomata
{
    // q1:A1, q2:A1...
    public readonly Dictionary<State, State> Overrides;

    public Mure(
        Dictionary<State, State> overrides,
        ICollection<Transition> transitions )
        : base( transitions )
    {
        Overrides = overrides;
    }
}