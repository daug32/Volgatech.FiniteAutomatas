using ReToDfa.FiniteAutomatas.ValueObjects;

namespace ReToDfa.FiniteAutomatas.ToDfaExtensions;

public static class FiniteAutomataToDfaExtension
{
    public static FiniteAutomata ToDfa( this FiniteAutomata automata )
    {
        var dfaTransitions = new HashSet<Transition>();
        var dfaStart = new CollapsedState( automata.AllStates.First( x => x.IsStart ) );

        var alphabet = automata.Alphabet.Where( x => x != AlphabetSymbol.Epsilon );
        var queue = new Queue<CollapsedState>();
        var processedStates = new HashSet<CollapsedState>();
        queue.Enqueue( dfaStart );

        Print( "Generating DFA" );

        while ( queue.Any() )
        {
            CollapsedState fromState = queue.Dequeue();
            Print( $"{nameof( CollapsedState )}: {fromState.Name}" );

            processedStates.Add( fromState );

            foreach ( AlphabetSymbol argument in alphabet )
            {
                Print( $"\t{nameof( AlphabetSymbol )}: {argument.Value}" );

                var achievableStates = fromState.States
                    .SelectMany( state => automata.Move( state, argument ) )
                    .ToHashSet();

                if ( !achievableStates.Any() )
                {
                    Print( $"\t{nameof( achievableStates )} are empty" );
                    Print();
                    continue;
                }

                var toState = new CollapsedState( achievableStates );

                // If we didn't process the state yet
                if ( !processedStates.Contains( toState ) )
                {
                    Print( $"\t{nameof( processedStates )}: enqueueing" );
                    queue.Enqueue( toState );
                }

                dfaTransitions.Add( new Transition(
                    fromState.ToState(),
                    argument,
                    toState.ToState() ) );

                Print( $"\t{nameof( Transition )}: From: {fromState.Name}, Arg: {argument.Value}, To: {toState.Name}" );
                Print();
            }
        }

        return BuildDfa( dfaTransitions, processedStates.Select( x => x.ToState() ) );
    }

    private static FiniteAutomata BuildDfa( ICollection<Transition> transitions, IEnumerable<State> states )
    {
        // oldName, newName
        var statesList = states.ToList();
        
        var nameOverrides = new Dictionary<string, string>();
        for ( int i = 0; i < statesList.Count; i++ )
        {   
            State state = statesList[i];

            string oldName = state.Name;
            string newName = $"S{i}";
            
            nameOverrides.Add( oldName, newName );
            state.Name = newName;
        }

        var alphabet = new HashSet<AlphabetSymbol>();
        foreach ( Transition transition in transitions )
        {
            transition.From.Name = nameOverrides[transition.From.Name];
            transition.To.Name = nameOverrides[transition.To.Name];
            alphabet.Add( transition.Argument );
        }

        return new FiniteAutomata(
            alphabet: alphabet,
            transitions: transitions,
            allStates: statesList );
    } 

    private static void Print( string message = "" )
    {
        return;
        Console.WriteLine( message );
    }
}