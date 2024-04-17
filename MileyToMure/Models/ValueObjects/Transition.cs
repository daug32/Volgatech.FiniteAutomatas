namespace MileyToMure.Models.ValueObjects;

public class Transition
{
    public State From;
    public Argument Argument;
    public State To;
    public string? OutputSymbol;

    public Transition(
        State from,
        Argument argument,
        State to,
        string? outputSymbol = null)
    {
        From = from;
        Argument = argument;
        To = to;
        OutputSymbol = outputSymbol;
    }
}