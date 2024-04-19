using FiniteAutomatas.Domain.Automatas;
using FiniteAutomatas.Domain.Convertors;
using FiniteAutomatas.Domain.Convertors.Convertors;
using FiniteAutomatas.RegularExpressions.Console.Displays;
using FiniteAutomatas.Visualizations;

namespace FiniteAutomatas.RegularExpressions.Console;

public class Program
{
    public static void Main()
    {
        while ( true )
        {
            System.Console.Write( "Write a regex: " );
            string regex = System.Console.ReadLine()!;
            
            try
            {
                System.Console.WriteLine( "Creating an NFA..." );
                FiniteAutomata nfa = new RegexToNfaParser().Parse( regex );
                nfa.Print();
                new Visualizer( nfa ).ToImage( @"D:\Development\Projects\TestingStation\nfa.png" );

                System.Console.WriteLine( "Converting into DFA..." );
                FiniteAutomata dfa = nfa.Convert( new NfaToDfaConvertor() );
                dfa.Print();
                
                new Visualizer( dfa ).ToImage( @"D:\Development\Projects\TestingStation\dfa.png" );
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