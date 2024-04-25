using FiniteAutomatas.Domain.Models.Automatas;
using FiniteAutomatas.Visualizations.Implementation;

namespace FiniteAutomatas.Visualizations;

public static class Visualizer
{
    public static TAutomata PrintToImage<TAutomata, TAutomataType>( 
        this TAutomata automata,
        string path,
        VisualizationOptions? options = null )
        where TAutomata : IAutomata<TAutomataType>
    {
        Task task = new FiniteAutomataImageVisualizer<TAutomataType>( automata ).ToImage( path, options );
        task.Wait();

        return automata;
    }
    
    public static async Task<TAutomata> PrintToImageAsync<TAutomata, TAutomataType>( 
        this TAutomata automata,
        string path,
        VisualizationOptions? options = null )
        where TAutomata : IAutomata<TAutomataType>
    {
        await new FiniteAutomataImageVisualizer<TAutomataType>( automata ).ToImage( path, options ?? new VisualizationOptions() );
        return automata;
    }

    public static TAutomata PrintToConsole<TAutomata, TAutomataType>(
        this TAutomata automata,
        VisualizationOptions? options = null )
        where TAutomata : IAutomata<TAutomataType>
    {
        new FiniteAutomataConsoleVisualizer<TAutomataType>( automata ).Print( options ?? new VisualizationOptions() );
        return automata;
    }
}