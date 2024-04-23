﻿using FiniteAutomatas.Domain.Convertors.Convertors.Minimization.Implementation.Models;
using FiniteAutomatas.Domain.Models.Automatas;
using FiniteAutomatas.Domain.Models.ValueObjects;

namespace FiniteAutomatas.Domain.Convertors.Convertors.Minimization.Implementation;

internal class DfaMinimizer
{
    private readonly DeterminedFiniteAutomata _automata;
    private readonly Dictionary<StateId, Dictionary<Argument, StateId>> _transitions;
    
    public DfaMinimizer( DeterminedFiniteAutomata automata )
    {
        _automata = automata;

        _transitions = _automata.AllStates.ToDictionary( x => x.Id, x => new Dictionary<Argument, StateId>() );
        foreach ( Transition transition in _automata.Transitions )
        {
            if ( !_transitions.ContainsKey( transition.From.Id ) )
            {
                _transitions[transition.From.Id] = new Dictionary<Argument, StateId>();
            }

            _transitions[transition.From.Id].Add( transition.Argument, transition.To.Id );
        }
    }
    
    public DeterminedFiniteAutomata Minimize()
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

        foreach ( Argument argument in _automata.Alphabet )
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