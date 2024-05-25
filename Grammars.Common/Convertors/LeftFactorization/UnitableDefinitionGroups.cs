using Grammars.Common.ValueObjects;
using Grammars.Common.ValueObjects.Symbols;
using LinqExtensions;

namespace Grammars.Common.Convertors.LeftFactorization;

public class UnitableDefinitionGroups
{
    public HashSet<RuleSymbol> Headings;
    public List<ConcreteDefinition> ConcreteDefinitions;

    public UnitableDefinitionGroups( HashSet<RuleSymbol> headings, IEnumerable<ConcreteDefinition> concreteDefinitions )
    {
        Headings = headings;
        ConcreteDefinitions = concreteDefinitions.ToList();
    }

    public static List<UnitableDefinitionGroups> Create(
        RuleName targetRuleName,
        CommonGrammar grammar,
        Dictionary<RuleDefinition, GuidingSymbolsSet> definitionsToHeadings )
    {
        var groups = new List<UnitableDefinitionGroups>();
        
        GrammarRule rule = grammar.Rules[targetRuleName];
        LinkedList<RuleDefinition> definitionsToProcess = new LinkedList<RuleDefinition>().AddRangeToTail( rule.Definitions );

        while( definitionsToProcess.Any() )
        {
            RuleDefinition firstDefinition = definitionsToProcess.DequeueFirst();
            if ( firstDefinition.Symbols[0].Type == RuleSymbolType.NonTerminalSymbol && 
                 firstDefinition.Symbols[0].RuleName == targetRuleName )
            {
                continue;
            }

            GuidingSymbolsSet firstDefinitionHeadings = definitionsToHeadings[firstDefinition];
            
            var toUnite = new List<RuleDefinition>();
            HashSet<RuleSymbol> toUniteCommonHeadings = firstDefinitionHeadings.GuidingSymbols.ToHashSet();

            LinkedListNode<RuleDefinition>? currentItem = definitionsToProcess.First;
            while ( currentItem != null )
            {
                RuleDefinition secondDefinition = currentItem.Value;
                GuidingSymbolsSet secondDefinitionHeadings = definitionsToHeadings[secondDefinition];

                LinkedListNode<RuleDefinition>? next = currentItem.Next;
                if ( firstDefinitionHeadings.HasIntersections( secondDefinitionHeadings ) )
                {
                    toUnite.Add( secondDefinition );
                    toUniteCommonHeadings.IntersectWith( secondDefinitionHeadings.GuidingSymbols );
                    definitionsToProcess.Remove( currentItem );
                }

                currentItem = next;
            }

            if ( !toUnite.Any() )
            {
                continue;
            }

            groups.Add( new UnitableDefinitionGroups( 
                toUniteCommonHeadings,
                toUnite
                    .With( firstDefinition )
                    .Select( definition => new ConcreteDefinition( rule.Name, definition ) ) ) );
        }

        return groups;
    }
}