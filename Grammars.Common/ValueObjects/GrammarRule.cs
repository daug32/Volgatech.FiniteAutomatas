﻿using LinqExtensions;

namespace Grammars.Common.ValueObjects;

public class GrammarRule
{
    public readonly RuleName Name;
    public IReadOnlyList<RuleDefinition> Definitions { get; private set; }

    public GrammarRule( RuleName name, IEnumerable<RuleDefinition> definitions )
    {
        Name = name;
        Definitions = definitions.ToList();
    }

    public void AddDefinition( RuleDefinition definition )
    {
        Definitions = Definitions.ToList().With( definition );
    }

    public void RemoveDefinition( RuleDefinition definition )
    {
        Definitions = Definitions.Where( x => !x.Equals( definition ) ).ToList();
    }
}