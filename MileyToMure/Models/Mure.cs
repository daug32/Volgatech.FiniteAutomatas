using MileyToMure.Models.ValueObjects;

namespace MileyToMure.Models;

public class Mure
{
    // q1 | A1, q2 | A1...
    public Dictionary<State, State> Overrides;
    public HashSet<Transition> Transitions;
    public HashSet<Argument> Alphabet;

    public Mure(Dictionary<State, State> overrides, IEnumerable<Transition> transitions)
    {
        Overrides = overrides;
        Transitions = transitions.ToHashSet();
        Alphabet = Transitions.Select(x => x.Argument).ToHashSet();
    }
}