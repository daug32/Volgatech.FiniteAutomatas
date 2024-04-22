using FiniteAutomatas.Domain.Convertors.Convertors.Minimization.Implementation.Models;
using FiniteAutomatas.Domain.Models.Automatas;
using FiniteAutomatas.Domain.Models.ValueObjects;

namespace FiniteAutomatas.Domain.Convertors.Convertors.Minimization.Implementation;

internal class DfaMinimizer
{
    private readonly FiniteAutomata _automata;
    private readonly Dictionary<string, Dictionary<Argument, string>> _transitions;
    
    public DfaMinimizer( FiniteAutomata automata )
    {
        _automata = automata;

        _transitions = _automata.AllStates.ToDictionary( x => x.Name, x => new Dictionary<Argument, string>() );
        foreach ( Transition transition in _automata.Transitions )
        {
            if ( !_transitions.ContainsKey( transition.From.Name ) )
            {
                _transitions[transition.From.Name] = new Dictionary<Argument, string>();
            }

            _transitions[transition.From.Name].Add( transition.Argument, transition.To.Name );
        }
    }
    
    public FiniteAutomata Minimize()
    {
        return MinimizationGroupsConvertor.ToFiniteAutomata(
            FindEquivalentStates(),
            _automata.Transitions );
    }

    private List<MinimizationGroup> FindEquivalentStates()
    {
        List<MinimizationGroup> groups = MinimizationGroupsConvertor.ParseFromStates( _automata.AllStates );

        bool hasChanges = true;
        while ( hasChanges )
        {
            hasChanges = false;
            for ( int groupIndex = 0; groupIndex < groups.Count; groupIndex++ )
            {
                MinimizationGroup currentGroup = groups[groupIndex];
                
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

                    currentGroup.Remove( current.Name );

                    bool addedToGroup = false;
                    var allGroups = groups.Union( createdGroups ).ToList();
                    foreach ( MinimizationGroup createdGroup in createdGroups )
                    {
                        State example = createdGroup.GetStates().First();
                        if ( !HasSameEquivalenceClass( current, example, allGroups ) )
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
                
                groups.AddRange( createdGroups );
            }
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

        foreach ( Argument argument in _automata.Alphabet )
        {
            MinimizationGroup? firstGroup = groups.SingleOrDefault( x => x.Any( groupItem => groupItem.Name == _transitions[first.Name][argument] ) );
            if ( firstGroup is null )
            {
                return false;
            }
            
            MinimizationGroup? secondGroup = groups.SingleOrDefault( x => x.Any( groupItem => groupItem.Name == _transitions[second.Name][argument] ) );
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