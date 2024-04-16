using ReToDfa.FiniteAutomatas;

namespace ReToDfa;

public enum RegexSymbolType
{
    // a, 3, -...
    Symbol = 0,
    
    // ab
    And,

    // a | b
    Or,

    // a* 
    ZeroOrMore,

    // a+
    OneOrMore,

    // (
    OpenBrace,

    // )
    CloseBrace
}

public static class RegexSymbolTypeExtensions
{
    public static string ToSymbol( this RegexSymbolType type ) => type switch
    {
        RegexSymbolType.And => "&",
        RegexSymbolType.Or => "|",
        RegexSymbolType.ZeroOrMore => "*",
        RegexSymbolType.OneOrMore => "+",
        RegexSymbolType.OpenBrace => "(",
        RegexSymbolType.CloseBrace => ")",
        _ => throw new ArgumentOutOfRangeException( nameof( type ), type, null )
    };
}

public class RegexSymbol
{
    public readonly char? Value;
    public readonly RegexSymbolType Type;

    private RegexSymbol( char value )
    {
        Value = value;
        Type = RegexSymbolType.Symbol;
    }

    private RegexSymbol( RegexSymbolType symbolType )
    {
        Type = symbolType == RegexSymbolType.Symbol
            ? throw new InvalidOperationException()
            : symbolType;
    }

    public static List<RegexSymbol> Parse( string regex )
    {
        var result = new List<RegexSymbol>( regex.Length );

        for ( var i = 0; i < regex.Length; i++ )
        {
            RegexSymbolType currentSymbolType = ParseRegexSymbolType( regex[i] );
            RegexSymbolType? nextSymbolType = i + 1 < regex.Length
                ? ParseRegexSymbolType( regex[i + 1] )
                : null;
            
            if ( currentSymbolType is RegexSymbolType.Symbol )
            {
                result.Add( new RegexSymbol( regex[i] ) );

                if ( nextSymbolType is 
                    RegexSymbolType.Symbol or 
                    RegexSymbolType.OpenBrace )
                {
                    result.Add( new RegexSymbol( RegexSymbolType.And ) );
                }
            }
            else
            {
                result.Add( new RegexSymbol( currentSymbolType ) );

                if ( currentSymbolType is
                         RegexSymbolType.CloseBrace or 
                         RegexSymbolType.OneOrMore or 
                         RegexSymbolType.ZeroOrMore &&
                     nextSymbolType is
                         RegexSymbolType.Symbol )
                {
                    result.Add( new RegexSymbol( RegexSymbolType.And ) );
                    continue;
                }
            }
        }

        return result;
    }

    public override string ToString()
    {
        return Type == RegexSymbolType.Symbol
            ? Value!.ToString()!
            : Type.ToSymbol();
    }

    private static RegexSymbolType ParseRegexSymbolType( char c )
    {
        return c switch
        {
            '(' => RegexSymbolType.OpenBrace,
            ')' => RegexSymbolType.CloseBrace,
            '+' => RegexSymbolType.OneOrMore,
            '*' => RegexSymbolType.ZeroOrMore,
            '|' => RegexSymbolType.Or,
            _ => RegexSymbolType.Symbol
        };
    }
}

public class RegexNode
{
    public RegexNode? LeftOperand;
    public RegexNode? RightOperand;
    public RegexSymbol Operator;

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
    
    public RegexNode( List<RegexSymbol> regex )
    {
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
            Operator = regex.First();
            Console.WriteLine( $"\tLeaf was built. Operator: {Operator}" );
            return;
        }

        var left = regex.GetRange( 0, symbolIndex );
        RegexSymbol currentOperator = regex[symbolIndex];
        var right = regex.GetRange( symbolIndex + 1, regex.Count - symbolIndex - 1 );
        
        Console.WriteLine( $"\tLeft: {String.Join( "_", left)}" );
        Console.WriteLine( $"\tCurrentOperator: {currentOperator}" );
        Console.WriteLine( $"\tRight: {String.Join( "_", right)}" );
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
}

public class FiniteAutomataCreator
{
    public FiniteAutomata CreateFromRegex( string regex )
    {
        RegexNode node = RegexNode.Parse( $"{regex}" );

        return null;
    }
}