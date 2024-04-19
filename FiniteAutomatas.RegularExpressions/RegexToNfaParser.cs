using System.Diagnostics;
using FiniteAutomatas.Domain.Automatas;
using FiniteAutomatas.Domain.ValueObjects;
using FiniteAutomatas.RegularExpressions.Implementation.Models;
using FiniteAutomatas.RegularExpressions.Implementation.Utils;

namespace FiniteAutomatas.RegularExpressions;

public class RegexToNfaParser
{
    public bool TryParse( string regex, out FiniteAutomata? automata )
    {
        try
        {
            automata = Parse( regex );
            return true;
        }
        catch
        {
            automata = null;
            return false;
        }
    }

    public FiniteAutomata Parse( string regex )
    {
        var alphabet = new HashSet<Argument>( regex.Select( x => new Argument( x.ToString() ) ) );
        alphabet.Add( Argument.Epsilon );
        alphabet.ExceptWith( RegexSymbolTypeHelper.SpecialSymbols.Select( x => new Argument( x.ToString() ) ) );

        RegexNode node = RegexNode.Parse( regex );

        var stack = GetItemsToProcess( node );
        var nodesAutomatas = new Dictionary<RegexNode, FiniteAutomata>();
        while ( stack.Any() )
        {
            RegexNode curr = stack.Pop();
            FiniteAutomata automata = ConvertNodeToAutomata(
                curr,
                curr.LeftOperand != null
                    ? nodesAutomatas[curr.LeftOperand]
                    : null,
                curr.RightOperand != null
                    ? nodesAutomatas[curr.RightOperand]
                    : null,
                alphabet.ToHashSet() );

            nodesAutomatas[curr] = automata;
        }

        return nodesAutomatas[node];
    }

    private FiniteAutomata ConvertNodeToAutomata(
        RegexNode current,
        FiniteAutomata? left,
        FiniteAutomata? right,
        HashSet<Argument> alphabet )
    {
        RegexSymbol regexSymbol = current.Value;

        if ( regexSymbol.Type == RegexSymbolType.Symbol )
        {
            return new FiniteAutomata( new[]
            {
                new Transition(
                    new State( "0", true ),
                    to: new State( "1", isEnd: true ),
                    argument: new Argument( regexSymbol.ToString() ) )
            } );
        }

        if ( regexSymbol.Type is RegexSymbolType.ZeroOrMore )
        {
            int endStateName = UpdateNamesAndGetBiggest( 1, left!.AllStates ) + 1;

            var newStart = new State( "0", true );
            left.AllStates.Add( newStart );

            State oldStart = left.AllStates.First( x => x.IsStart );
            oldStart.IsStart = false;
            left.Transitions.Add( new Transition( from: newStart, to: oldStart, argument: Argument.Epsilon ) );

            var newEnd = new State( endStateName.ToString(), isEnd: true );
            left.AllStates.Add( newEnd );

            State oldEnd = left.AllStates.First( x => x.IsEnd );
            oldEnd.IsEnd = false;
            left.Transitions.Add( new Transition( from: oldEnd, to: newEnd, argument: Argument.Epsilon ) );

            left.Transitions.Add( new Transition( from: oldEnd, to: oldStart, argument: Argument.Epsilon ) );
            left.Transitions.Add( new Transition( from: newStart, to: newEnd, argument: Argument.Epsilon ) );

            return left;
        }

        if ( regexSymbol.Type == RegexSymbolType.Or )
        {
            int biggestLeft = FindMaxName( left!.AllStates );
            int biggestRight = FindMaxName( right!.AllStates );

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

            var newStart = new State( "0", true );
            left.AllStates.Add( newStart );

            var newEnd = new State( endStateName.ToString(), isEnd: true );
            left.AllStates.Add( newEnd );

            State leftOldStart = left.AllStates.First( x => x.IsStart );
            leftOldStart.IsStart = false;
            left.Transitions.Add( new Transition( newStart, to: leftOldStart, argument: Argument.Epsilon ) );

            State rightOldStart = right.AllStates.First( x => x.IsStart );
            rightOldStart.IsStart = false;
            right.Transitions.Add( new Transition( newStart, to: rightOldStart, argument: Argument.Epsilon ) );

            State leftOldEnd = left.AllStates.First( x => x.IsEnd );
            leftOldEnd.IsEnd = false;
            left.Transitions.Add( new Transition( leftOldEnd, to: newEnd, argument: Argument.Epsilon ) );

            State rightOldEnd = right.AllStates.First( x => x.IsEnd );
            rightOldEnd.IsEnd = false;
            right.Transitions.Add( new Transition( rightOldEnd, to: newEnd, argument: Argument.Epsilon ) );

            return new FiniteAutomata(
                allStates: left.AllStates.Union( right.AllStates ),
                transitions: left.Transitions.Union( right.Transitions ),
                alphabet: alphabet );
        }

        if ( regexSymbol.Type == RegexSymbolType.And )
        {
            int biggestLeft = FindMaxName( left!.AllStates );
            int biggestRight = FindMaxName( right!.AllStates );
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

            return new FiniteAutomata(
                alphabet,
                left.Transitions.Union( right.Transitions ),
                left.AllStates.Union( right.AllStates ) );
        }

        throw new UnreachableException();
    }

    private static Stack<RegexNode> GetItemsToProcess( RegexNode node )
    {
        var stack = new Stack<RegexNode>();

        var queue = new Queue<RegexNode>();
        queue.Enqueue( node );

        while ( queue.Any() )
        {
            RegexNode curr = queue.Dequeue();

            if ( curr.Value.Type == RegexSymbolType.OneOrMore )
            {
                curr.RightOperand = new RegexNode(
                    value: new RegexSymbol( RegexSymbolType.ZeroOrMore ),
                    leftOperand: curr.LeftOperand!.DeepCopy(),
                    rightOperand: null );
                curr.Value = new RegexSymbol( RegexSymbolType.And );
            }

            if ( curr.LeftOperand is not null )
            {
                queue.Enqueue( curr.LeftOperand );
            }

            if ( curr.RightOperand is not null )
            {
                queue.Enqueue( curr.RightOperand );
            }

            stack.Push( curr );
        }

        return stack;
    }

    private int UpdateNamesAndGetBiggest( int offset, IEnumerable<State> states )
    {
        var max = 0;
        foreach ( State state in states )
        {
            int newName = Int32.Parse( state.Name ) + offset;
            state.Name = newName.ToString();
            max = max > newName
                ? max
                : newName;
        }

        return max;
    }

    private int FindMaxName( IEnumerable<State> states )
    {
        return states.Select( x => Int32.Parse( x.Name ) ).Max();
    }
}