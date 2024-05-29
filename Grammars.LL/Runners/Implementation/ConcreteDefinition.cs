using System.Diagnostics;
using Grammars.Common.Grammars;
using Grammars.Common.Grammars.Extensions;
using Grammars.Common.Grammars.ValueObjects;
using Grammars.Common.Grammars.ValueObjects.GrammarRules;
using Grammars.Common.Grammars.ValueObjects.RuleDefinitions;
using Grammars.Common.Grammars.ValueObjects.Symbols;

namespace Grammars.LL.Runners.Implementation;

internal class ConcreteDefinition
{
    public readonly RuleName RuleName;
    public readonly RuleDefinition Definition;
    public HashSet<TerminalSymbol> HeadingSymbols;

    public static List<ConcreteDefinition> FromGrammar( CommonGrammar grammar )
    {
        var result = new List<ConcreteDefinition>();
        
        foreach ( GrammarRule rule in grammar.Rules.Values )
        {
            foreach ( RuleDefinition definition in rule.Definitions )
            {
                IEnumerable<TerminalSymbol> headingSymbols = grammar.GetFirstSet( rule.Name, definition )
                    .GuidingSymbols
                    .Select( x => x.Symbol ?? throw new UnreachableException() );

                result.Add( new ConcreteDefinition( rule.Name, new RuleDefinition( definition.Symbols ), headingSymbols ) );
            }
        }

        return result;
    }

    public ConcreteDefinition( RuleName ruleName, RuleDefinition definition, IEnumerable<TerminalSymbol> headingSymbols )
    {
        RuleName = ruleName;
        Definition = definition;
        HeadingSymbols = headingSymbols.ToHashSet();
    }

    public override string ToString() => $"<{RuleName}> -> {String.Join( " ", Definition.Symbols )}";
}