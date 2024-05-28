using System.Diagnostics;
using FluentAssertions;

namespace Grammars.Common.ValueObjects.Symbols;

public class TerminalSymbol
{
    public readonly string? Value;
    public readonly TerminalSymbolType Type;

    public static TerminalSymbol EmptySymbol() => new( null, TerminalSymbolType.EmptySymbol );
    public static TerminalSymbol WhiteSpace() => new( null, TerminalSymbolType.WhiteSpace );
    public static TerminalSymbol End() => new( null, TerminalSymbolType.End );
    public static TerminalSymbol Word( string word ) => new( word.ThrowIfNullOrWhiteSpace(), TerminalSymbolType.Word );

    private TerminalSymbol( string? value, TerminalSymbolType type )
    {
        Value = value;
        Type = type;
    }
    
    public override bool Equals( object? obj ) =>
        obj is TerminalSymbol other &&
        other.Type == Type &&
        other.Value == Value;

    public override int GetHashCode() => HashCode.Combine( ( int )Type, Value );

    public static bool operator ==( TerminalSymbol? a, TerminalSymbol? b )
    {
        if ( a is null )
        {
            return b is null;
        }
        
        return a.Equals( b );
    }

    public static bool operator !=( TerminalSymbol? a, TerminalSymbol? b ) => !( a == b );

    public override string ToString() => Type switch
    {
        TerminalSymbolType.End => "<end/>",
        TerminalSymbolType.WhiteSpace => " ",
        TerminalSymbolType.Word => Value ?? throw new UnreachableException(),
        TerminalSymbolType.EmptySymbol => "<eps/>",
        _ => throw new ArgumentOutOfRangeException()
    };
}