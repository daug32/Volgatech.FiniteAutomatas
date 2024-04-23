using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using FiniteAutomatas.Domain.Models.Automatas;
using FiniteAutomatas.Domain.Models.ValueObjects;

namespace FiniteAutomatas.Visualizations.Implementation;

public class FiniteAutomataImageVisualizer
{
    private readonly string _graphvizPath = "./Graphviz/bin/dot.exe";
    private readonly string _graphName = "graphName";
    
    private readonly IFiniteAutomata _automata;

    public FiniteAutomataImageVisualizer( IFiniteAutomata automata )
    {
        _automata = automata;
    }

    public async Task ToImage( string path, VisualizationOptions? options = null )
    {
        var tempDataPath = $"{path}.dot";

        await File.WriteAllTextAsync( 
            tempDataPath, 
            BuildData( options ) );

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _graphvizPath,
                Arguments = $"-Tpng {tempDataPath} -o {path}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            },
        };

        process.Start();

        Task work = process.WaitForExitAsync();

        var tasks = new List<Task>();
        tasks.Add( work );
        if ( options.TimeoutInMilliseconds.HasValue )
        {
            tasks.Add( Task.Delay( options.TimeoutInMilliseconds.Value ) );
        }

        await Task.WhenAny( tasks.ToArray() );
        
        if ( !work.IsCompleted )
        {
            process.Kill();
            await work;
            File.Delete( tempDataPath );
            throw new TimeoutException( "Graphviz: Waiting for graph building takes too long time" );
        }

        File.Delete( tempDataPath );
    }

    private string BuildData( VisualizationOptions options )
    {
        var statesToDraw = _automata.AllStates.ToHashSet();
        if ( !options.DrawErrorState )
        {
            statesToDraw = statesToDraw.Where( x => !x.IsError ).ToHashSet();
        }

        var dataBuilder = new StringBuilder();
        dataBuilder.Append( $"digraph {_graphName}" );
        dataBuilder.Append( "{" );
        dataBuilder.Append( "rankdir=LR;" );
        
        dataBuilder.Append( "{" );
        dataBuilder.AppendJoin( "", BuildNodes( statesToDraw ) );
        dataBuilder.Append( "}" );
        
        dataBuilder.Append( "{" );
        dataBuilder.AppendJoin( "", BuildTransitions( statesToDraw ) );
        dataBuilder.Append( "}" );
        
        dataBuilder.Append( "}" );
        
        return Regex.Replace( dataBuilder.ToString(), @"\s\s*", " " );
    }

    private IEnumerable<string> BuildTransitions( HashSet<State> statesToDraw )
    {
        foreach ( Transition transition in _automata.Transitions )
        {
            if ( !statesToDraw.Contains( transition.To ) )
            {
                continue;
            }

            string label = transition.Argument == Argument.Epsilon
                ? "Eps"
                : transition.Argument.Value;
            yield return $"{transition.From.Id} -> {transition.To.Id} [label=\"{label}\"];";
        }
    }

    private IEnumerable<string> BuildNodes( HashSet<State> statesToDraw )
    {
        if ( statesToDraw.Any( x => x.IsEnd ) )
        {
            yield return $"end [style=\"filled\" fillcolor=\"{Color.Green.ToHex()}\" label=\"end\" shape=\"doublecircle\"]";
        }

        if ( statesToDraw.Any( x => x.IsStart ) )
        {
            yield return $"start [style=\"filled\" fillcolor=\"{Color.DodgerBlue.ToHex()}\" label=\"start\" shape=\"circle\"]";
        }

        if ( statesToDraw.Any( x => x.IsError ) )
        {
            yield return $"error [style=\"filled\" fillcolor=\"{Color.Red.ToHex()}\" label=\"error\" shape=\"doublecircle\"]";
        }
        
        foreach ( State state in statesToDraw )
        {
            string style = "filled";
            string label = state.Id.ToString();
            string shape = state.IsTerminateState ? "doublecircle" : "circle";
            string fillcolor = BuildFillColor( state );

            yield return $"{state.Id} [style=\"{style}\" fillcolor=\"{fillcolor}\" label=\"{label}\" shape=\"{shape}\"];";
        }
    }

    private static string BuildFillColor( State state )
    {
        Color? color = null;
        if ( state.IsError )
        {
            color = Color.Red;
        }

        if ( state.IsEnd )
        {
            Color endColor = Color.Green;
            color = color?.Avg( endColor ) ?? endColor;
        }

        if ( state.IsStart )
        {
            Color startColor = Color.DodgerBlue;
            color = color?.Avg( startColor ) ?? startColor;
        }

        color ??= Color.White;

        return color.Value.ToHex();
    }
}