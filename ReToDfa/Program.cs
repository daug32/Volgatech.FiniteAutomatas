using ReToDfa.FiniteAutomatas;
using ReToDfa.FiniteAutomatas.ToDfaExtensions;
using ReToDfa.FiniteAutomatas.ValueObjects;
using ReToDfa.Regexes;

namespace ReToDfa;

public class Program
{
    public static void Main( string[] args )
    {
        var finiteAutomataCreator = new FiniteAutomataCreator();
        while ( true )
        {
            Console.Write( "Write a regex: " );
            string regex = Console.ReadLine()!;

            Console.WriteLine( "Creating an NFA..." );
            // if ( !finiteAutomataCreator.TryCreateFromRegex( regex, out FiniteAutomata? nfa ) )
            // {
            //     Console.WriteLine( "Couldn't create an NFA" );
            //     continue;
            // }
            FiniteAutomata nfa = finiteAutomataCreator.CreateFromRegex( regex );

            Console.WriteLine( "Converting into DFA..." );
            FiniteAutomata dfa = nfa!.ToDfa();
            
            dfa.Print();
        }

        Console.WriteLine( "Press any key..." );
        Console.ReadKey();
    }

    private static void ReToNfaTest()
    {
        void Test( string expression )
        {
            FiniteAutomata nfa = new FiniteAutomataCreator().CreateFromRegex( expression );
            nfa.Print();
            
            FiniteAutomata dfa = nfa.ToDfa();
            dfa.Print();
        }
        
        Test( "a" );
        Test( "(a)" );
        Test( "((a))" );
        Test( "a(b)" );
        Test( "a*" );
        Test( "a+" );
        Test( "a|b" );
        Test( "(a)|b" );
        Test( "a|(b)" );
        Test( "(abc)+" );
        Test( "a*|b(c*|a*)" );
    }

    private static void NfaToDfaTest()
    {
        var alphabet = new List<AlphabetSymbol>()
        {
            new( "a" ),
            new( "b" ),
            new( "c" ),
            AlphabetSymbol.Epsilon
        }.ToDictionary( x => x.Value, x => x );

        var allStates = new List<State>()
        {
            new( "q0", true ),
            new( "q1" ),
            new( "q2" ),
            new( "q3" ),
            new( "q4" ),
            new( "q5" ),
            new( "q6" ),
            new( "q7" ),
            new( "q8" ),
            new( "q9" ),
            new( "q10" ),
            new( "q11", isEnd: true )
        }.ToDictionary( x => x.Name, x => x );

        var transitions = new List<Transition>()
        {
            new( allStates["q0"], alphabet[""], allStates["q1"] ),
            new( allStates["q0"], alphabet[""], allStates["q5"] ),

            new( allStates["q1"], alphabet["a"], allStates["q2"] ),

            new( allStates["q2"], alphabet["b"], allStates["q3"] ),

            new( allStates["q3"], alphabet["c"], allStates["q4"] ),

            new( allStates["q4"], alphabet[""], allStates["q5"] ),
            new( allStates["q4"], alphabet[""], allStates["q1"] ),

            new( allStates["q5"], alphabet[""], allStates["q6"] ),
            new( allStates["q5"], alphabet[""], allStates["q8"] ),

            new( allStates["q6"], alphabet["a"], allStates["q7"] ),

            new( allStates["q7"], alphabet[""], allStates["q8"] ),
            new( allStates["q7"], alphabet[""], allStates["q6"] ),

            new( allStates["q8"], alphabet[""], allStates["q11"] ),
            new( allStates["q8"], alphabet[""], allStates["q9"] ),

            new( allStates["q9"], alphabet["b"], allStates["q10"] ),

            new( allStates["q10"], alphabet[""], allStates["q9"] ),
            new( allStates["q10"], alphabet[""], allStates["q11"] )
        };

        var nfa = new FiniteAutomata(
            alphabet.Values,
            transitions,
            allStates.Values );
        nfa.Print();

        FiniteAutomata dfa = nfa.ToDfa();
        dfa.Print();
    }
}
