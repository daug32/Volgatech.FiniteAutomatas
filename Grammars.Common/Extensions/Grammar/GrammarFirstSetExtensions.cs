using Grammars.Common.Extensions.Grammar.ValuesObjects;
using Grammars.Common.ValueObjects;
using Grammars.Common.ValueObjects.Symbols;
using LinqExtensions;

namespace Grammars.Common.Extensions.Grammar;

public static class GrammarFirstSetExtensions
{
    public static GuidingSymbolsSet GetFirstSet( this CommonGrammar grammar, RuleName ruleName )
    {
        return grammar.GetFirstSet( ruleName, grammar.Rules[ruleName].Definitions );
    }

    public static GuidingSymbolsSet GetFirstSet( this CommonGrammar grammar, RuleName ruleName, RuleDefinition definition )
    {
        return grammar.GetFirstSet( ruleName, new[] { definition } );
    }

    private static GuidingSymbolsSet GetFirstSet( this CommonGrammar grammar, RuleName ruleName, IEnumerable<RuleDefinition> definitions )
    {
        var guidingSymbols = new HashSet<RuleSymbol>();

        var definitionsToCheck = new Queue<RuleDefinition>();
        definitionsToCheck.EnqueueRange( definitions );

        var processedDefinitions = new HashSet<RuleDefinition>();

        while ( definitionsToCheck.Any() )
        {
            RuleDefinition ruleDefinition = definitionsToCheck.Dequeue();
            if ( processedDefinitions.Contains( ruleDefinition ) )
            {
                continue;
            }

            processedDefinitions.Add( ruleDefinition );

            RuleSymbol headerSymbol = ruleDefinition.Symbols.First();
            if ( headerSymbol.Type == RuleSymbolType.NonTerminalSymbol )
            {
                definitionsToCheck.EnqueueRange( grammar.Rules[headerSymbol.RuleName!].Definitions );
                continue;
            }

            guidingSymbols.Add( headerSymbol );
        }

        return new GuidingSymbolsSet( ruleName, guidingSymbols );
    }
}