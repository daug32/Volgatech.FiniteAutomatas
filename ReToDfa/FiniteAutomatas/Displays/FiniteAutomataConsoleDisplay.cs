using ConsoleTables;
using ReToDfa.FiniteAutomatas.ValueObjects;

namespace ReToDfa.FiniteAutomatas.Displays;

public static class FiniteAutomataConsoleDisplay
{
    public static void Print( FiniteAutomata automata )
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
        foreach ( State state in GetOrderedStates( automata.AllStates ) )
        {
            var items = new List<string>();
            
            foreach ( string column in columns )
            {
                if ( column == "Id" )
                {
                    items.Add( state.Name );
                    continue;
                }

                List<string> transitions = automata.Transitions
                    .Where( x =>
                        x.From.Equals( state ) &&
                        x.Argument == new AlphabetSymbol( column ) )
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
        result.AddRange( automata.Alphabet.Select( x => x.Value ) );

        return result;
    }

    private static List<State> GetOrderedStates( IEnumerable<State> allStates )
    {
        var states = allStates.ToList();
        states.Sort( ( a, b ) =>
        {
            if ( a.IsStart )
            {
                return -1;
            }

            if ( b.IsStart )
            {
                return 1;
            }

            if ( a.IsEnd )
            {
                return 1;
            }

            if ( b.IsEnd )
            {
                return -1;
            }

            return 0;
        } );

        return states;
    }
}