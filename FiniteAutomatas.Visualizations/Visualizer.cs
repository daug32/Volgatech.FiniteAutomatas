using FiniteAutomatas.Domain.Models.Automatas;
using FiniteAutomatas.Visualizations.Implementation;

namespace FiniteAutomatas.Visualizations;

public static class Visualizer
{
    public static T PrintToImage<T>( 
        this T automata,
        string path,
        VisualizationOptions? options = null )
        where T : FiniteAutomata
    {
        new ImageVisualizer( automata ).ToImage( path, options );
        return automata;
    }

    public static T PrintToConsole<T>(
        this T automata )
        where T : FiniteAutomata
    {
        new FiniteAutomataConsoleVisualizer( automata ).Print();
        return automata;
    }
}