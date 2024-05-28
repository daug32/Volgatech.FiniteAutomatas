using Grammars.Common.ValueObjects;
using Grammars.Common.ValueObjects.Symbols;

namespace Grammars.Common.Convertings.Convertors.Inlinings.Implementation;

internal class NonTerminalsInliner
{
    public CommonGrammar Inline( CommonGrammar grammar )
    {
        List<ConcreteDefinition> concreteDefinitions = ConcreteDefinition.CreateFromGrammar( grammar );

        bool hasChanges = true;
        while ( hasChanges )
        {
            hasChanges = false;

            for ( int index = 0; index < concreteDefinitions.Count; index++ )
            {
                ConcreteDefinition definitionToInline = concreteDefinitions[index];
                if ( !definitionToInline.StartsWithTerminal )
                {
                    continue;
                }

                IEnumerable<ConcreteDefinition> definitionsToProcess = concreteDefinitions.Where( x => x.CanInline( definitionToInline ) );
                
                foreach ( ConcreteDefinition definition in definitionsToProcess )
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

    private bool CanBeRemoved( ConcreteDefinition definitionToRemove, IEnumerable<ConcreteDefinition> allDefinitions )
    {
        if ( definitionToRemove.IsStartRule )
        {
            return false;
        }
        
        foreach ( ConcreteDefinition rule in allDefinitions )
        {
            foreach ( RuleDefinition definition in rule.Definitions )
            {
                foreach ( RuleSymbol symbol in definition.Symbols )
                {
                    if ( symbol.RuleName == definitionToRemove.RuleName )
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }
}