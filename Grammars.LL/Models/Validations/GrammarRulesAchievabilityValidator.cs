using Grammars.LL.Models.ValueObjects;
using Grammars.LL.Models.ValueObjects.Symbols;

namespace Grammars.LL.Models.Validations;

internal class GrammarRulesAchievabilityValidator
{
    // If a non terminal rule is achievable, then all its values are achievable
    public IEnumerable<RuleName> CheckAndGetFailed(
        RuleName startRule,
        IDictionary<RuleName, GrammarRule> rules )
    {
        HashSet<RuleName> uncheckedRules = rules.Keys.ToHashSet();
        
        var queue = new Queue<RuleName>();
        queue.Enqueue( startRule );

        while ( queue.Any() )
        {
            RuleName ruleName = queue.Dequeue();
            if ( !uncheckedRules.Contains( ruleName ) )
            {
                continue;
            }

            uncheckedRules.Remove( ruleName );

            GrammarRule rule = rules[ruleName];
            foreach ( RuleDefinition ruleValue in rule.Definitions )
            {
                foreach ( RuleSymbol symbol in ruleValue.Symbols )
                {
                    if ( symbol.Type != RuleSymbolType.NonTerminalSymbol )
                    {
                        continue;
                    }
                    
                    queue.Enqueue( symbol.RuleName! );
                }
            }
        }

        return uncheckedRules;
    }
}