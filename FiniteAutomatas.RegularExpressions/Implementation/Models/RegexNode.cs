using FiniteAutomatas.RegularExpressions.Implementation.Extensions;

namespace FiniteAutomatas.RegularExpressions.Implementation.Models;

internal class RegexNode
{
    private RegexNode? _parent;
    
    public RegexNode? LeftOperand;
    public RegexNode? RightOperand;
    
    public RegexSymbol Value;

    public static RegexNode Parse( string expression )
    {
        if ( expression.Length == 0 )
        {
            throw new Exception();
        }
        
        return new RegexNode( 
            RegexSymbol.Parse( expression.Length == 1 ? $"{expression}|{expression}" : expression ),
            null );
    }
    
    private RegexNode( List<RegexSymbol> regex, RegexNode? parent )
    {
        _parent = parent;
        regex = regex.SimplifyBracesIfNeed();

        int symbolIndex = regex.GetOperationSymbolIndex();

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
                LeftOperand = new RegexNode( regex.GetRange( 0, regex.Count - 1 ), this );
                return;
            }

            Value = regex.First();
            return;
        }

        var left = regex.GetRange( 0, symbolIndex );
        var right = regex.GetRange( symbolIndex + 1, regex.Count - symbolIndex - 1 );
        
        Value = regex[symbolIndex];
        LeftOperand = left.Count > 0 ? new RegexNode( left, this ) : null;
        RightOperand = right.Count > 0 ? new RegexNode( right, this ) : null;
    }

    public RegexNode( RegexSymbol value, RegexNode? leftOperand, RegexNode? rightOperand )
    {
        Value = value;
        LeftOperand = leftOperand;
        RightOperand = rightOperand;
    }
    
    public RegexNode DeepCopy()
    {
        RegexNode? left = null;
        if ( LeftOperand != null) {
            left = LeftOperand.DeepCopy();
        }
        
        RegexNode? right = null;
        if ( RightOperand != null) {
            right = RightOperand.DeepCopy();
        }
        
        return new RegexNode( Value.Copy(), left, right);
    }

    public override string ToString() => $"(V={Value}, L={LeftOperand}, R={RightOperand})";
}