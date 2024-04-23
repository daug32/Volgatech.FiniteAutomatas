using FiniteAutomatas.Domain.Models.ValueObjects;

namespace FiniteAutomatas.Domain.Models.Automatas;

public interface IFiniteAutomata
{
    IReadOnlySet<State> AllStates { get; }
    IReadOnlySet<Argument> Alphabet { get; }
    IReadOnlySet<Transition> Transitions { get; }

    HashSet<StateId> Move( StateId from, Argument argument );
    State GetState( StateId stateId );
    HashSet<State> GetStates( HashSet<StateId> stateIds );
}