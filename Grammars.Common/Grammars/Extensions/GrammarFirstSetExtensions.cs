using Grammars.Common.Grammars.ValueObjects;
using Grammars.Common.Grammars.ValueObjects.RuleDefinitions;
using Grammars.Common.Grammars.ValueObjects.Symbols;
using LinqExtensions;

namespace Grammars.Common.Grammars.Extensions;

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
        var guidingSymbols = grammar.Rules.Keys.ToDictionary( x => x, x => new HashSet<RuleSymbol>() );

        var definitionsToCheck = BuildQueue( ruleName, definitions.ToList(), grammar );
        var processedDefinitions = new HashSet<ConcreteDefinition>();

        while ( definitionsToCheck.Any() )
        {
            ConcreteDefinition ruleDefinition = definitionsToCheck.Dequeue();
            if ( processedDefinitions.Contains( ruleDefinition ) )
            {
                continue;
            }

            processedDefinitions.Add( ruleDefinition );

            var headings = new HashSet<RuleSymbol>();
            bool hasNonEpsilonProductionAtTheEnd = false;
            foreach ( RuleSymbol symbol in ruleDefinition.Definition.Symbols )
            {
                if ( symbol.Type != RuleSymbolType.NonTerminalSymbol )
                {
                    headings.Add( symbol );
                    hasNonEpsilonProductionAtTheEnd = symbol.Symbol!.Type != TerminalSymbolType.EmptySymbol;
                    break;
                }

                var innerRuleHeadings = guidingSymbols[symbol.RuleName!];
                hasNonEpsilonProductionAtTheEnd = innerRuleHeadings.All( x => x.Symbol!.Type != TerminalSymbolType.EmptySymbol );
                
                headings.AddRange( innerRuleHeadings );

                if ( headings.Any( x => x.Symbol!.Type == TerminalSymbolType.EmptySymbol ) )
                {
                    continue;
                }

                break;
            }

            if ( hasNonEpsilonProductionAtTheEnd )
            {
                headings = headings
                    .Where( x => x.Type != RuleSymbolType.TerminalSymbol || x.Symbol!.Type != TerminalSymbolType.EmptySymbol )
                    .ToHashSet();
            }

            guidingSymbols[ruleDefinition.RuleName].AddRange( headings );
        }

        return new GuidingSymbolsSet( ruleName, guidingSymbols[ruleName] );
    }

    private static Queue<ConcreteDefinition> BuildQueue(
        RuleName ruleName,
        List<RuleDefinition> targetDefinitions,
        CommonGrammar grammar )
    {
        var result = new List<ConcreteDefinition>();

        var processedNonTerms = new HashSet<RuleName>();
        var nonTermsToProcess = new Queue<RuleName>( targetDefinitions.SelectMany( x =>
            x.Symbols.Where( x => x.Type == RuleSymbolType.NonTerminalSymbol ).Select( x => x.RuleName! ) ) );

        while ( nonTermsToProcess.Any() )
        {
            RuleName rule = nonTermsToProcess.Dequeue();
            if ( processedNonTerms.Contains( rule ) )
            {
                continue;
            }

            List<RuleDefinition> ruleDefinitions = grammar.Rules[rule].Definitions;
            IEnumerable<RuleName> nonTerms =
                ruleDefinitions.SelectMany( x => x.Symbols.Where( x => x.Type == RuleSymbolType.NonTerminalSymbol ).Select( x => x.RuleName! ) );

            foreach ( RuleName nonTerm in nonTerms )
            {
                nonTermsToProcess.Enqueue( nonTerm );
            }

            result.AddRange( ruleDefinitions.Select( x => new ConcreteDefinition( rule, x ) ) );

            processedNonTerms.Add( rule );
        }

        if ( !processedNonTerms.Contains( ruleName ) )
        {
            result.AddRange( targetDefinitions.Select( x => new ConcreteDefinition( ruleName, x ) ) );
        }

        return new Queue<ConcreteDefinition>( result.OrderBy( definition =>
        {
            RuleSymbol first = definition.Definition.FirstSymbol();

            if ( first.Type == RuleSymbolType.NonTerminalSymbol )
            {
                return 2;
            }

            if ( first.Symbol!.Type == TerminalSymbolType.EmptySymbol )
            {
                return 1;
            }

            return 0;
        } ) );
    }
}