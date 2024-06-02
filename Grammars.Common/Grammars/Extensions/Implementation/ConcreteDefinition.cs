using Grammars.Common.Grammars.ValueObjects.RuleDefinitions;
using Grammars.Common.Grammars.ValueObjects.RuleNames;

namespace Grammars.Common.Grammars.Extensions.Implementation;

internal class ConcreteDefinition
{
    public readonly RuleName RuleName;
    public readonly RuleDefinition Definition;

    public ConcreteDefinition( RuleName ruleName, RuleDefinition definition )
    {
        RuleName = ruleName;
        Definition = new RuleDefinition( definition.Symbols );
    }

    public bool Has( RuleName ruleName ) => Definition.Has( ruleName );

    public override bool Equals( object? obj ) => 
        obj is ConcreteDefinition other && 
        other.RuleName == RuleName && 
        other.Definition == Definition;

    public override int GetHashCode() => HashCode.Combine( RuleName, Definition );
}