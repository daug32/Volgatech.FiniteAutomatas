using System.Runtime.CompilerServices;
using FiniteAutomatas.Domain.Automatas;
using FiniteAutomatas.Domain.ValueObjects;

namespace FiniteAutomatas.Visualizations;

public class Visualizer
{
    private readonly FiniteAutomata _automata;

    public Visualizer( FiniteAutomata automata )
    {
        _automata = automata;
    }

    public void ToXml( string path )
    {
        var file = new StreamWriter( $"{path}.graphml" );
        
        file.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        file.WriteLine("<graphml>");
        file.WriteLine("<graph id=\"Graph\" uidGraph=\"3\" uidEdge=\"10010\">");

        var lastNodeId = 0;
        var stateToNodeId = new Dictionary<State, int>();
        foreach ( State state in _automata.AllStates )
        {
            file.WriteLine( 
                $"<node " + 
                    $"id=\"{lastNodeId}\" " + 
                    $"mainText=\"{state.Name}\" " + 
                    "upText=\"\" " + 
                    "size=\"30\">" + 
                "</node>" );    
            stateToNodeId[state] = lastNodeId++;
        }

        var lastEdgeId = 1000;
        foreach ( Transition transition in _automata.Transitions )
        {
            int from = stateToNodeId[transition.From];
            int to = stateToNodeId[transition.To];
            string text = transition.Argument.Value;
            
            file.WriteLine( 
                $"<edge source=\"{from}\" " + 
                    $"target=\"{to}\" " + 
                    "isDirect=\"true\" " + 
                    $"id=\"{lastEdgeId++}\" " + 
                    $"text=\"{text}\" >" + 
                "</edge>" );
        }
        
        file.WriteLine("</graph>");
        file.WriteLine("</graphml>");
        
        file.Close();
    }
}