using Grammars.LL.Models.Validations;
using Grammars.LL.Models.ValueObjects;
using Grammars.LL.Models.ValueObjects.Symbols;
using LinqExtensions;

namespace Grammars.LL.Models;

public class LlOneGrammar
{
    public RuleName StartRule { get; }
    public IDictionary<RuleName, GrammarRule> Rules { get; private set; }

    public LlOneGrammar(
        RuleName startRule,
        IEnumerable<GrammarRule> rules )
    {
        StartRule = startRule;
        Rules = rules.ToDictionary(
            x => x.Name,
            x => x );

        ValidateOrThrow();
    }

    public HashSet<RuleSymbol> GetTerminalHeaderSymbols( RuleName ruleName )
    {
        var result = new HashSet<RuleSymbol>();
        
        var queue = new Queue<RuleDefinition>();
        queue.EnqueueRange( Rules[ruleName].Definitions );
        var processed = new HashSet<RuleDefinition>();

        while ( queue.Any() )
        {
            RuleDefinition ruleDefinition = queue.Dequeue();
            if ( processed.Contains( ruleDefinition ) )
            {
                continue;
            }

            processed.Add( ruleDefinition );

            RuleSymbol headerSymbol = ruleDefinition.Symbols.First();
            if ( headerSymbol.Type == RuleSymbolType.NonTerminalSymbol )
            {
                queue.EnqueueRange( Rules[headerSymbol.RuleName!].Definitions );
                continue;
            }

            result.Add( headerSymbol );
        }

        return result;
    }

    internal LlOneGrammar Copy()
    {
        var rules = new List<GrammarRule>();
        foreach ( GrammarRule rule in Rules.Values )
        {
            var definitions = new List<RuleDefinition>();
            foreach ( RuleDefinition definition in rule.Definitions )
            {
                var ruleSymbols = new List<RuleSymbol>();
                foreach ( RuleSymbol ruleSymbol in definition.Symbols )
                {
                    ruleSymbols.Add( ruleSymbol.Type == RuleSymbolType.NonTerminalSymbol
                        ? RuleSymbol.NonTerminalSymbol( ruleSymbol.RuleName! )
                        : RuleSymbol.TerminalSymbol( ruleSymbol.Symbol! ) ); 
                }
                
                definitions.Add( new RuleDefinition( ruleSymbols ) );
            }
            
            rules.Add( new GrammarRule( new RuleName( rule.Name.Value ), definitions ) );
        }

        return new LlOneGrammar( new RuleName( StartRule.Value ), rules );
    }

    private void ValidateOrThrow()
    {
        var errors = new List<Exception>();

        errors.AddRange( new GrammarRulesAchievabilityValidator()
            .CheckAndGetFailed( StartRule, Rules )
            .Select( x => new ArgumentException( $"Rule is not achievable. RuleName: {x.Value}" ) ) );

        errors.AddRange( new GrammarRulesDeclarationCheck()
            .CheckAndGetFailed( StartRule, Rules )
            .Select( x => new ArgumentException( $"Rule was not declared. RuleName: {x.Value}" ) ) );
        
        // Add determination check: All rule values of the same rule must start from different symbols

        if ( errors.Any() )
        {
            throw new AggregateException( errors );
        }
    }
}