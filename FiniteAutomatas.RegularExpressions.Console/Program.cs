using FiniteAutomatas.Domain.Convertors;
using FiniteAutomatas.Domain.Convertors.Convertors;
using FiniteAutomatas.Domain.Models.Automatas;
using FiniteAutomatas.Visualizations;

namespace FiniteAutomatas.RegularExpressions.Console;

public class Program
{
    private static readonly VisualizationOptions _visualizationOptions = new()
    {
        DrawErrorState = false
    };

    public static void Main()
    {
        while ( true )
        {
            System.Console.Write( "Write a regex: " );
            string regex = System.Console.ReadLine()!;
            
            try
            {
                System.Console.WriteLine( "Creating an NFA..." );
                FiniteAutomata nfa = new RegexToNfaParser()
                    .Parse( regex )
                    .PrintToConsole()
                    .PrintToImage( @"D:\Development\Projects\FiniteAutomatas\nfa.png", _visualizationOptions );

                System.Console.WriteLine( "Converting into DFA..." );
                FiniteAutomata dfa = nfa
                    .Convert( new NfaToDfaConvertor() )
                    .PrintToImage( @"D:\Development\Projects\FiniteAutomatas\dfa.png", _visualizationOptions )
                    .Convert( new DfaNormalizationConvertor() )
                    .PrintToConsole()
                    .PrintToImage( @"D:\Development\Projects\FiniteAutomatas\dfaMinimized.png", _visualizationOptions );
            }
            catch ( Exception ex )
            {
                System.Console.WriteLine( $"Couldn't create an NFA for regex. Regex: {regex}" );
                System.Console.WriteLine( ex );
            }
        }
        
        System.Console.WriteLine( "Press any key..." );
        System.Console.ReadKey();
    }
}