namespace FiniteAutomatas.Domain.Models.ValueObjects;

public class TransitionsContainer
{
    private readonly Dictionary<string, Dictionary<Argument, string>> _transitions = new();
}