using System.Collections.Immutable;
using Grammars.Common.ValueObjects.Symbols;

namespace Grammars.Common.ValueObjects;

public class GuidingSymbolsSet
{
    public readonly RuleName Rule;
    public readonly IImmutableSet<RuleSymbol> GuidingSymbols;

    public GuidingSymbolsSet( RuleName rule, IEnumerable<RuleSymbol> guidingSymbols )
    {
        Rule = rule;
        GuidingSymbols = guidingSymbols.ToImmutableHashSet();
    }

    public bool Has( RuleSymbol symbol ) => GuidingSymbols.Contains( symbol );

    public bool HasIntersections( GuidingSymbolsSet other )
    {
        return GuidingSymbols.Intersect( other.GuidingSymbols ).Any();
    }
}