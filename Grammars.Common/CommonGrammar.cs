using Grammars.Common.ValueObjects;
using Grammars.Common.ValueObjects.Symbols;
using LinqExtensions;

namespace Grammars.Common;

public class CommonGrammar
{
    public RuleName StartRule { get; }
    public IDictionary<RuleName, GrammarRule> Rules { get; }

    public CommonGrammar( RuleName startRule, IEnumerable<GrammarRule> rules )
    {
        StartRule = startRule;
        Rules = rules.ToDictionary(
            x => x.Name,
            x => x );

        if ( !Rules.ContainsKey( startRule ) )
        {
            throw new ArgumentException( $"StartRule was not found. StartRuleName: {startRule}" );
        }

        var symbols = Rules.Values
            .SelectMany( rule => rule.Definitions.SelectMany( definition => definition.Symbols ) )
            .Where( x => x.Type == RuleSymbolType.NonTerminalSymbol )
            .ToHashSet();

        foreach ( RuleSymbol symbol in symbols )
        {
            if ( !Rules.ContainsKey( symbol.RuleName! ) )
            {
                throw new ArgumentException( $"Grammar doesn't have a definition for rule. RuleName: {symbol}" );
            }
        }
    }

    public GuidingSymbolsSet GetGuidingSymbolsSet( RuleName ruleName, RuleDefinition definition ) =>
        GetGuidingSymbolsSet( ruleName, new[] { definition } );

    public CommonGrammar Copy() => new( 
        StartRule,
        Rules.Values.Select( grammarRule => grammarRule.Copy() ) );

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