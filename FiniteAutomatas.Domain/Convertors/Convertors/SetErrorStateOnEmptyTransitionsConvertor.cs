using FiniteAutomatas.Domain.Models.Automatas;
using FiniteAutomatas.Domain.Models.ValueObjects;

namespace FiniteAutomatas.Domain.Convertors.Convertors;

public class SetErrorStateOnEmptyTransitionsConvertor : IAutomataConvertor<FiniteAutomata>
{
    public FiniteAutomata Convert( FiniteAutomata automata )
    {
        var stateToTransitions = automata.AllStates
            .ToDictionary( 
                x => x.Name, 
                _ => new Dictionary<Argument, string>() );
        foreach ( Transition transition in automata.Transitions )
        {
            stateToTransitions[transition.From.Name].Add( transition.Argument, transition.To.Name );
        }
        
        var newStates = automata.AllStates
            .Select( x=> x.Copy() )
            .ToDictionary( x => x.Name, x => x );

        bool addedNewTransition = false;
        var alphabet = automata.Transitions.Select( x => x.Argument ).ToHashSet();
        foreach ( var transition in stateToTransitions )
        {
            var argumentToTargetState = transition.Value;
            if ( argumentToTargetState.Count == alphabet.Count )
            {
                continue;
            }

            addedNewTransition = true;
            foreach ( Argument argument in alphabet )
            {
                if ( argumentToTargetState.ContainsKey( argument ) )
                {
                    continue;
                }
                
                argumentToTargetState.Add( argument, "-1" );
            }
        }

        if ( !addedNewTransition )
        {
            return automata.Copy();
        } 

        newStates.Add( "-1", new State( "-1", isError: true ) );
        stateToTransitions.Add( "-1", automata.Alphabet.ToDictionary( x => x, _ => "-1" ) );

        return new FiniteAutomata(
            alphabet: alphabet,
            allStates: newStates.Values,
            transitions: stateToTransitions.SelectMany( stateToTransition => stateToTransition.Value
                .Select( argumentToTargetState => new Transition(
                    from: newStates[stateToTransition.Key],
                    to: newStates[argumentToTargetState.Value],
                    argument: argumentToTargetState.Key ) ) ) );
    }
}