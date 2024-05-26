using System.Diagnostics;
using Grammars.Common.Convertors.LeftFactorization.Implementation.Inlinings;
using Grammars.Common.Convertors.LeftRecursions.Implementation;
using Grammars.Common.ValueObjects;
using Grammars.Common.ValueObjects.Symbols;
using LinqExtensions;

namespace Grammars.Common.Convertors.LeftFactorization.Implementation;

internal class LeftFactorizationHandler
{
    public CommonGrammar Factorize( MutableGrammar mutableGrammar )
    {
        Queue<RuleName> rulesToProcess = new Queue<RuleName>().EnqueueRange( mutableGrammar.Rules.Keys );

        while ( rulesToProcess.Any() )
        {
            GrammarRule rule = mutableGrammar.Rules[rulesToProcess.Dequeue()];
            
            Dictionary<RuleDefinition, GuidingSymbolsSet> definitionsToHeadings = BuildDefinitionsToHeadings( mutableGrammar, rule );

            List<UnitableDefinitionsGroups> unitableDefinitionsGroups = UnitableDefinitionsGroups.Create(
                rule.Name,
                mutableGrammar,
                definitionsToHeadings );

            var currentRuleDefinitions = rule.Definitions.ToList();
            foreach ( UnitableDefinitionsGroups definitionGroup in unitableDefinitionsGroups )
            {
                var newRuleName = RuleName.Random();
                var newRuleDefinitions = new List<RuleDefinition>();
                
                RuleSymbol heading = definitionGroup.Headings.First();

                currentRuleDefinitions = currentRuleDefinitions
                    .Where( definition => !NeedToBeRemoved( definition, definitionGroup.Definitions ) )
                    .ToList()
                    .With( new RuleDefinition( new []
                    {
                        heading,
                        RuleSymbol.NonTerminalSymbol( newRuleName ), 
                    } ) );
                
                Debug.WriteLine( $"Definitions: {currentRuleDefinitions.Count.ToString()}" );

                foreach ( RuleDefinition oldDefinitionToMigrate in definitionGroup.Definitions )
                {
                    RuleSymbol firstSymbol = oldDefinitionToMigrate.Symbols.First();
                    if ( firstSymbol.Type == RuleSymbolType.NonTerminalSymbol )
                    {
                        throw new NotSupportedException(
                            "LeftFactorization can not be performed "
                            + "because can not get prefixes from the given non terminal symbol. "
                            + $"{nameof( BeggingNonTerminalsInliner )} had to prepare data but something went wrong. "
                            + $"NonTerminalSymbol: {firstSymbol}. "
                            + "Possible invalid data example: A -> B | a c; B -> a b " );
                    }

                    var migratedDefinition = oldDefinitionToMigrate.Symbols.ToList().WithoutFirst();
                    if ( firstSymbol.Type == RuleSymbolType.TerminalSymbol && !migratedDefinition.Any() )
                    {
                        migratedDefinition.Add(
                            RuleSymbol.TerminalSymbol( 
                                TerminalSymbol.EmptySymbol() ) );
                    }
                    
                    newRuleDefinitions.Add( new RuleDefinition( migratedDefinition ) );
                }
                
                mutableGrammar.Rules.Add( newRuleName, new GrammarRule( newRuleName, newRuleDefinitions ) );
                rulesToProcess.Enqueue( newRuleName );
            }

            mutableGrammar.Rules[rule.Name] = new GrammarRule( rule.Name, currentRuleDefinitions );
        }

        return mutableGrammar.ToGrammar();
    }

    private bool NeedToBeRemoved( RuleDefinition definition, List<RuleDefinition> definitionToRemove )
    {
        foreach ( RuleDefinition ruleDefinition in definitionToRemove )
        {
            if ( ruleDefinition.Equals( definition ) )
            {
                return true;
            }
        }

        return false;
    }

    private static Dictionary<RuleDefinition, GuidingSymbolsSet> BuildDefinitionsToHeadings( MutableGrammar grammar, GrammarRule rule )
    {
        return rule.Definitions.ToDictionary(
            x => x,
            x => grammar.GetGuidingSymbolsSet( rule.Name, x ) );
    }
}