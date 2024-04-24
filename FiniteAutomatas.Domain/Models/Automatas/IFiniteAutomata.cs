using FiniteAutomatas.Domain.Models.ValueObjects;

namespace FiniteAutomatas.Domain.Models.Automatas;

public interface IFiniteAutomata<T>
{
    IReadOnlyCollection<State> AllStates { get; }
    IReadOnlyCollection<Argument<T>> Alphabet { get; }
    IReadOnlyCollection<Transition<T>> Transitions { get; }

    HashSet<StateId> Move( StateId from, Argument<T> argument );
    State GetState( StateId stateId );
    HashSet<State> GetStates( HashSet<StateId> stateIds );
}