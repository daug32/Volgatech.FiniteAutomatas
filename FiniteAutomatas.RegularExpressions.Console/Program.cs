using FiniteAutomatas.Domain.Convertors;
using FiniteAutomatas.Domain.Convertors.Convertors;
using FiniteAutomatas.Domain.Convertors.Convertors.Implementation;
using FiniteAutomatas.Domain.Models.Automatas;
using FiniteAutomatas.Visualizations;

namespace FiniteAutomatas.RegularExpressions.Console;

public class Program
{
    private static readonly VisualizationOptions _visualizationOptions = new()
    {
        DrawErrorState = true
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
                    .PrintToConsole( "DFA" )
                    .PrintToImage( @".\dfa1.png", _visualizationOptions )
                    
                    .Convert( new SetErrorStateOnEmptyTransitionsConvertor() )
                    .PrintToConsole( "Normalized DFA" )
                    .PrintToImage( @".\dfa2WithErrors.png", _visualizationOptions )
                    
                    .Convert( new DfaMinimizationConvertor() )
                    .PrintToConsole( "Minimized DFA" )
                    .PrintToImage( @".\dfa3Minimized.png", _visualizationOptions );
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