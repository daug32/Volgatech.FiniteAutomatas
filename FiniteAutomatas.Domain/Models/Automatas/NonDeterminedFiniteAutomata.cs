using FiniteAutomatas.Domain.Models.ValueObjects;

namespace FiniteAutomatas.Domain.Models.Automatas;

public class NonDeterminedFiniteAutomata<T> : BaseAutomata<T>
{
    public NonDeterminedFiniteAutomata(
        IEnumerable<Argument<T>> alphabet,
        IEnumerable<Transition<T>> transitions,
        IEnumerable<State> allStates )
        : base( alphabet, transitions, allStates )
    {
    }

    public override HashSet<StateId> Move( StateId from, Argument<T> argument )
    {
        return Move( from, argument );
    }

    public HashSet<StateId> Move( StateId from, Argument<T> argument, HashSet<StateId>? epsClosures = null )
    {
        epsClosures ??= EpsClosure( from ).ToHashSet();
        return Transitions
            .Where( transition =>
                transition.Argument == argument && epsClosures.Contains( transition.From ) )
            .Select( transition => transition.To )
            .ToHashSet();
    }

    public Dictionary<StateId, HashSet<StateId>> EpsClosure()
    {
        var result = new Dictionary<StateId, HashSet<StateId>>();
        foreach ( State state in AllStates )
        {
            result[state.Id] = EpsClosure( state.Id );
        }

        return result;
    }

    public HashSet<StateId> EpsClosure( StateId from )
    {
        var closures = new HashSet<StateId>
        {
            from
        };

        var statesToProcess = new Queue<StateId>();
        var processedStates = new HashSet<StateId>();
        statesToProcess.Enqueue( from );

        while ( statesToProcess.Any() )
        {
            StateId state = statesToProcess.Dequeue();
            if ( processedStates.Contains( state ) )
            {
                continue;
            }

            processedStates.Add( state );

            var stateTransitions = new Queue<Transition<T>>( Transitions.Where( transition => transition.From == state ) );

            while ( stateTransitions.Any() )
            {
                var transition = stateTransitions.Dequeue();
                if ( transition.Argument != Argument<T>.Epsilon )
                {
                    continue;
                }

                closures.Add( transition.To );
                statesToProcess.Enqueue( transition.To );
            }
        }

        return closures;
    }
}