namespace Grammars.Common.ValueObjects;

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
}