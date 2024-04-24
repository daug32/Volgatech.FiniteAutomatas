using FiniteAutomatas.Domain.Models.ValueObjects;

namespace FiniteAutomatas.Domain.Models.Automatas;

public class NonDeterminedFiniteAutomata<T> : IFiniteAutomata<T>
{
    public IReadOnlyCollection<State> AllStates { get; }
    public IReadOnlyCollection<Argument<T>> Alphabet { get; }
    public IReadOnlyCollection<Transition<T>> Transitions { get; }

    public NonDeterminedFiniteAutomata( 
        IEnumerable<Argument<T>> alphabet,
        IEnumerable<Transition<T>> transitions, 
        IEnumerable<State> allStates )
    {
        Alphabet = alphabet.ToHashSet();
        AllStates = allStates.ToHashSet();
        Transitions = transitions.ToHashSet();
        
        foreach ( Transition<T> transition in Transitions )
        {
            if ( !AllStates.Any( x => x.Id == transition.From ) )
            {
                throw new ArgumentException( 
                    $"Some of the transitions has a state that is not presented in the {nameof( AllStates )}. " + 
                    $"Transition: {transition}. State: {transition.From}" );
            }

            if ( !AllStates.Any( x => x.Id == transition.To ) )
            {
                throw new ArgumentException( 
                    $"Some of the transitions has a state that is not presented in the {nameof( AllStates )}. " + 
                    $"Transition: {transition}. State: {transition.To}" );
            }

            if ( !Alphabet.Contains( transition.Argument ) )
            {
                throw new ArgumentException( 
                    $"Some of the transitions has an argument that is not presented in the {nameof( Alphabet )}. " + 
                    $"Transition: {transition}. Argument: {transition.Argument}" );
            }
        }
    }

    HashSet<StateId> IFiniteAutomata<T>.Move( StateId from, Argument<T> argument ) => Move( from, argument );

    public HashSet<StateId> Move( StateId from, Argument<T> argument, HashSet<StateId>? epsClosures = null )
    {
        epsClosures ??= EpsClosure( from ).ToHashSet();
        return Transitions
            .Where( transition =>
                transition.Argument == argument &&
                epsClosures.Contains( transition.From ) )
            .Select( transition => transition.To )
            .ToHashSet();
    }

    public HashSet<State> GetStates( HashSet<StateId> stateIds )
    {
        var result = new HashSet<State>();
        foreach ( State state in AllStates )
        {
            if ( stateIds.Contains( state.Id ) )
            {
                result.Add( state );
            }
        }

        if ( result.Count != stateIds.Count )
        {
            throw new ArgumentException( "Can't find states with given ids" );
        }

        return result;
    }

    public State GetState( StateId stateId )
    {
        return AllStates.Single( x => x.Id == stateId );
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
                Transition<T> transition = stateTransitions.Dequeue();
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