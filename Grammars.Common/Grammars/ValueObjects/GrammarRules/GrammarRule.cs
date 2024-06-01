using Grammars.Common.Grammars.ValueObjects.RuleDefinitions;
using Grammars.Common.Grammars.ValueObjects.RuleNames;

namespace Grammars.Common.Grammars.ValueObjects.GrammarRules;

public class GrammarRule
{
    public readonly RuleName Name;
    public List<RuleDefinition> Definitions;

    public GrammarRule( RuleName name, IEnumerable<RuleDefinition> definitions )
    {
        Name = name;
        Definitions = definitions.ToList();
    }
    
    public GrammarRule Copy() => new( Name, Definitions.Select( x => x.Copy() ) );

    public override string ToString() => $"{Name}";
}