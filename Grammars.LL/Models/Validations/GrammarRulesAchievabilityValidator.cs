using Grammars.Grammars.LeftRoRightOne.Models.ValueObjects;

namespace Grammars.Grammars.LeftRoRightOne.Models.Validations;

public class GrammarRulesAchievabilityValidator
{
    // If a non terminal rule is achievable, then all its values are achievable
    public IEnumerable<RuleName> Check(
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
    public IEnumerable<RuleName> Check( IDictionary<RuleName, GrammarRule> rules )
    {
        var nonDeclared = new HashSet<RuleName>();

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

public class GrammarRules