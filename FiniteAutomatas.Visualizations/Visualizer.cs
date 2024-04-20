using System.Diagnostics;
using FiniteAutomatas.Domain.Models.Automatas;
using FiniteAutomatas.Domain.Models.ValueObjects;

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
        var nodes = _automata.AllStates.Select( x => $"{x.Name} [{BuildNodeStyles( x )}];" ).ToList();
        if ( _automata.AllStates.Any( x => x.IsError ) )
        {
            nodes.Add( "error [style=\"filled\" fillcolor=\"red\" label=\"error\"]" );
        }

        nodes.Add( "end [style=\"filled\" fillcolor=\"green\" label=\"end\"]" );
        nodes.Add( "start [style=\"filled\" fillcolor=\"#40b0f0\" label=\"start\"]" );
        var transitions = _automata.Transitions.Select( x => $"{x.From.Name} -> {x.To.Name} [label=\"{BuildTransitionLabel( x )}\"];" );

        string data = $@"
            digraph {_graphName} {{
	            rankdir=LR; 
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
            StartInfo = startInfo,
        };

        process.Start();
        process.WaitForExit();

        File.Delete( tempDataPath );
    }

    private static string BuildTransitionLabel( Transition transition )
    {
        return transition.Argument == Argument.Epsilon
            ? "Eps"
            : transition.Argument.Value;
    }

    private static string BuildNodeStyles( State x )
    {
        string style = "filled";
        string label = x.Name;
        string fillcolor = BuildFillColor( x );

        return $"style=\"{style}\" fillcolor=\"{fillcolor}\" label=\"{label}\"";
    }

    private static string BuildFillColor( State x )
    {
        if ( x.IsEnd && x.IsStart )
        {
            return "#008080";
        }
        
        if ( x.IsError && x.IsStart )
        {
            return "purple";
        }
        
        if ( x.IsStart )
        {
            // Blue
            return "#40b0f0";
        }

        if ( x.IsEnd )
        {
            return "green";
        }

        if ( x.IsError )
        {
            return "red";
        }

        return "white";
    }
}
