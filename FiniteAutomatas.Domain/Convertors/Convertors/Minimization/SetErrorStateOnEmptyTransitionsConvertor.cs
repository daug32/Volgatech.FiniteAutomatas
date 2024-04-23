using FiniteAutomatas.Domain.Models.Automatas;
using FiniteAutomatas.Domain.Models.ValueObjects;

namespace FiniteAutomatas.Domain.Convertors.Convertors.Minimization;

public class SetErrorStateOnEmptyTransitionsConvertor : IAutomataConvertor<DeterminedFiniteAutomata, DeterminedFiniteAutomata>
{
    private static readonly string _errorStateId = "-1";
        
    public DeterminedFiniteAutomata Convert( DeterminedFiniteAutomata automata )
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
            .Select( x => x.Copy() )
            .ToDictionary( x => x.Name, x => x );

        var addedNewTransition = false;
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

                argumentToTargetState.Add( argument, _errorStateId );
            }
        }

        if ( !addedNewTransition )
        {
            return automata.Copy();
        }

        newStates.Add( _errorStateId, new State( _errorStateId, isError: true ) );
        stateToTransitions.Add( _errorStateId, automata.Alphabet.ToDictionary( x => x, _ => _errorStateId ) );

        return new DeterminedFiniteAutomata(
            alphabet,
            allStates: newStates.Values,
            transitions: stateToTransitions.SelectMany( stateToTransition => stateToTransition.Value
                .Select( argumentToTargetState => new Transition(
                    newStates[stateToTransition.Key],
                    to: newStates[argumentToTargetState.Value],
                    argument: argumentToTargetState.Key ) ) ) );
    }
}