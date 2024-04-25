using FiniteAutomatas.Domain.Convertors.Convertors.Minimization.Implementation.Models;
using FiniteAutomatas.Domain.Models.Automatas;
using FiniteAutomatas.Domain.Models.ValueObjects;

namespace FiniteAutomatas.Domain.Convertors.Convertors.Minimization.Implementation;

internal class DfaMinimizer<T>
{
    private readonly DeterminedFiniteAutomata<T> _finiteAutomata;
    private readonly Dictionary<StateId, Dictionary<Argument<T>, StateId>> _transitions;
    
    public DfaMinimizer( DeterminedFiniteAutomata<T> finiteAutomata )
    {
        _finiteAutomata = finiteAutomata;

        _transitions = _finiteAutomata.AllStates.ToDictionary( x => x.Id, x => new Dictionary<Argument<T>, StateId>() );
        foreach ( Transition<T> transition in _finiteAutomata.Transitions )
        {
            if ( !_transitions.ContainsKey( transition.From ) )
            {
                _transitions[transition.From] = new Dictionary<Argument<T>, StateId>();
            }

            _transitions[transition.From].Add( transition.Argument, transition.To );
        }
    }
    
    public DeterminedFiniteAutomata<T> Minimize()
    {
        return MinimizationGroupsConvertor.ToFiniteAutomata(
            FindEquivalentStates(),
            _finiteAutomata.Transitions );
    }

    private List<MinimizationGroup> FindEquivalentStates()
    {
        List<MinimizationGroup> groups = MinimizationGroupsConvertor.ParseFromStates( _finiteAutomata.AllStates );

        bool hasChanges = true;
        while ( hasChanges )
        {
            var newGroups = new List<MinimizationGroup>();
            
            hasChanges = false;
            for ( int groupIndex = 0; groupIndex < groups.Count; groupIndex++ )
            {
                MinimizationGroup originalGroup = groups[groupIndex];
                MinimizationGroup currentGroup = originalGroup.Copy();
                newGroups.Add( currentGroup );
                
                var createdGroups = new List<MinimizationGroup>();

                State groupExample = currentGroup.GetStates().First();
                var statesToProcess = new Queue<State>( currentGroup.GetStates().GetRange( 1, currentGroup.Count - 1 ) );
                while ( statesToProcess.Any() )
                {
                    State current = statesToProcess.Dequeue();
                    if ( HasSameEquivalenceClass( groupExample, current, groups ) )
                    {
                        continue;
                    }

                    currentGroup.Remove( current.Id );

                    bool addedToGroup = false;
                    foreach ( MinimizationGroup createdGroup in createdGroups )
                    {
                        State example = createdGroup.GetStates().First();
                        if ( !HasSameEquivalenceClass( current, example, groups ) )
                        {
                            continue;
                        }

                        addedToGroup = true;
                        createdGroup.Add( current );
                        break;
                    }

                    if ( !addedToGroup )
                    {
                        createdGroups.Add( new MinimizationGroup( current ) );
                    }
                }

                if ( createdGroups.Any() )
                {
                    hasChanges = true;
                }
                
                newGroups.AddRange( createdGroups );
            }

            groups = newGroups;
        }

        return groups.Where( x => x.Any() ).ToList();
    }

    private bool HasSameEquivalenceClass(
        State first,
        State second,
        ICollection<MinimizationGroup> groups )
    {
        if ( first.IsError || second.IsError )
        {
            return false;
        }

        foreach ( Argument<T> argument in _finiteAutomata.Alphabet )
        {
            MinimizationGroup? firstGroup = groups.SingleOrDefault( x => x.Any( groupItem => groupItem.Id == _transitions[first.Id][argument] ) );
            if ( firstGroup is null )
            {
                return false;
            }
            
            MinimizationGroup? secondGroup = groups.SingleOrDefault( x => x.Any( groupItem => groupItem.Id == _transitions[second.Id][argument] ) );
            if ( secondGroup is null )
            {
                return false;
            }

            if ( !firstGroup.Equals( secondGroup ) )
            {
                return false;
            }
        }

        return true;
    }
}