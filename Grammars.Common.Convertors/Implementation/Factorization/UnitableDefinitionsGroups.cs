using Grammars.Common.Grammars;
using Grammars.Common.Grammars.Extensions;
using Grammars.Common.Grammars.ValueObjects;
using Grammars.Common.Grammars.ValueObjects.GrammarRules;
using Grammars.Common.Grammars.ValueObjects.RuleDefinitions;
using Grammars.Common.Grammars.ValueObjects.Symbols;
using LinqExtensions;

namespace Grammars.Common.Convertors.Implementation.Factorization;

internal class UnitableDefinitionsGroups
{
    public readonly RuleName RuleName;
    public readonly HashSet<RuleSymbol> Headings;
    public readonly List<RuleDefinition> Definitions;

    public UnitableDefinitionsGroups( RuleName ruleName, HashSet<RuleSymbol> headings, IEnumerable<RuleDefinition> concreteDefinitions )
    {
        RuleName = ruleName;
        Headings = headings;
        Definitions = concreteDefinitions.ToList();
    }

    public static List<UnitableDefinitionsGroups> Create( RuleName targetRuleName, CommonGrammar grammar )
    {
        var groups = new List<UnitableDefinitionsGroups>();
        
        GrammarRule rule = grammar.Rules[targetRuleName];
        Dictionary<RuleDefinition, GuidingSymbolsSet> definitionsToHeadings = rule.Definitions.ToDictionary(
            definition => definition,
            definition => grammar.GetFirstSet( rule.Name, definition ) ); 
        LinkedList<RuleDefinition> definitionsToProcess = new LinkedList<RuleDefinition>().AddRangeToTail( rule.Definitions );

        while( definitionsToProcess.Any() )
        {
            RuleDefinition firstDefinition = definitionsToProcess.DequeueFirst();
            GuidingSymbolsSet firstDefinitionHeadings = definitionsToHeadings[firstDefinition];
            
            // Check for left recursion
            if ( firstDefinition.Symbols[0].Type == RuleSymbolType.NonTerminalSymbol && 
                 firstDefinition.Symbols[0].RuleName == targetRuleName )
            {
                continue;
            }
            
            List<RuleDefinition> toUnite = GetDefinitionsWithFirstSetIntersections( firstDefinitionHeadings, 
                definitionsToHeadings,
                definitionsToProcess );
            if ( !toUnite.Any() )
            {
                continue;
            }

            HashSet<RuleSymbol> commonHeadings = toUnite
                .SelectMany( x => definitionsToHeadings[x].GuidingSymbols )
                .ToHashSet();

            groups.Add( new UnitableDefinitionsGroups(
                rule.Name,
                commonHeadings,
                toUnite
                    .Append( firstDefinition )
                    .Select( definition => definition.Copy() ) ) );
        }

        return groups;
    }

    private static List<RuleDefinition> GetDefinitionsWithFirstSetIntersections(
        GuidingSymbolsSet firstDefinitionHeadings,
        Dictionary<RuleDefinition, GuidingSymbolsSet> definitionsToHeadings,
        LinkedList<RuleDefinition> definitionsToProcess )
    {
        var toUnite = new List<RuleDefinition>();

        LinkedListNode<RuleDefinition>? currentItem = definitionsToProcess.First;
        while ( currentItem != null )
        {
            RuleDefinition secondDefinition = currentItem.Value;
            GuidingSymbolsSet secondDefinitionHeadings = definitionsToHeadings[secondDefinition];

            LinkedListNode<RuleDefinition>? next = currentItem.Next;
            if ( firstDefinitionHeadings.HasIntersections( secondDefinitionHeadings ) )
            {
                toUnite.Add( secondDefinition );
                definitionsToProcess.Remove( currentItem );
            }

            currentItem = next;
        }

        return toUnite;
    }
}