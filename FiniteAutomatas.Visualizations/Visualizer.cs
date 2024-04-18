using System.Diagnostics;
using FiniteAutomatas.Domain.Automatas;
using FiniteAutomatas.Domain.ValueObjects;

namespace FiniteAutomatas.Visualizations;

public class Visualizer
{
    private readonly string _graphvizPath = "./Graphviz/bin/dot.exe";
    private readonly string _graphName = "graphName";

    private readonly FiniteAutomata _automata;

    public Visualizer( FiniteAutomata automata )
    {
        _automata = automata;
    }

    public void ToImage( string path )
    {
        var nodes = _automata.AllStates.Select( x => $"{x.Name} [style=\"filled\" fillcolor=\"{BuildColor( x )}\" label=\"{x.Name}\"];" );
        var transitions = _automata.Transitions.Select( x => $"{x.From.Name} -> {x.To.Name} [label=\"{x.Argument.Value}\"];" );

        string data = $@"
            digraph {_graphName} {{
                {{ {String.Join( "", nodes )} }}
                {{ {String.Join( "", transitions )} }}
            }}
        ";

        var tempDataPath = $"{path}.dot";
        File.WriteAllText( tempDataPath, data );

        var startInfo = new ProcessStartInfo
        {
            FileName = _graphvizPath,
            Arguments = $"-Tpng {tempDataPath} -o {path}",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = new Process
        {
            StartInfo = startInfo
        };

        process.Start();
        process.WaitForExit();

        File.Delete( tempDataPath );
    }

    private static string BuildColor( State x )
    {
        if ( x.IsEnd )
        {
            return "red";
        }

        if ( x.IsStart )
        {
            // Blue
            return "#40b0f0";
        }

        return "white";
    }
}
