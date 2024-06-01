using FiniteAutomatas.Domain.Models.ValueObjects;

namespace FiniteAutomatas.Domain.Models.Automatas;

public class DeterminedFiniteAutomata<T> : BaseAutomata<T>
{
    public DeterminedFiniteAutomata(
        IEnumerable<Argument<T>> alphabet,
        IEnumerable<Transition<T>> transitions,
        IEnumerable<State> allStates )
        : base( alphabet, transitions, allStates )
    {
        var statesTransitions = AllStates.ToDictionary(
            x => x.Id,
            _ => Alphabet.ToHashSet() );
        
        foreach ( var transition in Transitions )
        {
            if ( transition.Argument == Argument<T>.Epsilon )
            {
                throw new ArgumentException( "DFA automata must not have epsilon transitions" );
            }

            if ( !statesTransitions[transition.From].Contains( transition.Argument ) )
            {
                throw new ArgumentException( "DFA automata's state must contain only one transition per argument" );
            }

            statesTransitions[transition.From].Remove( transition.Argument );
        }
    }

    public override HashSet<StateId> Move( StateId from, Argument<T> argument )
    {
        return Transitions
            .Where( transition =>
                transition.Argument == argument && transition.From == from )
            .Select( transition => transition.To )
            .ToHashSet();
    }
}