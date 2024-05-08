using Grammars.Grammars.LeftRoRightOne.Models.Validations;
using Grammars.Grammars.LeftRoRightOne.Models.ValueObjects;
using LinqExtensions;

namespace Grammars.Grammars.LeftRoRightOne.Models;

public class LlOneGrammar
{
    public RuleName StartRule { get; }
    public IDictionary<RuleName, GrammarRule> Rules { get; }

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
        
        var queue = new Queue<RuleValue>();
        queue.EnqueueRange( Rules[ruleName].Values );
        var processed = new HashSet<RuleValue>();

        while ( queue.Any() )
        {
            RuleValue ruleValue = queue.Dequeue();
            if ( processed.Contains( ruleValue ) )
            {
                continue;
            }

            processed.Add( ruleValue );

            RuleSymbol headerSymbol = ruleValue.Symbols.First();
            if ( headerSymbol.Type == RuleSymbolType.NonTerminalSymbol )
            {
                queue.EnqueueRange( Rules[headerSymbol.RuleName!].Values );
                continue;
            }

            result.Add( headerSymbol );
        }

        return result;
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