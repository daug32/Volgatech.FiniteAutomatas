using FiniteAutomatas.Domain.Models.ValueObjects;

namespace FiniteAutomatas.Domain.Models.Automatas;

public interface IFiniteAutomata
{
    IReadOnlyCollection<State> AllStates { get; }
    IReadOnlyCollection<Argument> Alphabet { get; }
    IReadOnlyCollection<Transition> Transitions { get; }

    HashSet<StateId> Move( StateId from, Argument argument );
    State GetState( StateId stateId );
    HashSet<State> GetStates( HashSet<StateId> stateIds );
}