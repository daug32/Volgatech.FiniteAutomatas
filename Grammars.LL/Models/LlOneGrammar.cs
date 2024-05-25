using Grammars.Common;
using Grammars.Common.ValueObjects;
using Grammars.Common.ValueObjects.Symbols;
using Grammars.LL.Models.Validations;
using LinqExtensions;

namespace Grammars.LL.Models;

public class LlOneGrammar : CommonGrammar
{
    public LlOneGrammar( RuleName startRule, IEnumerable<GrammarRule> rules )
        : base( startRule, rules )
    {
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

    private void ValidateOrThrow()
    {
        var errors = new List<Exception>();

        errors.AddRange( new GrammarRulesAchievabilityValidator()
            .CheckAndGetFailed( StartRule, Rules )
            .Select( x => new ArgumentException( $"Rule is not achievable. RuleName: {x.Value}" ) ) );

        errors.AddRange( new GrammarRulesDeclarationCheck()
            .CheckAndGetFailed( StartRule, Rules )
            .Select( x => new ArgumentException( $"Rule was not declared. RuleName: {x.Value}" ) ) );

        if ( errors.Any() )
        {
            throw new AggregateException( errors );
        }
    }
}