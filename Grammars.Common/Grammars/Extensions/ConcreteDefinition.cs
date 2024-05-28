using Grammars.Common.Grammars.ValueObjects;
using Grammars.Common.Grammars.ValueObjects.RuleDefinitions;
using Grammars.Common.Grammars.ValueObjects.Symbols;

namespace Grammars.Common.Grammars.Extensions;

internal class ConcreteDefinition
{
    public RuleName RuleName;
    public RuleDefinition Definition;

    public ConcreteDefinition( RuleName ruleName, RuleDefinition definition )
    {
        RuleName = ruleName;
        Definition = new RuleDefinition( definition.Symbols );
    }

    public bool Has( RuleName ruleName ) => Definition.Has( ruleName );

    public override bool Equals( object? obj )
    {
        if ( obj is not ConcreteDefinition other )
        {
            return false;
        }

        return other.RuleName == RuleName && other.Definition == Definition;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine( RuleName, Definition );
    }
}