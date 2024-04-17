namespace ReToDfa.FiniteAutomatas.ValueObjects;

public class Transition
{
    public State From;
    public AlphabetSymbol Argument;
    public State To;

    public Transition( State from, AlphabetSymbol argument, State to )
    {
        From = from;
        Argument = argument;
        To = to;
    }

    public override string ToString() => $"(f={From}, t={To}, arg: {Argument})";
}