using FiniteAutomatas.Domain.Automatas;
using FiniteAutomatas.Domain.ValueObjects;

namespace FiniteAutomatas.Domain.Convertors.Convertors;

public class NfaToDfaConvertor : IAutomataConvertor<FiniteAutomata>
{
    private class CollapsedState
    {
        public string Name { get; }
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

        public CollapsedState( HashSet<State> states, bool isStart, bool isEnd )
        {
            Name = String.Join( "_", states.Select( x => x.Name ).OrderBy( x => x ) );
            IsEnd = isEnd;
            IsStart = isStart;

            foreach ( State state in states )
            {
                IsEnd = IsEnd || state.IsEnd;
                IsStart = IsStart || state.IsStart;
                States.Add( state );
            }
        }

        public State ToState()
        {
            return new State( Name, IsStart, IsEnd );
        }

        public override bool Equals( object? obj )
        {
            return obj is CollapsedState other && Equals( other );
        }

        public bool Equals( CollapsedState? other )
        {
            return Name == other?.Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }

    public FiniteAutomata Convert( FiniteAutomata automata )
    {
        var dfaTransitions = new HashSet<Transition>();
        var dfaStart = new CollapsedState( automata.AllStates.First( x => x.IsStart ) );

        var alphabet = automata.Alphabet.Where( x => x != Argument.Epsilon );
        var queue = new Queue<CollapsedState>();
        var processedStates = new HashSet<CollapsedState>();
        queue.Enqueue( dfaStart );

        var stateToEpsClosures = new Dictionary<State, HashSet<State>>(
            automata.AllStates.Select( 
                state => new KeyValuePair<State, HashSet<State>>( 
                    key: state,
                    value: automata.EpsClosure( state ).ToHashSet() ) ) );
        
        while ( queue.Any() )
        {
            CollapsedState fromState = queue.Dequeue();

            processedStates.Add( fromState );

            foreach ( State state in fromState.States )
            {
                var epsClosures = stateToEpsClosures[state];
                foreach ( State epsClosure in epsClosures )
                {
                    fromState.IsStart = fromState.IsStart || epsClosure.IsStart;
                    fromState.IsEnd = fromState.IsEnd || epsClosure.IsEnd;
                }
            } 

            foreach ( Argument argument in alphabet )
            {
                var achievableStates = fromState.States
                    .SelectMany( state => automata.Move( state, argument ) )
                    .ToHashSet();

                if ( !achievableStates.Any() )
                {
                    continue;
                }

                var toState = new CollapsedState(
                    achievableStates,
                    isStart: false,
                    isEnd: false );

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

        FiniteAutomata dfa = BuildDfa( dfaTransitions, processedStates.Select( x => x.ToState() ) );

        return dfa;
    }

    private static FiniteAutomata BuildDfa( ICollection<Transition> transitions, IEnumerable<State> states )
    {
        // oldName, newName
        var statesList = states.ToList();

        var nameOverrides = new Dictionary<string, string>();
        for ( var i = 0; i < statesList.Count; i++ )
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
            alphabet,
            transitions,
            statesList );
    }
}