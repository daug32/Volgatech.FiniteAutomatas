using Grammars.Common.Grammars;
using Grammars.Common.Grammars.ValueObjects.GrammarRules;
using Grammars.Common.Grammars.ValueObjects.RuleDefinitions;
using Grammars.Common.Grammars.ValueObjects.RuleNames;
using Grammars.Common.Grammars.ValueObjects.Symbols;

namespace Grammars.Common.Convertors.Convertors.Inlining.Implementation;

internal class InlineRulesHandler
{
    public CommonGrammar Inline( CommonGrammar grammar )
    {
        var hasChanges = true;
        while ( hasChanges )
        {
            hasChanges = false;

            List<RuleName> rulesToInline = grammar.Rules.Keys.ToList();
            foreach ( RuleName ruleToInline in rulesToInline )
            {
                hasChanges |= InlineRule( ruleToInline, grammar );
            }
        }

        return grammar;
    }

    private bool InlineRule( RuleName ruleToInline, CommonGrammar grammar )
    {
        List<RuleDefinition> ruleToInlineDefinitions = grammar.Rules[ruleToInline].Definitions.ToList();
        
        bool hasReferenceToSelf = grammar.Rules[ruleToInline].Has( ruleToInline );
        bool canBeRemoved = !hasReferenceToSelf && grammar.StartRule != ruleToInline;
        if ( !canBeRemoved )
        {
            return false;
        }
        
        grammar.Rules.Remove( ruleToInline );
        
        bool hasChanges = false;
        foreach ( GrammarRule rule in grammar.Rules.Values )
        {
            var newDefinitions = new List<RuleDefinition>();
            var definitionsToRemove = new List<RuleDefinition>();
            
            foreach ( RuleDefinition definition in rule.Definitions )
            {
                for ( var index = 0; index < definition.Symbols.Count; index++ )
                {
                    RuleSymbol symbol = definition.Symbols[index];
                    if ( symbol.Type != RuleSymbolType.NonTerminalSymbol )
                    {
                        continue;
                    }

                    if ( symbol.RuleName != ruleToInline )
                    {
                        continue;
                    }

                    foreach ( RuleDefinition ruleToInlineDefinition in ruleToInlineDefinitions )
                    {
                        var newDefinition = definition.Symbols.ToList();
                        newDefinition.RemoveAt( index );
                        newDefinition.InsertRange( index, ruleToInlineDefinition.Symbols );
                        newDefinitions.Add( new RuleDefinition( newDefinition ) );
                    }
                    
                    definitionsToRemove.Add( definition );
                }
            }

            hasChanges = newDefinitions.Any();

            rule.Definitions = rule.Definitions.Where( x => !definitionsToRemove.Contains( x ) ).ToList();
            rule.Definitions.AddRange( newDefinitions );
        }
        
        return hasChanges;
    }
}