using Grammars.Common.Convertors.LeftRecursions.Implementation;
using Grammars.Common.ValueObjects;
using Grammars.Common.ValueObjects.Symbols;
using LinqExtensions;

namespace Grammars.Common.Convertors.LeftFactorization;

public class LeftFactorizationConvertor : IGrammarConvertor
{
    public CommonGrammar Convert( CommonGrammar grammar )
    {
        return new LeftFactorizationHandler().Factorize( new MutableGrammar( grammar ) );
    }
}

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

                foreach ( RuleDefinition oldDefinitionToMigrate in definitionGroup.Definitions )
                {
                    RuleSymbol firstSymbol = oldDefinitionToMigrate.Symbols.First();
                    if ( firstSymbol.Type == RuleSymbolType.NonTerminalSymbol )
                    {
                        throw new NotSupportedException();
                    }

                    newRuleDefinitions.Add( new RuleDefinition( oldDefinitionToMigrate.Symbols.ToList().WithoutFirst() ) );
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

    private static Dictionary<RuleDefinition, GuidingSymbolsSet> BuildDefinitionsToHeadings( CommonGrammar grammar, GrammarRule rule )
    {
        return rule.Definitions.ToDictionary(
            x => x,
            x => grammar.GetGuidingSymbolsSet( rule.Name, x ) );
    }
}