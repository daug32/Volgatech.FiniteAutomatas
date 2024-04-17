using MileyToMure.Models.ValueObjects;

namespace MileyToMure.Models;

public class Miley
{
    public readonly HashSet<Argument> Alphabet;
    public readonly HashSet<State> AllStates;
    public readonly HashSet<Transition> Transitions;

    public Miley(IEnumerable<Transition> transitions)
    {
        Transitions = transitions.ToHashSet();
        if (Transitions.Any(x => x.OutputSymbol == null)) throw new ArgumentException(nameof(Transitions));

        AllStates = Transitions.SelectMany(x => new[] { x.From, x.To }).ToHashSet();
        Alphabet = Transitions.Select(x => x.Argument).ToHashSet();
    }
}