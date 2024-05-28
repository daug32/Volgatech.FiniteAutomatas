using Grammars.Common.Extensions.Grammar.ValuesObjects;
using Grammars.Common.ValueObjects.Symbols;

namespace Grammars.Common.Extensions.Grammar;

public static class GrammarFollowSetExtensions
{
    public static GuidingSymbolsSet GetFollowSet( this CommonGrammar grammar, ValueObjects.RuleName ruleName )
    {
        var followSymbols = new HashSet<RuleSymbol>();

        var rulesToFind = new Queue<ValueObjects.RuleName>( new[] { ruleName } );
        var processedRules = new HashSet<ValueObjects.RuleName>();

        while ( rulesToFind.Any() )
        {
            ValueObjects.RuleName ruleToFind = rulesToFind.Dequeue();
            if ( processedRules.Contains( ruleToFind ) )
            {
                continue;
            }

            processedRules.Add( ruleToFind );

            List<(ValueObjects.RuleName RuleName, ValueObjects.RuleDefinition RuleDefinition)> usages = grammar.Rules.Values
                .SelectMany( r => r.Definitions.Select( d => ( Rule: r.Name, RuleDefinition: d ) ) )
                .Where( x => x.RuleDefinition.Has( ruleToFind ) )
                .ToList();

            foreach ( (ValueObjects.RuleName RuleName, ValueObjects.RuleDefinition RuleDefinition) usage in usages )
            {
                for ( var index = 0; index < usage.RuleDefinition.Symbols.Count; index++ )
                {
                    RuleSymbol symbol = usage.RuleDefinition.Symbols[index];
                    if ( symbol.Type != RuleSymbolType.NonTerminalSymbol || symbol.RuleName != ruleName )
                    {
                        continue;
                    }

                    if ( index + 1 < usage.RuleDefinition.Symbols.Count )
                    {
                        followSymbols.Add( usage.RuleDefinition.Symbols[index + 1] );
                        continue;
                    }

                    rulesToFind.Enqueue( usage.RuleName );
                }
            }

            if ( ruleToFind == grammar.StartRule )
            {
                followSymbols.Add( RuleSymbol.TerminalSymbol( TerminalSymbol.End() ) );
            }
        }

        return new GuidingSymbolsSet( ruleName, followSymbols );
    }
}