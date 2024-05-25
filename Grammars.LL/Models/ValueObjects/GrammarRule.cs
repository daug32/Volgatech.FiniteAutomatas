using LinqExtensions;

namespace Grammars.LL.Models.ValueObjects;

public class GrammarRule
{
    public readonly RuleName Name;
    public IReadOnlyCollection<RuleDefinition> Definitions { get; private set; }

    public GrammarRule( RuleName name, IEnumerable<RuleDefinition> definitions )
    {
        Name = name;
        Definitions = definitions.ToList();
    }

    public void AddDefinition( RuleDefinition definition )
    {
        Definitions = Definitions.ToList().With( definition );
    }
}