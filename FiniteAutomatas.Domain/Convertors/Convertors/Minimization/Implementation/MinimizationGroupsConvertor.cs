using FiniteAutomatas.Domain.Convertors.Convertors.Minimization.Implementation.Models;
using FiniteAutomatas.Domain.Models.Automatas;
using FiniteAutomatas.Domain.Models.ValueObjects;

namespace FiniteAutomatas.Domain.Convertors.Convertors.Minimization.Implementation;

internal static class MinimizationGroupsConvertor
{
    public static List<MinimizationGroup> ParseFromStates( IEnumerable<State> allStates )
    {
        var nonFinalStates = new MinimizationGroup();
        var finalStates = new MinimizationGroup();
        var errorStates = new MinimizationGroup();

        foreach ( State state in allStates )
        {
            if ( state.IsError )
            {
                errorStates.Add( state );
                continue;
            }

            if ( state.IsEnd )
            {
                finalStates.Add( state );
                continue;
            }
            
            nonFinalStates.Add( state );
        }

        return new[]
        {
            nonFinalStates,
            finalStates,
            errorStates
        }.Where( x => x.Any() ).ToList();
    }

    public static DeterminedFiniteAutomata ToFiniteAutomata(
        List<MinimizationGroup> groups,
        IEnumerable<Transition> oldTransitions )
    {
        var states = new Dictionary<StateName, State>( groups.Count );
        var oldStateNameToNewStateName = new Dictionary<StateName, StateName>();
        var index = 0;
        foreach ( MinimizationGroup group in groups )
        {
            var newName = new StateName( index++.ToString() );

            var isStart = false;
            var isEnd = false;
            var isError = false;
            foreach ( State state in group.GetStates() )
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
                x.From.Equals( from ) && 
                x.To.Equals( to ) &&
                x.Argument.Equals( argument ) );
            if ( hasThisTransition )
            {
                continue;
            }

            transitions.Add( new Transition( from, to: to, argument: argument ) );
            alphabet.Add( argument );
        }

        return new DeterminedFiniteAutomata(
            alphabet,
            transitions,
            states.Values );
    }
}