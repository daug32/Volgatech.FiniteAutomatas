using Grammars.Grammars.LeftRoRightOne.Models.ValueObjects;

namespace Grammars.Grammars.LeftRoRightOne.Models.Validations;

public class GrammarRulesAchievabilityValidator
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
            uncheckedRules.Remove( ruleName );

            GrammarRule rule = rules[ruleName];
            foreach ( RuleValue ruleValue in rule.Values )
            {
                foreach ( RuleValueItem item in ruleValue.Items )
                {
                    if ( item.Type != RuleValueItemType.TerminalSymbol )
                    {
                        continue;
                    }
                    
                    queue.Enqueue( item.RuleName! );
                }
            }
        }

        return uncheckedRules;
    }
}

public class GrammarRulesDeclarationCheck 
{
    public IEnumerable<RuleName> CheckAndGetFailed(
        RuleName startRule,
        IDictionary<RuleName, GrammarRule> rules )
    {
        var nonDeclared = new HashSet<RuleName>();
        if ( !rules.ContainsKey( startRule ) )
        {
            nonDeclared.Add( startRule );
        }

        foreach ( GrammarRule rule in rules.Values )
        {
            foreach ( RuleValue ruleValue in rule.Values )
            {
                foreach ( RuleValueItem valueItem in ruleValue.Items )
                {
                    if ( valueItem.Type != RuleValueItemType.NonTerminalSymbol )
                    {
                        continue;
                    }

                    if ( !rules.ContainsKey( valueItem.RuleName! ) )
                    {
                        nonDeclared.Add( valueItem.RuleName! );
                    }
                }
            }
        }

        return nonDeclared;
    }
}

public class GrammarRulesProductivityCheck
{
    public IEnumerable<RuleName> CheckAndGetFailed(
        RuleName startRule,
        IDictionary<RuleName, GrammarRule> rules )
    {
        var productiveRules = new HashSet<RuleName>();

        while ( true )
        {
            bool hasChanges = false;
            
            foreach ( GrammarRule grammarRule in rules.Values )
            {
                foreach ( RuleValue ruleValue in grammarRule.Values )
                {
                    bool containOnlyProductiveOrTerminalSymbols = ruleValue.Items.All( x => 
                        x.Type != RuleValueItemType.NonTerminalSymbol ||
                        productiveRules.Contains( x.RuleName! ) );

                    if ( !containOnlyProductiveOrTerminalSymbols )
                    {
                        continue;
                    }

                    hasChanges = true;
                    productiveRules.Add( grammarRule.Name );
                    break;
                }
            }
            
            
        }
    }
}