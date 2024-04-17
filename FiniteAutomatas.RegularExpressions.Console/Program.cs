using FiniteAutomatas.Domain.Automatas;
using FiniteAutomatas.Domain.Convertors;
using FiniteAutomatas.Domain.Convertors.Convertors;
using FiniteAutomatas.RegularExpressions.Console.Displays;

namespace FiniteAutomatas.RegularExpressions.Console;

public class Program
{
    private static readonly RegexToFiniteAutomataParser _regexToFiniteAutomataParser = new();

    public static void Main( string[] args )
    {
        while ( true )
        {
            System.Console.Write( "Write a regex: " );
            string regex = System.Console.ReadLine()!;
        
            System.Console.WriteLine( "Creating an NFA..." );
            if ( !_regexToFiniteAutomataParser.TryParse( regex, out FiniteAutomata? nfa ) )
            {
                System.Console.WriteLine( "Couldn't create an NFA" );
                continue;
            }
        
            System.Console.WriteLine( "Converting into DFA..." );
            FiniteAutomata dfa = nfa!.Convert( new FiniteAutomataToDfaConvertor() );
        
            dfa.Print();
        }
        
        System.Console.WriteLine( "Press any key..." );
        System.Console.ReadKey();
    }
}