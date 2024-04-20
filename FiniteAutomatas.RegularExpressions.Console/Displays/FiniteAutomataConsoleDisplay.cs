using ConsoleTables;
using FiniteAutomatas.Domain.Models.Automatas;
using FiniteAutomatas.Domain.Models.ValueObjects;

namespace FiniteAutomatas.RegularExpressions.Console.Displays;

public static class FiniteAutomataConsoleDisplay
{
    public static void Print( this FiniteAutomata automata )
    {
        // Create columns
        string[] columns = BuildColumns( automata ).ToArray();
        
        // Create rows
        List<string>[] rows = BuildRows( automata, columns ).ToArray();

        var table = new ConsoleTable( columns );
        foreach ( var row in rows )
        {
            table.AddRow( row.ToArray() );
        }

        table.Write();
    }

    private static IEnumerable<List<string>> BuildRows( FiniteAutomata automata, string[] columns )
    {
        foreach ( State state in automata.AllStates.OrderBy( x => Int32.TryParse( x.Name, out int value )
                     ? value
                     : -1 ) )
        {
            var items = new List<string>();
            
            foreach ( string column in columns )
            {
                if ( column == "Id" )
                {
                    items.Add( state.Name );
                    continue;
                }

                if ( column == "IsStart" )
                {
                    items.Add( state.IsStart.ToString() );
                    continue;
                }

                if ( column == "IsEnd" )
                {
                    items.Add( state.IsEnd.ToString() );
                    continue;
                }

                List<string> transitions = automata.Transitions
                    .Where( x =>
                        x.From.Equals( state ) &&
                        x.Argument == new Argument( column ) )
                    .Select( x => x.To.Name )
                    .ToList();
                
                items.Add( String.Join( ",", transitions ) );
            }

            yield return items;
        }
    }

    private static IEnumerable<string> BuildColumns( FiniteAutomata automata )
    {
        var result = new List<string>();
        result.Add( "Id" );
        result.Add( "IsStart" );
        result.Add( "IsEnd" );
        result.AddRange( automata.Alphabet.Select( x => x.Value ) );

        return result;
    }
}