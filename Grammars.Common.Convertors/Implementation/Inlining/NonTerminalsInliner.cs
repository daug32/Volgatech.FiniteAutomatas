using Grammars.Common.Grammars;
using Grammars.Common.Grammars.ValueObjects.GrammarRules;
using Grammars.Common.Grammars.ValueObjects.RuleDefinitions;

namespace Grammars.Common.Convertors.Implementation.Inlining;

internal class NonTerminalsInliner
{
    public CommonGrammar Inline( CommonGrammar grammar )
    {
        List<InlinableDefinition> concreteDefinitions = InlinableDefinition.CreateFromGrammar( grammar );

        bool hasChanges = true;
        while ( hasChanges )
        {
            hasChanges = false;

            for ( int index = 0; index < concreteDefinitions.Count; index++ )
            {
                InlinableDefinition definitionToInline = concreteDefinitions[index];
                if ( !definitionToInline.StartsWithTerminal )
                {
                    continue;
                }

                IEnumerable<InlinableDefinition> definitionsToProcess = concreteDefinitions.Where( x => x.CanInline( definitionToInline ) );
                
                foreach ( InlinableDefinition definition in definitionsToProcess )
                {
                    hasChanges = true;
                    definition.Inline( definitionToInline );
                }

                if ( CanBeRemoved( definitionToInline, concreteDefinitions ) )
                {
                    concreteDefinitions.RemoveAt( index );
                    index--;
                }
            }
        }

        return new CommonGrammar(
            grammar.StartRule, 
            concreteDefinitions.Select( x => new GrammarRule( x.RuleName, x.Definitions ) ) );
    }

    private bool CanBeRemoved( InlinableDefinition definitionToRemove, IEnumerable<InlinableDefinition> allDefinitions )
    {
        if ( definitionToRemove.IsStartRule )
        {
            return false;
        }
        
        foreach ( InlinableDefinition rule in allDefinitions )
        {
            foreach ( RuleDefinition definition in rule.Definitions )
            {
                if ( definition.Has( definitionToRemove.RuleName ) )
                {
                    return false;
                }
            }
        }

        return true;
    }
}