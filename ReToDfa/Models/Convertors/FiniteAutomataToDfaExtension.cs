using FiniteAutomatas.Domain.Automatas;
using FiniteAutomatas.Domain.ValueObjects;

namespace ReToDfa.Models.Convertors;

public static class FiniteAutomataToDfaExtension
{
    public static FiniteAutomata ToDfa( this FiniteAutomata automata )
    {
        var dfaTransitions = new HashSet<Transition>();
        var dfaStart = new CollapsedState( automata.AllStates.First( x => x.IsStart ) );

        var alphabet = automata.Alphabet.Where( x => x != Argument.Epsilon );
        var queue = new Queue<CollapsedState>();
        var processedStates = new HashSet<CollapsedState>();
        queue.Enqueue( dfaStart );

        while ( queue.Any() )
        {
            CollapsedState fromState = queue.Dequeue();

            processedStates.Add( fromState );

            foreach ( Argument argument in alphabet )
            {
                var achievableStates = fromState.States
                    .SelectMany( state => automata.Move( state, argument ) )
                    .ToHashSet();

                if ( !achievableStates.Any() )
                {
                    continue;
                }

                var toState = new CollapsedState( achievableStates );

                // If we didn't process the state yet
                if ( !processedStates.Contains( toState ) )
                {
                    queue.Enqueue( toState );
                }

                dfaTransitions.Add( new Transition(
                    fromState.ToState(),
                    argument,
                    toState.ToState() ) );
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

        var alphabet = new HashSet<Argument>();
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
}