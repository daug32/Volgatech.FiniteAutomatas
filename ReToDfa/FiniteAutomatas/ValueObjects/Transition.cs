namespace ReToDfa.FiniteAutomatas.ValueObjects;

public struct Transition
{
    public readonly State From;
    public readonly AlphabetSymbol Argument;
    public readonly State To;

    public Transition( State from, AlphabetSymbol argument, State to )
    {
        From = from;
        Argument = argument;
        To = to;
    }
}