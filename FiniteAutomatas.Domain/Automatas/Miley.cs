using FiniteAutomatas.Domain.ValueObjects;

namespace FiniteAutomatas.Domain.Automatas;

public class Miley
{
    /// <summary> a, b, c, ... </summary>
    public readonly HashSet<Argument> Alphabet;
    
    /// <summary> q0, q1, q2, ... </summary>
    public readonly HashSet<State> AllStates;
    
    /// <summary> q0->q1: a, q1->q0:Eps, ... </summary>
    public readonly HashSet<Transition> Transitions;

    public Miley(IEnumerable<Transition> transitions)
    {
        Transitions = transitions.ToHashSet();
        if (Transitions.Any(x => x.AdditionalData == null))
        {
            throw new ArgumentException(nameof(Transitions));
        }

        AllStates = Transitions.SelectMany(x => new[] { x.From, x.To }).ToHashSet();
        Alphabet = Transitions.Select(x => x.Argument).ToHashSet();
    }
}