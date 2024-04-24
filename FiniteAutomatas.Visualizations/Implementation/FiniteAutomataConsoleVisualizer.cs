using ConsoleTables;
using FiniteAutomatas.Domain.Models.Automatas;
using FiniteAutomatas.Domain.Models.ValueObjects;

namespace FiniteAutomatas.Visualizations.Implementation;

internal class FiniteAutomataConsoleVisualizer<T>
{
    private readonly IFiniteAutomata<T> _automata;

    public FiniteAutomataConsoleVisualizer( IFiniteAutomata<T> automata )
    {
        _automata = automata;
    }

    public void Print( VisualizationOptions options )
    {
        // Create columns
        string[] columns = BuildColumns( options ).ToArray();

        // Create rows
        var rows = BuildRows( columns, options ).ToArray();

        var table = new ConsoleTable( columns );
        foreach ( var row in rows )
        {
            table.AddRow( row.ToArray() );
        }

        table.Write();
    }

    private IEnumerable<List<string>> BuildRows( string[] columns, VisualizationOptions options )
    {
        foreach ( State state in _automata.AllStates.OrderBy( x => x.Id.Value ) )
        {
            var items = new List<string>();

            foreach ( string column in columns )
            {
                if ( column == "Id" )
                {
                    items.Add( state.Id.ToString() );
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

                if ( options.DrawErrorState && column == "IsError" )
                {
                    items.Add( state.IsError.ToString() );
                    continue;
                }

                var transitions = _automata.Transitions
                    .Where( transition =>
                        transition.From == state.Id &&
                        transition.Argument.ToString() == column )
                    .Select( x =>
                    {
                        string transitionLabel = x.To.ToString(); 
                        if ( x.AdditionalData != null )
                        {
                            transitionLabel = $"{transitionLabel}/{x.AdditionalData}";
                        }

                        return transitionLabel;
                    } )
                    .ToList();

                items.Add( String.Join( ",", transitions ) );
            }

            yield return items;
        }
    }

    private IEnumerable<string> BuildColumns( VisualizationOptions options )
    {
        var result = new List<string>();
        result.Add( "Id" );
        result.Add( "IsStart" );
        result.Add( "IsEnd" );
        if ( options.DrawErrorState )
        {
            result.Add( "IsError" );
        }

        result.AddRange( _automata.Alphabet.Select( x => x.Value!.ToString() ).Order()! );

        return result;
    }
}