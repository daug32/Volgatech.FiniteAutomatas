using FiniteAutomatas.Domain.Models.Automatas;
using FiniteAutomatas.Visualizations.Implementation;

namespace FiniteAutomatas.Visualizations;

public static class Visualizer
{
    public static T PrintToImage<T>( 
        this T automata,
        string path,
        VisualizationOptions? options = null )
        where T : DeterminedFiniteAutomata
    {
        Task task = new FiniteAutomataImageVisualizer( automata ).ToImage( path, options );
        task.Wait();

        return automata;
    }
    
    public static async Task<T> PrintToImageAsync<T>( 
        this T automata,
        string path,
        VisualizationOptions? options = null )
        where T : IFiniteAutomata
    {
        await new FiniteAutomataImageVisualizer( automata ).ToImage( path, options ?? new VisualizationOptions() );
        return automata;
    }

    public static T PrintToConsole<T>(
        this T automata,
        VisualizationOptions? options = null )
        where T : IFiniteAutomata
    {
        new FiniteAutomataConsoleVisualizer( automata ).Print( options ?? new VisualizationOptions() );
        return automata;
    }
}