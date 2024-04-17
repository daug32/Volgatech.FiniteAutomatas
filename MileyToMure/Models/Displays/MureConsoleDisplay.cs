using ConsoleTables;
using FiniteAutomatas.Domain.ValueObjects;

namespace MileyToMure.Models.Displays;

public static class MureConsoleDisplay
{
    public static void Print(this Mure fa)
    {
        string[] columns = BuildColumns(fa).ToArray();
        List<string[]> rows = BuildRows(fa).ToList();

        var table = new ConsoleTable(columns);
        foreach (string[] row in rows)
        {
            table.AddRow(row);
        }
        
        table.Write();
    }

    private static IEnumerable<string[]> BuildRows(Mure fa)
    {
        foreach (Argument argument in fa.Alphabet)
        {
            var result = new List<string>();
            result.Add( argument.Value );

            foreach (var statePair in fa.Overrides.OrderBy(x => x.Value).ThenBy(x => x.Key))
            {
                var transition = fa.Transitions.Single(x => 
                    x.From.Equals(statePair.Key) && 
                    x.Argument.Equals(argument));
                result.Add( $"{fa.Overrides[transition.To]}" );
            }
            
            yield return result.ToArray();
        }
    }

    private static IEnumerable<string> BuildColumns(Mure fa)
    {
        yield return "Id";

        foreach (var statePair in fa.Overrides.OrderBy(x => x.Value).ThenBy(x => x.Key))
        {
            yield return $"{statePair.Value.Name}/{statePair.Key.Name}";
        }
    }
}