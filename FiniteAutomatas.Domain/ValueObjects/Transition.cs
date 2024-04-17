namespace FiniteAutomatas.Domain.ValueObjects;

public class Transition
{
    public State From { get; set; }
    public Argument Argument { get; }
    public State To { get; set; }
    
    public string? AdditionalData { get; }

    public Transition(
        State from,
        Argument argument,
        State to,
        string? additionalData = null)
    {
        From = from;
        Argument = argument;
        To = to;
        AdditionalData = additionalData;
    }
}