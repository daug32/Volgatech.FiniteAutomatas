using FiniteAutomatas.Domain.Models.Automatas;
using FiniteAutomatas.Domain.Models.ValueObjects;

namespace FiniteAutomatas.Domain.Convertors.Convertors;

public class MileyToMureConvertor : IAutomataConvertor<Miley, Mure>
{
    public Mure Convert( Miley automata )
    {
        return new Mure(
            BuildStateOverrides( automata ),
            BuildTransitions( automata ) );
    }

    private static HashSet<Transition> BuildTransitions( Miley fa )
    {
        var transitions = new HashSet<Transition>();

        foreach ( State state in fa.AllStates )
        {
            var argumentToState = new Dictionary<Argument, State>(
                fa.Transitions
                    .Where( x => x.From.Equals( state ) )
                    .Select( x => new KeyValuePair<Argument, State>( x.Argument, x.To ) ) );

            var stateTransitions = argumentToState.Select( x => new Transition(
                state,
                to: x.Value,
                argument: x.Key ) );

            transitions.UnionWith( stateTransitions );
        }

        return transitions;
    }

    private static Dictionary<State, State> BuildStateOverrides( Miley fa )
    {
        var stateToOutputs = new Dictionary<State, List<string>>();
        foreach ( State? state in fa.AllStates )
        {
            IEnumerable<Transition> stateTransitions = fa.Transitions
                .Where( x => x.From
                    .Equals( state ) )
                    .OrderBy( x => x.Argument );

            stateToOutputs.Add( 
                state, 
                stateTransitions
                    .Select( x => x.AdditionalData )
                    .ToList()! );
        }

        var outputsToGroup = new Dictionary<List<string>, State>();
        var stateToGroup = new Dictionary<State, State>();
        foreach ( State state in fa.AllStates )
        {
            var outputs = outputsToGroup.Keys.FirstOrDefault( x =>
            {
                for ( var i = 0; i < stateToOutputs[state].Count; i++ )
                {
                    if ( stateToOutputs[state][i] != x[i] )
                    {
                        return false;
                    }
                }

                return true;
            } );

            if ( outputs is null )
            {
                outputs = stateToOutputs[state];
                var stateName = ( outputsToGroup.Count + 1 ).ToString();
                outputsToGroup[outputs] = new State( stateName );
            }

            stateToGroup[state] = outputsToGroup[outputs];
        }

        return stateToGroup;
    }
}