using Grammars.Common.ValueObjects;

namespace Grammars.Common.Convertors.LeftFactorization;

public class ConcreteDefinition
{
    public RuleName RuleName;
    public RuleDefinition Definition;

    public ConcreteDefinition( RuleName ruleName, RuleDefinition definition )
    {
        RuleName = ruleName;
        Definition = definition;
    }
}