using System.Diagnostics;
using FiniteAutomatas.Domain.Models.Automatas;
using FiniteAutomatas.Domain.Models.ValueObjects;
using FiniteAutomatas.RegularExpressions.Implementation.Models;
using FiniteAutomatas.RegularExpressions.Implementation.Utils;
using FluentAssertions;

namespace FiniteAutomatas.RegularExpressions;

public class RegexToNfaParser
{
    public bool TryParse( string regex, out NonDeterminedFiniteAutomata? automata )
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

    public NonDeterminedFiniteAutomata Parse( string regex )
    {
        RegexNode node = RegexNode.Parse( regex );

        var stack = GetItemsToProcess( node );
        var nodesAutomatas = new Dictionary<RegexNode, NonDeterminedFiniteAutomata>();
        while ( stack.Any() )
        {
            RegexNode curr = stack.Pop();
            NonDeterminedFiniteAutomata automata = ConvertNodeToAutomata(
                curr,
                curr.LeftOperand != null
                    ? nodesAutomatas[curr.LeftOperand]
                    : null,
                curr.RightOperand != null
                    ? nodesAutomatas[curr.RightOperand]
                    : null );

            nodesAutomatas[curr] = automata;
        }

        return nodesAutomatas[node];
    }

    private NonDeterminedFiniteAutomata ConvertNodeToAutomata(
        RegexNode current,
        NonDeterminedFiniteAutomata? left,
        NonDeterminedFiniteAutomata? right )
    {
        RegexSymbol regexSymbol = current.Value;

        if ( regexSymbol.Type == RegexSymbolType.Symbol )
        {
            left.ThrowIfNotNull();
            right.ThrowIfNotNull();
            return FiniteAutomataDictionary.ForSymbol( new Argument( regexSymbol.Value.Value ) );
        }

        if ( regexSymbol.Type is RegexSymbolType.ZeroOrMore )
        {
            right.ThrowIfNotNull();
            return FiniteAutomataDictionary.ForZeroOrMore( left );
        }

        if ( regexSymbol.Type == RegexSymbolType.Or )
        {
            return FiniteAutomataDictionary.ForOr( left, right );
        }

        if ( regexSymbol.Type == RegexSymbolType.And )
        {
            return FiniteAutomataDictionary.ForAnd( left, right );
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
}