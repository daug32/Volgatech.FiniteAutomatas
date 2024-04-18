using FiniteAutomatas.Domain.Automatas;
using FiniteAutomatas.Domain.Convertors;
using FiniteAutomatas.Domain.Convertors.Convertors;
using FiniteAutomatas.RegularExpressions.Console.Displays;
using FiniteAutomatas.Visualizations;

namespace FiniteAutomatas.RegularExpressions.Console;

public class Program
{
    private static readonly RegexToNfaParser _regexToNfaParser = new();

    public static void Main( string[] args )
    {
        while ( true )
        {
            System.Console.Write( "Write a regex: " );
            string regex = System.Console.ReadLine()!;
            
            try
            {
                System.Console.WriteLine( "Creating an NFA..." );
                FiniteAutomata nfa = new RegexToNfaParser().Parse( regex );

                System.Console.WriteLine( "Converting into DFA..." );
                FiniteAutomata dfa = nfa.Convert( new NfaToDfaConvertor() );
                dfa.Print();
                
                new Visualizer( dfa ).ToImage( @"D:\Development\Projects\TestingStation\image.png" );
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