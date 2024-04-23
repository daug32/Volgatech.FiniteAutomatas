using System.Diagnostics;
using FiniteAutomatas.Domain.Models.Automatas;
using FiniteAutomatas.Domain.Models.ValueObjects;
using FiniteAutomatas.RegularExpressions.Implementation.Models;
using FluentAssertions;

namespace FiniteAutomatas.RegularExpressions.Implementation.Utils;

internal class FiniteAutomataDictionary
{
    public static NonDeterminedFiniteAutomata Convert(
        RegexNode current,
        NonDeterminedFiniteAutomata? left,
        NonDeterminedFiniteAutomata? right)
    {
        RegexSymbol regexSymbol = current.Value;
        switch ( regexSymbol.Type )
        {
            case RegexSymbolType.Symbol:
                left.ThrowIfNotNull();
                right.ThrowIfNotNull();
                return ForSymbol( new Argument( regexSymbol.Value.ThrowIfNull()!.Value ) );
            
            case RegexSymbolType.ZeroOrMore:
                right.ThrowIfNotNull();
                return ForZeroOrMore( left );
            
            case RegexSymbolType.Or: return ForOr( left, right );
            case RegexSymbolType.And: return ForAnd( left, right );
            
            default: throw new UnreachableException();
        }
    }

    private static NonDeterminedFiniteAutomata ForSymbol( Argument argument )
    {
        var start = new State( new StateId( 0 ), isStart: true );
        var end = new State( new StateId( 1 ), isEnd: true );

        return new NonDeterminedFiniteAutomata(
            alphabet: new[] { argument },
            transitions: new[]
            {
                new Transition(
                    from: start.Id,
                    to: end.Id,
                    argument: argument )
            },
            allStates: new[] { start, end } );
    }

    private static NonDeterminedFiniteAutomata ForAnd(
        NonDeterminedFiniteAutomata? left,
        NonDeterminedFiniteAutomata? right )
    {
        left = left.ThrowIfNull();
        right = right.ThrowIfNull();
        
        int biggestLeft = FindMaxName( left.AllStates );
        int biggestRight = FindMaxName( right.AllStates );
        if ( biggestLeft > biggestRight )
        {
            UpdateNamesAndGetBiggest( biggestLeft + 1, right );
        }
        else
        {
            UpdateNamesAndGetBiggest( biggestRight + 1, left );
        }

        State leftOldEnd = left.AllStates.First( x => x.IsEnd );
        leftOldEnd.IsEnd = false;

        State rightOldStart = right.AllStates.First( x => x.IsStart );
        rightOldStart.IsStart = false;

        foreach ( Transition rightTransition in right.Transitions )
        {
            if ( rightTransition.From == rightOldStart.Id )
            {
                rightTransition.From = leftOldEnd.Id;
            }

            if ( rightTransition.To == rightOldStart.Id )
            {
                rightTransition.To = leftOldEnd.Id;
            }
        }

        var transitions = left.Transitions.Union( right.Transitions ).ToHashSet();
        
        return new NonDeterminedFiniteAutomata(
            transitions.Select( x => x.Argument ).ToHashSet(),
            transitions,
            left.AllStates.Union( right.AllStates ) );
    }

    private static NonDeterminedFiniteAutomata ForOr(
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
            endStateName = UpdateNamesAndGetBiggest( biggestLeft + 2, right ) + 1;
            UpdateNamesAndGetBiggest( 1, left );
        }
        else
        {
            endStateName = UpdateNamesAndGetBiggest( biggestRight + 2, left ) + 1;
            UpdateNamesAndGetBiggest( 1, right );
        }

        var states = new List<State>();
        var newStart = new State( new StateId( 0 ), true );
        states.Add( newStart );
        var newEnd = new State( new StateId( endStateName ), isEnd: true );
        states.Add( newEnd );

        var transitions = new List<Transition>();
        transitions.AddRange( left.Transitions );
        transitions.AddRange( right.Transitions );

        State leftOldStart = left.AllStates.First( x => x.IsStart );
        leftOldStart.IsStart = false;
        transitions.Add( new Transition( newStart.Id, to: leftOldStart.Id, argument: Argument.Epsilon ) );

        State rightOldStart = right.AllStates.First( x => x.IsStart );
        rightOldStart.IsStart = false;
        transitions.Add( new Transition( newStart.Id, to: rightOldStart.Id, argument: Argument.Epsilon ) );

        State leftOldEnd = left.AllStates.First( x => x.IsEnd );
        leftOldEnd.IsEnd = false;
        transitions.Add( new Transition( leftOldEnd.Id, to: newEnd.Id, argument: Argument.Epsilon ) );

        State rightOldEnd = right.AllStates.First( x => x.IsEnd );
        rightOldEnd.IsEnd = false;
        transitions.Add( new Transition( rightOldEnd.Id, to: newEnd.Id, argument: Argument.Epsilon ) );
        
        states.AddRange( left.AllStates );
        states.AddRange( right.AllStates );

        return new NonDeterminedFiniteAutomata(
            allStates: states,
            transitions: transitions,
            alphabet: transitions.Select( x => x.Argument ).ToHashSet() );
    }

    private static NonDeterminedFiniteAutomata ForZeroOrMore( NonDeterminedFiniteAutomata? left )
    {
        left = left.ThrowIfNull();
        
        // Prepare
        var newEndId = new StateId( UpdateNamesAndGetBiggest( 1, left ) + 1 );
        
        var transitions = left.Transitions.ToHashSet();
        var states = left.AllStates.ToHashSet();

        // Update current automata
        var newStart = new State( new StateId( 0 ), true );
        states.Add( newStart );
        
        State oldStart = states.First( x => x.IsStart );
        oldStart.IsStart = false;
        transitions.Add( new Transition( from: newStart.Id, to: oldStart.Id, argument: Argument.Epsilon ) );

        var newEnd = new State( newEndId, isEnd: true );
        states.Add( newEnd );
        
        State oldEnd = states.First( x => x.IsEnd );
        oldEnd.IsEnd = false;
        transitions.Add( new Transition( from: oldEnd.Id, to: newEnd.Id, argument: Argument.Epsilon ) );

        transitions.Add( new Transition( from: oldEnd.Id, to: oldStart.Id, argument: Argument.Epsilon ) );
        transitions.Add( new Transition( from: newStart.Id, to: newEnd.Id, argument: Argument.Epsilon ) );

        return new NonDeterminedFiniteAutomata( 
            transitions.Select( x => x.Argument ).ToHashSet(),
            transitions,
            states );
    }

    private static int UpdateNamesAndGetBiggest( int offset, IFiniteAutomata automata )
    {
        var max = 0;

        var oldStateIdToNewStateId = new Dictionary<StateId, StateId>();
        foreach ( State state in automata.AllStates )
        {
            var newId = new StateId( state.Id.Value + offset );
            oldStateIdToNewStateId[state.Id] = newId;
            state.Id = newId;
            
            max = max > newId.Value
                ? max
                : newId.Value;
        }

        foreach ( Transition transition in automata.Transitions )
        {
            transition.From = oldStateIdToNewStateId[transition.From];
            transition.To = oldStateIdToNewStateId[transition.To];
        }

        return max;
    }

    private static int FindMaxName( IEnumerable<State> states ) => states.Max( x => x.Id.Value );
}