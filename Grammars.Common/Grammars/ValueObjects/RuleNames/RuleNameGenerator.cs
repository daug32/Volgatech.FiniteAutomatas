namespace Grammars.Common.Grammars.ValueObjects.RuleNames;

public class RuleNameGenerator
{
    private int _lastRuleIndex;

    public RuleNameGenerator( CommonGrammar grammar )
    {
        List<RuleName> ruleNames = grammar.Rules.Keys.ToList();
        
        int maxRuleName = -1;
        foreach ( RuleName ruleName in ruleNames )
        {
            if ( Int32.TryParse( ruleName.Value, out int ruleNameNumber ) )
            {
                maxRuleName = Math.Max( ruleNameNumber, maxRuleName );
            }
        }

        _lastRuleIndex = maxRuleName + 1;
    }

    public RuleName Next()
    {
        RuleName ruleName = new RuleName( _lastRuleIndex.ToString() );
        _lastRuleIndex++;

        return ruleName;
    }
}