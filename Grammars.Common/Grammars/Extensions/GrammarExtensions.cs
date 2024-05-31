using Grammars.Common.Grammars.ValueObjects;
using Grammars.Common.Grammars.ValueObjects.GrammarRules;
using Grammars.Common.Grammars.ValueObjects.RuleDefinitions;
using Grammars.Common.Grammars.ValueObjects.RuleNames;

namespace Grammars.Common.Grammars.Extensions;

public static class GrammarExtensions
{
    public static void RemoveAllDuplicateDefinitions( this CommonGrammar grammar )
    {
        foreach ( RuleName ruleName in grammar.Rules.Keys )
        {
            grammar.RemoveDuplicateDefinitions( ruleName );
        }
    }

    public static void RemoveDuplicateDefinitions( this CommonGrammar grammar, RuleName ruleToOptimizeName )
    {
        GrammarRule rule = grammar.Rules[ruleToOptimizeName];

        for ( var mainIterator = 0; mainIterator < rule.Definitions.Count; mainIterator++ )
        {
            RuleDefinition mainDefinition = rule.Definitions[mainIterator];

            for ( int secondIterator = mainIterator + 1; secondIterator < rule.Definitions.Count; secondIterator++ )
            {
                RuleDefinition secondDefinition = rule.Definitions[secondIterator];
                if ( !mainDefinition.Equals( secondDefinition ) )
                {
                    continue;
                }

                rule.Definitions.RemoveAt( secondIterator );
                secondIterator--;
            }
        }
    }
}