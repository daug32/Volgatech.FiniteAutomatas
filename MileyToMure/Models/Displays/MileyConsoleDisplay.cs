using ConsoleTables;
using FiniteAutomatas.Domain.ValueObjects;

namespace MileyToMure.Models.Displays;

public static class MileyConsoleDisplay
{
    public static void Print(this Miley fa)
    {
        string[] columns = BuildColumns(fa).ToArray();
        List<string[]> rows = BuildRows(fa).ToList();
        
        var table = new ConsoleTable(columns);
        foreach (string[] row in rows)
        {
            table.AddRow( row );
        }
        
        table.Write();
    }

    private static IEnumerable<string[]> BuildRows(Miley fa)
    {
        foreach (Argument argument in fa.Alphabet)
        {
            var result = new List<string>();
            result.Add( argument.Value );

            foreach (State state in fa.AllStates.OrderBy(x => x.Name))
            {
                var transition = fa.Transitions.Single(x => x.From.Equals(state) && x.Argument.Equals(argument));
                result.Add( $"{transition.To.Name}/{transition.OutputSymbol}" );
            }
            
            yield return result.ToArray();
        }
        
    }

    private static IEnumerable<string> BuildColumns(Miley fa)
    {
        yield return "Id";

        foreach (var state in fa.AllStates.OrderBy(x => x.Name))
        {
            yield return state.Name;
        }
    }
}