using Grammars.Common.Grammars.ValueObjects;
using Grammars.Common.Grammars.ValueObjects.GrammarRules;
using Grammars.Common.Grammars.ValueObjects.RuleDefinitions;
using Grammars.Common.Grammars.ValueObjects.RuleNames;
using Grammars.Common.Grammars.ValueObjects.Symbols;

namespace Grammars.Common.Convertors.Implementation.Factorization;

internal class UnitableDefinitionsGroups
{
    public readonly RuleName RuleName;
    public readonly HashSet<RuleSymbol> Headings;
    public readonly List<RuleDefinition> Definitions;

    public UnitableDefinitionsGroups( RuleName ruleName, HashSet<RuleSymbol> headings, IEnumerable<RuleDefinition> concreteDefinitions )
    {
        RuleName = ruleName;
        Headings = headings;
        Definitions = concreteDefinitions.ToList();
    }
}