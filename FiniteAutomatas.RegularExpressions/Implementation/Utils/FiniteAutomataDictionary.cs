using System.Diagnostics;
using FiniteAutomatas.Domain.Models.Automatas;
using FiniteAutomatas.Domain.Models.ValueObjects;
using FiniteAutomatas.RegularExpressions.Implementation.Models;
using FluentAssertions;

namespace FiniteAutomatas.RegularExpressions.Implementation.Utils;

internal class FiniteAutomataDictionary
{
    public static NonDeterminedFiniteAutomata<char> Convert(
        RegexNode current,
        NonDeterminedFiniteAutomata<char>? left,
        NonDeterminedFiniteAutomata<char>? right)
    {
        RegexSymbol regexSymbol = current.Value;
        switch ( regexSymbol.Type )
        {
            case RegexSymbolType.Symbol:
                left.ThrowIfNotNull();
                right.ThrowIfNotNull();
                return ForSymbol( new Argument<char>( regexSymbol.Value.ThrowIfNull()!.Value ) );
            
            case RegexSymbolType.ZeroOrMore:
                right.ThrowIfNotNull();
                return ForZeroOrMore( left );
            
            case RegexSymbolType.Or: return ForOr( left, right );
            case RegexSymbolType.And: return ForAnd( left, right );
            
            default: throw new UnreachableException();
        }
    }

    private static NonDeterminedFiniteAutomata<char> ForSymbol( Argument<char> argument )
    {
        var start = new State( new StateId( 0 ), isStart: true );
        var end = new State( new StateId( 1 ), isEnd: true );

        return new NonDeterminedFiniteAutomata<char>(
            alphabet: new[] { argument },
            transitions: new[]
            {
                new Transition<char>(
                    from: start.Id,
                    to: end.Id,
                    argument: argument )
            },
            allStates: new[] { start, end } );
    }

    private static NonDeterminedFiniteAutomata<char> ForAnd(
        NonDeterminedFiniteAutomata<char>? left,
        NonDeterminedFiniteAutomata<char>? right )
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

        foreach ( Transition<char> rightTransition in right.Transitions )
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
        
        return new NonDeterminedFiniteAutomata<char>(
            transitions.Select( x => x.Argument ).ToHashSet(),
            transitions,
            left.AllStates.Union( right.AllStates ) );
    }

    private static NonDeterminedFiniteAutomata<char> ForOr(
        NonDeterminedFiniteAutomata<char>? left,
        NonDeterminedFiniteAutomata<char>? right )
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

        var transitions = new List<Transition<char>>();
        transitions.AddRange( left.Transitions );
        transitions.AddRange( right.Transitions );

        State leftOldStart = left.AllStates.First( x => x.IsStart );
        leftOldStart.IsStart = false;
        transitions.Add( new Transition<char>( newStart.Id, to: leftOldStart.Id, argument: Argument<char>.Epsilon ) );

        State rightOldStart = right.AllStates.First( x => x.IsStart );
        rightOldStart.IsStart = false;
        transitions.Add( new Transition<char>( newStart.Id, to: rightOldStart.Id, argument: Argument<char>.Epsilon ) );

        State leftOldEnd = left.AllStates.First( x => x.IsEnd );
        leftOldEnd.IsEnd = false;
        transitions.Add( new Transition<char>( leftOldEnd.Id, to: newEnd.Id, argument: Argument<char>.Epsilon ) );

        State rightOldEnd = right.AllStates.First( x => x.IsEnd );
        rightOldEnd.IsEnd = false;
        transitions.Add( new Transition<char>( rightOldEnd.Id, to: newEnd.Id, argument: Argument<char>.Epsilon ) );
        
        states.AddRange( left.AllStates );
        states.AddRange( right.AllStates );

        return new NonDeterminedFiniteAutomata<char>(
            allStates: states,
            transitions: transitions,
            alphabet: transitions.Select( x => x.Argument ).ToHashSet() );
    }

    private static NonDeterminedFiniteAutomata<char> ForZeroOrMore( NonDeterminedFiniteAutomata<char>? left )
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
        transitions.Add( new Transition<char>( from: newStart.Id, to: oldStart.Id, argument: Argument<char>.Epsilon ) );

        var newEnd = new State( newEndId, isEnd: true );
        states.Add( newEnd );
        
        State oldEnd = states.First( x => x.IsEnd );
        oldEnd.IsEnd = false;
        transitions.Add( new Transition<char>( from: oldEnd.Id, to: newEnd.Id, argument: Argument<char>.Epsilon ) );

        transitions.Add( new Transition<char>( from: oldEnd.Id, to: oldStart.Id, argument: Argument<char>.Epsilon ) );
        transitions.Add( new Transition<char>( from: newStart.Id, to: newEnd.Id, argument: Argument<char>.Epsilon ) );

        return new NonDeterminedFiniteAutomata<char>( 
            transitions.Select( x => x.Argument ).ToHashSet(),
            transitions,
            states );
    }

    private static int UpdateNamesAndGetBiggest( int offset, IFiniteAutomata<char> automata )
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

        foreach ( Transition<char> transition in automata.Transitions )
        {
            transition.From = oldStateIdToNewStateId[transition.From];
            transition.To = oldStateIdToNewStateId[transition.To];
        }

        return max;
    }

    private static int FindMaxName( IEnumerable<State> states ) => states.Max( x => x.Id.Value );
}