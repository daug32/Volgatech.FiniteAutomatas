using FiniteAutomatas.Domain.ValueObjects;

namespace MileyToMure.Models;

public class Mure
{
    // q1:A1, q2:A1...
    public readonly Dictionary<State, State> Overrides;
    public readonly HashSet<Transition> Transitions;
    public readonly HashSet<Argument> Alphabet;

    public Mure(Dictionary<State, State> overrides, IEnumerable<Transition> transitions)
    {
        Overrides = overrides;
        Transitions = transitions.ToHashSet();
        Alphabet = Transitions.Select(x => x.Argument).ToHashSet();
    }
}