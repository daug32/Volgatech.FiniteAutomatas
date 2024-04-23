using FiniteAutomatas.Domain.Models.Automatas;
using FiniteAutomatas.Domain.Models.ValueObjects;
using FluentAssertions;

namespace FiniteAutomatas.RegularExpressions.Implementation.Utils;

internal class FiniteAutomataDictionary
{
    public static NonDeterminedFiniteAutomata ForSymbol( Argument argument )
    {
        var start = new State( new StateId( "0" ), isStart: true );
        var end = new State( new StateId( "1" ), isEnd: true );

        return new NonDeterminedFiniteAutomata(
            alphabet: new[] { argument },
            transitions: new[]
            {
                new Transition(
                    from: start,
                    to: end,
                    argument: argument )
            },
            allStates: new[] { start, end } );
    }

    public static NonDeterminedFiniteAutomata ForAnd(
        NonDeterminedFiniteAutomata? left,
        NonDeterminedFiniteAutomata? right )
    {
        left = left.ThrowIfNull();
        right = right.ThrowIfNull();
        
        int biggestLeft = FindMaxName( left.AllStates );
        int biggestRight = FindMaxName( right.AllStates );
        if ( biggestLeft > biggestRight )
        {
            UpdateNamesAndGetBiggest( biggestLeft + 1, right!.AllStates );
        }
        else
        {
            UpdateNamesAndGetBiggest( biggestRight + 1, left!.AllStates );
        }

        State leftOldEnd = left.AllStates.First( x => x.IsEnd );
        leftOldEnd.IsEnd = false;

        State rightOldStart = right.AllStates.First( x => x.IsStart );
        rightOldStart.IsStart = false;

        foreach ( Transition rightTransition in right.Transitions )
        {
            if ( rightTransition.From.Equals( rightOldStart ) )
            {
                rightTransition.From = leftOldEnd;
            }

            if ( rightTransition.To.Equals( rightOldStart ) )
            {
                rightTransition.To = leftOldEnd;
            }
        }

        right.AllStates.Remove( rightOldStart );

        var allTransitions = left.Transitions.Union( right.Transitions ).ToHashSet();
        return new NonDeterminedFiniteAutomata(
            allTransitions.Select( x => x.Argument ).ToHashSet(),
            allTransitions,
            left.AllStates.Union( right.AllStates ) );
    }

    public static NonDeterminedFiniteAutomata ForOr(
        NonDeterminedFiniteAutomata? left,
        NonDeterminedFiniteAutomata? right )
    {
        left = left.ThrowIfNull();
        right = right.ThrowIfNull();
        
        int biggestLeft = FindMaxName( left.AllStates );
        int biggestRight = FindMaxName( right.AllStates );

        int endStateName;
        if ( biggestLeft > biggestRight )
        {
            endStateName = UpdateNamesAndGetBiggest( biggestLeft + 2, right!.AllStates ) + 1;
            UpdateNamesAndGetBiggest( 1, left!.AllStates );
        }
        else
        {
            endStateName = UpdateNamesAndGetBiggest( biggestRight + 2, left!.AllStates ) + 1;
            UpdateNamesAndGetBiggest( 1, right!.AllStates );
        }
        
        var transitions = left.Transitions.Union( right.Transitions ).ToHashSet();

        var newStart = new State( new StateId( "0" ), true );
        left.AllStates.Add( newStart );

        var newEnd = new State( new StateId( endStateName.ToString() ), isEnd: true );
        left.AllStates.Add( newEnd );

        State leftOldStart = left.AllStates.First( x => x.IsStart );
        leftOldStart.IsStart = false;
        transitions.Add( new Transition( newStart, to: leftOldStart, argument: Argument.Epsilon ) );

        State rightOldStart = right.AllStates.First( x => x.IsStart );
        rightOldStart.IsStart = false;
        transitions.Add( new Transition( newStart, to: rightOldStart, argument: Argument.Epsilon ) );

        State leftOldEnd = left.AllStates.First( x => x.IsEnd );
        leftOldEnd.IsEnd = false;
        transitions.Add( new Transition( leftOldEnd, to: newEnd, argument: Argument.Epsilon ) );

        State rightOldEnd = right.AllStates.First( x => x.IsEnd );
        rightOldEnd.IsEnd = false;
        transitions.Add( new Transition( rightOldEnd, to: newEnd, argument: Argument.Epsilon ) );

        return new NonDeterminedFiniteAutomata(
            allStates: left.AllStates.Union( right.AllStates ),
            transitions: transitions,
            alphabet: transitions.Select( x => x.Argument ).ToHashSet() );
    }

    public static NonDeterminedFiniteAutomata ForZeroOrMore( NonDeterminedFiniteAutomata? left )
    {
        left = left.ThrowIfNull();

        int endStateName = UpdateNamesAndGetBiggest( 1, left.AllStates ) + 1;

        var newStart = new State( new StateId( "0" ), true );
        left.AllStates.Add( newStart );

        State oldStart = left.AllStates.First( x => x.IsStart );
        oldStart.IsStart = false;
        left.Transitions.Add( new Transition( from: newStart, to: oldStart, argument: Argument.Epsilon ) );

        var newEnd = new State( new StateId( endStateName.ToString() ), isEnd: true );
        left.AllStates.Add( newEnd );

        State oldEnd = left.AllStates.First( x => x.IsEnd );
        oldEnd.IsEnd = false;
        left.Transitions.Add( new Transition( from: oldEnd, to: newEnd, argument: Argument.Epsilon ) );

        left.Transitions.Add( new Transition( from: oldEnd, to: oldStart, argument: Argument.Epsilon ) );
        left.Transitions.Add( new Transition( from: newStart, to: newEnd, argument: Argument.Epsilon ) );

        return left;
    }

    private static int UpdateNamesAndGetBiggest( int offset, IEnumerable<State> states )
    {
        var max = 0;
        foreach ( State state in states )
        {
            int newName = Int32.Parse( state.Id.Value ) + offset;
            state.Id = new StateId( newName.ToString() );
            max = max > newName
                ? max
                : newName;
        }

        return max;
    }

    private static int FindMaxName( IEnumerable<State> states )
    {
        return states.Select( x => Int32.Parse( x.Id.Value ) ).Max();
    }
}