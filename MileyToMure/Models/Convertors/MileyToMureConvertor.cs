using FiniteAutomatas.Domain.Automatas;
using FiniteAutomatas.Domain.ValueObjects;

namespace MileyToMure.Models.Convertors;

public class MileyToMureConvertor
{
    public Mure ToMure( Miley fa ) => new(
        overrides: BuildStateOverrides(fa), 
        transitions: BuildTransitions(fa) );

    private static HashSet<Transition> BuildTransitions(Miley fa)
    {
        var transitions = new HashSet<Transition>();

        foreach (State state in fa.AllStates)
        {
            var argumentToState = new Dictionary<Argument, State>(
                fa.Transitions
                    .Where(x => x.From.Equals(state))
                    .Select(x => new KeyValuePair<Argument, State>(x.Argument, x.To)));

            IEnumerable<Transition> stateTransitions = argumentToState.Select(x => new Transition(
                from: state,
                to: x.Value,
                argument: x.Key));
            transitions.UnionWith(stateTransitions);
        }

        return transitions;
    }

    private static Dictionary<State, State> BuildStateOverrides(Miley fa)
    {
        var stateToOutputs = new Dictionary<State, List<string>>();
        foreach (var state in fa.AllStates)
        {
            IEnumerable<Transition> stateTransitions =
                fa.Transitions.Where(x => x.From.Equals(state)).OrderBy(x => x.Argument);
            stateToOutputs.Add(state, stateTransitions.Select(x => x.OutputSymbol).ToList()!);
        }

        var outputsToGroup = new Dictionary<List<string>, State>();
        var stateToGroup = new Dictionary<State, State>();
        foreach (State state in fa.AllStates)
        {
            List<string>? outputs = outputsToGroup.Keys.FirstOrDefault(x =>
            {
                for (int i = 0; i < stateToOutputs[state].Count; i++)
                {
                    if (stateToOutputs[state][i] != x[i])
                    {
                        return false;
                    }
                }

                return true;
            });

            if (outputs is null)
            {
                outputs = stateToOutputs[state];
                outputsToGroup[outputs] = new State((outputsToGroup.Count + 1).ToString());
            }

            stateToGroup[state] = outputsToGroup[outputs];
        }

        return stateToGroup;
    }
}