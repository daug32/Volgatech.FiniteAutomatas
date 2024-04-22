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
        Task task = new FiniteAutomataImageVisualizer( automata ).ToImage( path, options );
        task.Wait();

        return automata;
    }
    
    public static async Task<T> PrintToImageAsync<T>( 
        this T automata,
        string path,
        VisualizationOptions? options = null )
        where T : FiniteAutomata
    {
        await new FiniteAutomataImageVisualizer( automata ).ToImage( path, options );
        return automata;
    }

    public static T PrintToConsole<T>(
        this T automata,
        string? title = null )
        where T : FiniteAutomata
    {
        Console.WriteLine( title );
        new FiniteAutomataConsoleVisualizer( automata ).Print();
        return automata;
    }
}