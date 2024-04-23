using FiniteAutomatas.Domain.Models.ValueObjects;

namespace FiniteAutomatas.Domain.Models.Automatas;

public interface IFiniteAutomata
{
    HashSet<State> AllStates { get; }
    HashSet<Argument> Alphabet { get; }
    HashSet<Transition> Transitions { get; }

    HashSet<State> Move( State from, Argument argument );
}