using Grammar.Parsers.Implementation.Implementation.Models;
using Grammars.Common.Grammars;
using Grammars.Common.Grammars.ValueObjects;
using Grammars.Common.Grammars.ValueObjects.GrammarRules;
using Grammars.Common.Grammars.ValueObjects.RuleDefinitions;

namespace Grammar.Parsers.Implementation.Implementation;

public class GrammarBuilder
{
    public CommonGrammar BuildGrammar( List<GrammarRuleParseResult> rules )
    {
        var rulesDictionary = new Dictionary<RuleName, List<RuleDefinition>>();
        foreach ( GrammarRuleParseResult rule in rules )
        {
            if ( rulesDictionary.ContainsKey( rule.RuleName ) )
            {
                rulesDictionary[rule.RuleName].AddRange( rule.RuleDefinitions );
            }
            else
            {
                rulesDictionary.Add( rule.RuleName, rule.RuleDefinitions );
            }
        }

        RuleName startRule = rules.First().RuleName;
        var grammarRules = rulesDictionary.Select( x => new GrammarRule( x.Key, x.Value ) );

        return new CommonGrammar( startRule, grammarRules );
    }
}