using FiniteAutomatas.Domain.Models.Automatas;
using FiniteAutomatas.Domain.Models.ValueObjects;

namespace FiniteAutomatas.Domain.Convertors.Convertors.Implementation;

internal class DfaMinimizationConvertor : IAutomataConvertor<FiniteAutomata>
{
    public FiniteAutomata Convert( FiniteAutomata automata )
    {
        return BuildMinimizedDfa(
            CollapseStates( automata ),
            automata.Transitions );
    }

    private static List<List<State>> CollapseStates( FiniteAutomata automata )
    {
        var groups = new List<List<State>>()
        {
            automata.AllStates.Where( x => x.IsEnd ).ToList(),
            automata.AllStates.Where( x => !x.IsEnd && !x.IsError ).ToList(),
            automata.AllStates.Where( x => x.IsError ).ToList()
        }.Where( x => x.Any() ).ToList();

        var transitions = automata.AllStates.ToDictionary( x => x.Name, x => new Dictionary<Argument, string>() );
        foreach ( Transition transition in automata.Transitions )
        {
            if ( !transitions.ContainsKey( transition.From.Name ) )
            {
                transitions[transition.From.Name] = new Dictionary<Argument, string>();
            }

            transitions[transition.From.Name].Add( transition.Argument, transition.To.Name );
        }

        while ( true )
        {
            var hasChanges = false;
            for ( var groupIndex = 0; groupIndex < groups.Count; groupIndex++ )
            {
                var currentStatesGroup = groups[groupIndex];
                if ( currentStatesGroup.Count < 2 )
                {
                    continue;
                }

                for ( var itemIndex = 0; itemIndex < currentStatesGroup.Count - 1; itemIndex++ )
                {
                    State first = currentStatesGroup[itemIndex];
                    State second = currentStatesGroup[itemIndex + 1];

                    bool hasSameEquivalenceClass = HasSameEquivalenceClass(
                        first,
                        second,
                        transitions,
                        automata.Alphabet,
                        groups );

                    if ( hasSameEquivalenceClass )
                    {
                        continue;
                    }

                    hasChanges = true;

                    var hasAddedToNewGroup = false;
                    foreach ( var group in groups )
                    {
                        if ( group.Count == 0 )
                        {
                            continue;
                        }

                        if ( HasSameEquivalenceClass(
                                second,
                                group.First(),
                                transitions,
                                automata.Alphabet,
                                groups ) )
                        {
                            hasAddedToNewGroup = true;
                            group.Add( second );
                            break;
                        }
                    }

                    if ( !hasAddedToNewGroup )
                    {
                        groups.Add( new List<State>
                        {
                            second
                        } );
                    }

                    currentStatesGroup.Remove( second );
                }
            }

            if ( !hasChanges )
            {
                break;
            }
        }

        return groups.Where( x => x.Any() ).ToList();
    }

    private FiniteAutomata BuildMinimizedDfa(
        List<List<State>> groups,
        HashSet<Transition> oldTransitions )
    {
        var states = new Dictionary<string, State>( groups.Count );
        var oldStateNameToNewStateName = new Dictionary<string, string>();
        var index = 0;
        foreach ( var group in groups )
        {
            var newName = index++.ToString();

            var isStart = false;
            var isEnd = false;
            var isError = false;
            foreach ( State state in group )
            {
                isError |= state.IsError;
                isEnd |= state.IsEnd;
                isStart |= state.IsStart;
                oldStateNameToNewStateName.Add( state.Name, newName );
            }

            var collapsedState = new State(
                newName,
                isStart,
                isEnd,
                isError );

            states.Add( collapsedState.Name, collapsedState );
        }

        var alphabet = new HashSet<Argument>();
        var transitions = new HashSet<Transition>();
        foreach ( Transition transition in oldTransitions )
        {
            State from = states[oldStateNameToNewStateName[transition.From.Name]];
            State to = states[oldStateNameToNewStateName[transition.To.Name]];
            Argument argument = transition.Argument;

            bool hasThisTransition = transitions.Any( x =>
                x.From.Equals( from ) && x.To.Equals( to ) && x.Argument.Equals( argument ) );

            if ( hasThisTransition )
            {
                continue;
            }

            transitions.Add( new Transition( from, to: to, argument: argument ) );
            alphabet.Add( argument );
        }

        return new FiniteAutomata(
            alphabet,
            transitions,
            states.Values );
    }

    private static bool HasSameEquivalenceClass(
        State first,
        State second,
        Dictionary<string, Dictionary<Argument, string>> transitions,
        IEnumerable<Argument> alphabet,
        List<List<State>> groups )
    {
        if ( first.IsError || second.IsError )
        {
            return false;
        }

        foreach ( Argument argument in alphabet )
        {
            var firstGroup = groups.SingleOrDefault( x => 
                x.Any( groupItem => groupItem.Name == transitions[first.Name][argument] ) );
            var secondGroup = groups.SingleOrDefault( x => 
                x.Any( groupItem => groupItem.Name == transitions[second.Name][argument] ) );

            if ( firstGroup is not null &&
                 secondGroup is not null &&
                 firstGroup.Equals( secondGroup ) )
            {
                continue;
            }

            return false;
        }

        return true;
    }
}