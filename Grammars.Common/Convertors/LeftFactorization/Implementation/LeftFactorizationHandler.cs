using System.Diagnostics;
using Grammars.Common.Convertors.LeftFactorization.Implementation.Inlinings;
using Grammars.Common.ValueObjects;
using Grammars.Common.ValueObjects.Symbols;
using LinqExtensions;

namespace Grammars.Common.Convertors.LeftFactorization.Implementation;

internal class LeftFactorizationHandler
{
    public CommonGrammar Factorize( CommonGrammar grammar )
    {
        Queue<RuleName> rulesToProcess = new Queue<RuleName>().EnqueueRange( grammar.Rules.Keys );

        while ( rulesToProcess.Any() )
        {
            GrammarRule rule = grammar.Rules[rulesToProcess.Dequeue()];
            
            Dictionary<RuleDefinition, GuidingSymbolsSet> definitionsToHeadings = BuildDefinitionsToHeadings( grammar, rule );

            List<UnitableDefinitionsGroups> unitableDefinitionsGroups = UnitableDefinitionsGroups.Create(
                rule.Name,
                grammar,
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
                
                grammar.Rules.Add( newRuleName, new GrammarRule( newRuleName, newRuleDefinitions ) );
                rulesToProcess.Enqueue( newRuleName );
            }

            grammar.Rules[rule.Name] = new GrammarRule( rule.Name, currentRuleDefinitions );
        }

        return grammar;
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

    private static Dictionary<RuleDefinition, GuidingSymbolsSet> BuildDefinitionsToHeadings( CommonGrammar grammar, GrammarRule rule )
    {
        return rule.Definitions.ToDictionary(
            x => x,
            x => grammar.GetGuidingSymbolsSet( rule.Name, x ) );
    }
}