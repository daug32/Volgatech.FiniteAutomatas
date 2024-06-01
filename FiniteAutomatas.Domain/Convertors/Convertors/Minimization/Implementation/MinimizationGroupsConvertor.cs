using FiniteAutomatas.Domain.Convertors.Convertors.Minimization.Implementation.Models;
using FiniteAutomatas.Domain.Models.Automatas;
using FiniteAutomatas.Domain.Models.ValueObjects;
using FiniteAutomatas.Domain.Models.ValueObjects.Implementation;

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

    public static DeterminedFiniteAutomata<T> ToFiniteAutomata<T>(
        List<MinimizationGroup> groups,
        IEnumerable<Transition<T>> oldTransitions )
    {
        var stateIdIncrementer = new StateIdIncrementer( groups.SelectMany( x => x.GetStates() ) );
        var states = new Dictionary<StateId, State>( groups.Count );
        var oldStateNameToNewStateName = new Dictionary<StateId, StateId>();
        foreach ( MinimizationGroup group in groups )
        {
            StateId newName = stateIdIncrementer.Next();

            var isStart = false;
            var isEnd = false;
            var isError = false;
            foreach ( State state in group.GetStates() )
            {
                isError |= state.IsError;
                isEnd |= state.IsEnd;
                isStart |= state.IsStart;
                oldStateNameToNewStateName.Add( state.Id, newName );
            }

            var collapsedState = new State(
                newName,
                isStart,
                isEnd,
                isError );

            states.Add( collapsedState.Id, collapsedState );
        }

        var alphabet = new HashSet<Argument<T>>();
        var transitions = new HashSet<Transition<T>>();
        foreach ( Transition<T> oldTransition in oldTransitions )
        {
            State from = states[oldStateNameToNewStateName[oldTransition.From]];
            State to = states[oldStateNameToNewStateName[oldTransition.To]];
            Argument<T> argument = oldTransition.Argument;

            bool hasThisTransition = transitions.Any( transition =>
                transition.From == from.Id && 
                transition.To == to.Id &&
                transition.Argument == argument );
            if ( hasThisTransition )
            {
                continue;
            }

            transitions.Add( new Transition<T>( from.Id, to: to.Id, argument: argument ) );
            alphabet.Add( argument );
        }

        return new DeterminedFiniteAutomata<T>(
            alphabet,
            transitions,
            states.Values );
    }
}