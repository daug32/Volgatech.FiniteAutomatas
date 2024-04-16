using ReToDfa.FiniteAutomatas.Displays;
using ReToDfa.FiniteAutomatas.ValueObjects;

namespace ReToDfa.FiniteAutomatas;

public class FiniteAutomata
{
    public readonly HashSet<AlphabetSymbol> Alphabet;
    public readonly HashSet<Transition> Transitions;

    public readonly HashSet<State> AllStates;

    public FiniteAutomata( 
        IEnumerable<AlphabetSymbol> alphabet,
        IEnumerable<Transition> transitions, 
        IEnumerable<State> allStates )
    {
        Alphabet = alphabet.ToHashSet();
        AllStates = allStates.ToHashSet();

        Transitions = transitions
            .All( x =>
                AllStates.Contains( x.From ) && 
                AllStates.Contains( x.To ) && 
                Alphabet.Contains( x.Argument ) )
            ? transitions.ToHashSet()
            : throw new ArgumentException( "Not all From and To states of the transitions are presented in the AllStates" );
        
        AllStates.Where( x => x.IsEnd ).ToHashSet();
    }

    public IEnumerable<State> Move( State from, AlphabetSymbol argument )
    {
        var result = new List<State>();
        
        var statesToProcess = new Queue<State>();
        statesToProcess.Enqueue( from );

        while ( statesToProcess.Any() )
        {
            State state = statesToProcess.Dequeue();
            
            var stateTransitions = new Queue<Transition>( Transitions.Where( transition => transition.From == state ) );

            while ( stateTransitions.Any() )
            {
                Transition transition = stateTransitions.Dequeue();

                if ( transition.Argument == argument )
                {
                    result.Add( transition.To );
                    continue;
                }

                if ( transition.Argument == AlphabetSymbol.Epsilon )
                {
                    statesToProcess.Enqueue( transition.To );
                    continue;
                }
            }
        }

        return result;
    }

    public void Print() => FiniteAutomataConsoleDisplay.Print( this );

    public FiniteAutomata Copy() => new(
        alphabet: Alphabet.ToHashSet(),
        transitions: Transitions,
        allStates: AllStates.ToHashSet() );
}