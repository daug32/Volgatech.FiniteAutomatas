using Grammars.Common.ValueObjects;

namespace Grammars.Common.Convertors.LeftFactorization.Implementation.Inlinings;

internal class BeggingNonTerminalsInliner
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
                
                bool hasInlinings = false;
                foreach ( ConcreteDefinition definition in definitionsToProcess )
                {
                    hasInlinings = true;
                    definition.Inline( definitionToInline );
                }

                hasChanges = hasChanges || hasInlinings;

                if ( hasInlinings )
                {
                    concreteDefinitions.RemoveAt( index );
                    index--;
                }
            }
        }

        var grammarRules = concreteDefinitions.Select( x => new GrammarRule( x.RuleName, x.Definitions ) );

        return new CommonGrammar( grammar.StartRule, grammarRules );
    }
}