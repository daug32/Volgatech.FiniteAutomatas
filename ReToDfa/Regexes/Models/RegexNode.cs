using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace ReToDfa.Regexes.Models;

public class RegexNode
{
    public RegexNode? Parent;
    public RegexNode? LeftOperand;
    public RegexNode? RightOperand;
    
    public readonly RegexSymbol Value;

    public static RegexNode Parse( string expression )
    {
        Console.WriteLine( $"Creating a node. Expression: {expression}" );

        if ( expression.Length == 0 )
        {
            throw new Exception();
        }

        expression = SimplifyBracesIfNeed( expression );
        if ( expression.Length == 1 )
        {
            expression = $"{expression}|{expression}";
        }
        
        var regex = RegexSymbol.Parse( expression );
        Console.WriteLine( $"\t{String.Join( "_", regex )}" );

        return new RegexNode( regex );
    }
    
    private RegexNode( List<RegexSymbol> regex )
    {
        regex = SimplifyBracesIfNeed( regex );

        int symbolIndex = -1;
        var braces = new Queue<RegexSymbolType>();
        for ( int i = 0; i < regex.Count; i++ )
        {
            if ( regex[i].Type == RegexSymbolType.OpenBrace )
            {
                braces.Enqueue( regex[i].Type );
                continue;
            }

            if ( regex[i].Type == RegexSymbolType.CloseBrace )
            {
                braces.Dequeue();
                continue;
            }

            bool wedgeFound =
                !braces.Any()
                && regex[i].Type is 
                    not RegexSymbolType.Symbol and
                    not RegexSymbolType.OneOrMore and
                    not RegexSymbolType.ZeroOrMore; 
            if ( wedgeFound )
            {
                symbolIndex = i;
                break;
            } 
        }

        // No operators were found
        if ( symbolIndex == -1 )
        {
            if ( regex.Last().Type is 
                    RegexSymbolType.OneOrMore or 
                    RegexSymbolType.ZeroOrMore && 
                 regex.First().Type is
                     RegexSymbolType.OpenBrace or 
                     RegexSymbolType.Symbol)
            {
                Value = regex.Last();
            
                Console.WriteLine( $"\tLeft: {String.Join( "_", regex.GetRange( 0, regex.Count - 1 ) )}" );
                Console.WriteLine( $"\tCurrentOperator: {Value}" );
                Console.WriteLine( $"\tRight:" );
            
                LeftOperand = new RegexNode( regex.GetRange( 0, regex.Count - 1 ) );
                
                return;
            }

            Value = regex.First();
        
            Console.WriteLine( $"\tLeft:" );
            Console.WriteLine( $"\tCurrentOperator: {Value}" );
            Console.WriteLine( $"\tRight:" );
            
            return;
        }

        var left = regex.GetRange( 0, symbolIndex );
        var right = regex.GetRange( symbolIndex + 1, regex.Count - symbolIndex - 1 );
        Value = regex[symbolIndex];
        
        Console.WriteLine( $"\tLeft: {String.Join( "_", left)}" );
        Console.WriteLine( $"\tCurrentOperator: {Value}" );
        Console.WriteLine( $"\tRight: {String.Join( "_", right)}" );
        
        LeftOperand = left.Count > 0 ? new RegexNode( left ).WithParent( this ) : null;
        RightOperand = right.Count > 0 ? new RegexNode( right ).WithParent( this ) : null;
    }

    private static string SimplifyBracesIfNeed( string expression )
    {
        while ( expression.First() == '(' && expression.Last() == ')' )
        {
            expression = expression.Substring( 1, expression.Length - 2 );
            Console.WriteLine( $"\tBraces were removed. New expression: {expression}" );
        }

        return expression;
    }

    private static List<RegexSymbol> SimplifyBracesIfNeed( List<RegexSymbol> regex )
    {
        while ( regex.First().Type == RegexSymbolType.OpenBrace && regex.Last().Type == RegexSymbolType.CloseBrace )
        {
            regex.RemoveAt( 0 );
            regex.RemoveAt( regex.Count - 1 );
            Console.WriteLine( $"\tBraces were removed. New expression: {String.Join( "_", regex )}" );
        }

        return regex;
    }
}

public static class RegexNodeExtensions
{
    public static RegexNode? WithParent( this RegexNode? node, RegexNode parent )
    {
        if ( node is not null )
        {
            node.Parent = parent ?? throw new ArgumentException( nameof( parent ) );
        }

        return node;
    }
}