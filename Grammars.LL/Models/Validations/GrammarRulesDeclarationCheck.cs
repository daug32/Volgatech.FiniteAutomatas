using Grammars.Grammars.LeftRoRightOne.Models.ValueObjects;

namespace Grammars.Grammars.LeftRoRightOne.Models.Validations;

internal class GrammarRulesDeclarationCheck 
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
                foreach ( RuleSymbol valueItem in ruleValue.Symbols )
                {
                    if ( valueItem.Type != RuleSymbolType.NonTerminalSymbol )
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