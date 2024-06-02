using System.Collections.Immutable;
using Grammars.Common.Grammars;
using Grammars.Common.Grammars.Extensions;
using Grammars.Common.Grammars.ValueObjects.GrammarRules;
using Grammars.Common.Grammars.ValueObjects.RuleDefinitions;
using Grammars.Common.Grammars.ValueObjects.RuleNames;
using Grammars.Common.Grammars.ValueObjects.Symbols;

namespace Grammars.LL.Models;

public class LlOneGrammar : CommonGrammar
{
    public LlOneGrammar( RuleName startRule, IEnumerable<GrammarRule> rules )
        : base( startRule, rules )
    {
        AssumeRulesDoesntHaveAmbiguousDefinitions( this );
    }

    private void AssumeRulesDoesntHaveAmbiguousDefinitions( CommonGrammar grammar )
    {
        foreach ( GrammarRule rule in grammar.Rules.Values )
        {
            List<(RuleDefinition Definition, IImmutableSet<RuleSymbol> FirstSet)> headingSymbols = rule.Definitions
                .Select( definition => 
                    (
                        Definition: definition,
                        FirstSet: grammar.GetFirstSet( rule.Name, definition ).GuidingSymbols
                    ) )
                .ToList();

            List<(RuleDefinition Definition, IImmutableSet<RuleSymbol> FirstSet)> duplicateHeadingSets = FindDuplicateHeadingSets( headingSymbols );

            if ( duplicateHeadingSets.Any() )
            {
                string serializedDefinitions = String.Join(
                    "\n",
                    duplicateHeadingSets.Select( definition =>
                    {
                        string serializedDefinition = String.Join( " ", definition.Definition.Symbols );
                        string serializedSet = String.Join( ",", definition.FirstSet );
                        return $"<{rule.Name}> -> {serializedDefinition}\tFirstSet: {serializedSet}";
                    } ) );
                
                throw new ArgumentException(
                    $"Rule {rule.Name} has duplicate FIRST for different definitions\n{serializedDefinitions}" );
            }
        }
    }

    private static List<(RuleDefinition Definition, IImmutableSet<RuleSymbol> FirstSet)> FindDuplicateHeadingSets(
        List<(RuleDefinition Definition, IImmutableSet<RuleSymbol> FirstSet)> definitionsWithFirstSet )
    {
        var result = new List<(RuleDefinition Definition, IImmutableSet<RuleSymbol> FirstSet)>();

        for ( var mainIterator = 0; mainIterator < definitionsWithFirstSet.Count; mainIterator++ )
        {
            var mainDefinition = definitionsWithFirstSet[mainIterator];

            var duplicates = new List<(RuleDefinition Definition, IImmutableSet<RuleSymbol> FirstSet)>()
            {
                mainDefinition
            };
            
            for ( int secondIterator = mainIterator + 1; secondIterator < definitionsWithFirstSet.Count; secondIterator++ )
            {
                var secondDefinition = definitionsWithFirstSet[secondIterator];
                if ( secondDefinition.FirstSet.SetEquals( mainDefinition.FirstSet ) )
                {
                    duplicates.Add( secondDefinition );
                    definitionsWithFirstSet.RemoveAt( secondIterator );
                    secondIterator--;
                }
            }

            if ( duplicates.Count > 1 )
            {
                result.AddRange( duplicates );
                definitionsWithFirstSet.RemoveAt( mainIterator );
                mainIterator--;
            }
        }

        return result
            .OrderBy( x => String.Join( 
                "", 
                x.FirstSet
                    .OrderBy( i => i.ToString() )
                    .Select( i => i.ToString() ) ) )
            .ToList();
    }
}