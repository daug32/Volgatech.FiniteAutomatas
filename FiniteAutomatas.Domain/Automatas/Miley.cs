using FiniteAutomatas.Domain.ValueObjects;

namespace FiniteAutomatas.Domain.Automatas;

public class Miley
{
    public readonly HashSet<Argument> Alphabet;
    public readonly HashSet<State> AllStates;
    public readonly HashSet<Transition> Transitions;

    public Miley(IEnumerable<Transition> transitions)
    {
        Transitions = transitions.ToHashSet();
        if (Transitions.Any(x => x.AdditionalData == null)) throw new ArgumentException(nameof(Transitions));

        AllStates = Transitions.SelectMany(x => new[] { x.From, x.To }).ToHashSet();
        Alphabet = Transitions.Select(x => x.Argument).ToHashSet();
    }
}