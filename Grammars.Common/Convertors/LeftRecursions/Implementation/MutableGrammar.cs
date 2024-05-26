using Grammars.Common.ValueObjects;
using Grammars.Common.ValueObjects.Symbols;
using LinqExtensions;

namespace Grammars.Common.Convertors.LeftRecursions.Implementation;

internal class MutableGrammar
{
    public RuleName StartRule { get; }
    public IDictionary<RuleName, GrammarRule> Rules { get; protected set; }
    
    public MutableGrammar( CommonGrammar grammar )
    {
        StartRule = grammar.StartRule;
        Rules = grammar.Rules.ToDictionary( x => x.Key, x => x.Value );
    }

    public void Replace( GrammarRule rule )
    {
        Rules[rule.Name] = rule;
    }

    public CommonGrammar ToGrammar() => new( StartRule, Rules.Values );

    public GuidingSymbolsSet GetGuidingSymbolsSet( RuleName ruleName, RuleDefinition definition ) =>
        GetGuidingSymbolsSet( ruleName, new[] { definition } );

    private GuidingSymbolsSet GetGuidingSymbolsSet( RuleName ruleName, IEnumerable<RuleDefinition> definitions )
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
                definitionsToCheck.EnqueueRange( Rules[headerSymbol.RuleName!].Definitions );
                continue;
            }

            guidingSymbols.Add( headerSymbol );
        }

        return new GuidingSymbolsSet( ruleName, guidingSymbols );
    }
}