using FiniteAutomatas.Domain.Convertors;
using FiniteAutomatas.Domain.Convertors.Convertors;
using FiniteAutomatas.Domain.Convertors.Convertors.Minimization;
using FiniteAutomatas.Domain.Models.Automatas;
using FiniteAutomatas.Visualizations;

namespace FiniteAutomatas.RegularExpressions.Console;

public class Program
{
    public static async Task Main()
    {
        while ( true )
        {
            System.Console.Write( "Write a regex: " );
            string regex = System.Console.ReadLine()!;
            
            try
            {
                System.Console.WriteLine( "Creating an NFA..." );
                FiniteAutomata nfa = await new RegexToNfaParser()
                    .Parse( regex )
                    .PrintToConsole()
                    .PrintToImageAsync( @".\nfa.png" );

                System.Console.WriteLine( "Converting NFA into DFA..." );
                FiniteAutomata dfa = await nfa
                    .Convert( new NfaToDfaConvertor() )
                    .PrintToConsole( "DFA" )
                    .PrintToImageAsync( @".\dfa1.png" );
                    
                System.Console.WriteLine( "DFA normalization..." );
                FiniteAutomata normalizedDfa = dfa
                    .Convert( new SetErrorStateOnEmptyTransitionsConvertor() )
                    .PrintToConsole( "Normalized DFA" );
                    
                System.Console.WriteLine( "DFA minimization..." );
                FiniteAutomata minimizedDfa = await normalizedDfa
                    .Convert( new DfaMinimizationConvertor() )
                    .PrintToConsole( "Minimized DFA" )
                    .PrintToImageAsync( @".\dfa3Minimized.png", new VisualizationOptions()
                    {
                        DrawErrorState = false,
                        TimeoutInMilliseconds = 15_000
                    } );
            }
            catch ( Exception ex )
            {
                System.Console.WriteLine( $"Couldn't create an finite automata for regex. Regex: {regex}" );
                System.Console.WriteLine( ex );
            }
        }
        
        System.Console.WriteLine( "Press any key..." );
        System.Console.ReadKey();
    }
}