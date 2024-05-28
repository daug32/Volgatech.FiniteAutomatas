using System.Diagnostics;
using Grammars.Common.Grammars;
using Grammars.Common.Grammars.Extensions;
using Grammars.Common.Grammars.ValueObjects;
using Grammars.Common.Grammars.ValueObjects.GrammarRules;
using Grammars.Common.Grammars.ValueObjects.RuleDefinitions;
using Grammars.Common.Grammars.ValueObjects.Symbols;
using LinqExtensions;

namespace Grammars.Common.Convertors.Implementation.Factorization;

internal class LeftFactorizationHandler
{
    public CommonGrammar Factorize( CommonGrammar grammar )
    {
        Queue<RuleName> processedRules = new Queue<RuleName>();
        Queue<RuleName> rulesToProcess = new Queue<RuleName>().EnqueueRange( grammar.Rules.Keys );

        while ( rulesToProcess.Any() )
        {
            RuleName ruleToProcessName = rulesToProcess.Dequeue();
            if ( processedRules.Contains( ruleToProcessName ) )
            {
                continue;
            }

            processedRules.Enqueue( ruleToProcessName );
            
            GrammarRule rule = grammar.Rules[ruleToProcessName];
            
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
                        // Я не ебу почему но оно так надо
                        newRuleDefinitions.Add( oldDefinitionToMigrate.Copy() );
                        continue;
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
        
        grammar.RemoveAllDuplicateDefinitions();

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
            x => grammar.GetFirstSet( rule.Name, x ) );
    }
}