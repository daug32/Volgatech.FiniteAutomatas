using FiniteAutomatas.Domain.ValueObjects;

namespace FiniteAutomatas.Domain.Automatas.Convertors;

public static class FiniteAutomataToDfaExtension
{
    private class CollapsedState
    {
        public string Name { get; set; }
        public bool IsStart { get; set; }
        public bool IsEnd { get; set; }
    
        public readonly HashSet<State> States = new();

        public CollapsedState( State state )
        {
            Name = state.Name;
            IsStart = state.IsStart;
            IsEnd = state.IsEnd;
            States.Add( state );
        }

        public CollapsedState( HashSet<State> states )
        {
            Name = String.Join( "_", states.Select( x => x.Name ).OrderBy( x => x ) );
            IsEnd = false;
            IsStart = false;

            foreach ( State state in states )
            {
                IsStart = IsStart || state.IsStart;
                IsEnd = IsEnd || state.IsEnd;
                States.Add( state );
            }
        }

        public State ToState() => new( name: Name, isStart: IsStart, isEnd: IsEnd );

        public override bool Equals( object? obj ) => obj is CollapsedState other && Equals( other );

        public bool Equals( CollapsedState? other ) => Name == other?.Name;

        public override int GetHashCode() => Name.GetHashCode();
    }
    
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