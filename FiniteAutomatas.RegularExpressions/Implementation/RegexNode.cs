namespace FiniteAutomatas.RegularExpressions.Implementation;

internal class RegexNode
{
    public readonly RegexNode? LeftOperand;
    public readonly RegexNode? RightOperand;
    
    public readonly RegexSymbol Value;

    public static RegexNode Parse( string expression )
    {
        if ( expression.Length == 0 )
        {
            throw new Exception();
        }

        expression = SimplifyBracesIfNeed( expression );
        if ( expression.Length == 1 )
        {
            expression = $"{expression}|{expression}";
        }
        
        return new RegexNode( RegexSymbol.Parse( expression ) );
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
                LeftOperand = new RegexNode( regex.GetRange( 0, regex.Count - 1 ) );
                return;
            }

            Value = regex.First();
            return;
        }

        var left = regex.GetRange( 0, symbolIndex );
        var right = regex.GetRange( symbolIndex + 1, regex.Count - symbolIndex - 1 );
        
        LeftOperand = left.Count > 0 ? new RegexNode( left ) : null;
        Value = regex[symbolIndex];
        RightOperand = right.Count > 0 ? new RegexNode( right ) : null;
    }

    public override string ToString() => $"(V={Value}, L={LeftOperand}, R={RightOperand})";

    private static string SimplifyBracesIfNeed( string expression )
    {
        while ( expression.First() == '(' && expression.Last() == ')' )
        {
            bool needToRemove = true;
            
            int bracesLevel = 0;
            for ( int i = 0; i < expression.Length && needToRemove; i++ )
            {
                if ( expression[i] == '(' )
                {
                    bracesLevel += 1;
                    continue;
                }

                if ( expression[i] == ')' )
                {
                    bracesLevel -= 1;
                    if ( bracesLevel == 0 && i + 1 < expression.Length - 1 )
                    {
                        needToRemove = false;
                    }
                }
            }

            if ( needToRemove )
            {
                expression = expression.Substring( 1, expression.Length - 2 );
            }
            else
            {
                break;
            }
        }

        return expression;
    }

    private static List<RegexSymbol> SimplifyBracesIfNeed( List<RegexSymbol> regex )
    {
        while ( regex.First().Type == RegexSymbolType.OpenBrace && regex.Last().Type == RegexSymbolType.CloseBrace )
        {
            bool needToRemove = true;
            
            int bracesLevel = 0;
            for ( int i = 0; i < regex.Count && needToRemove; i++ )
            {
                if ( regex[i].Type == RegexSymbolType.OpenBrace )
                {
                    bracesLevel += 1;
                    continue;
                }

                if ( regex[i].Type == RegexSymbolType.CloseBrace )
                {
                    bracesLevel -= 1;
                    if ( bracesLevel == 0 && i + 1 < regex.Count - 1 )
                    {
                        needToRemove = false;
                    }
                }
            }

            if ( needToRemove )
            {
                regex.RemoveAt( 0 );
                regex.RemoveAt( regex.Count - 1 );
            }
            else
            {
                break;
            }
        }

        return regex;
    }
}