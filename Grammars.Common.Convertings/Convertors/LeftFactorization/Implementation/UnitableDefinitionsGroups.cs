using Grammars.Common.Extensions.Grammar.ValuesObjects;
using Grammars.Common.ValueObjects;
using Grammars.Common.ValueObjects.Symbols;
using LinqExtensions;

namespace Grammars.Common.Convertings.Convertors.LeftFactorization.Implementation;

internal class UnitableDefinitionsGroups
{
    public HashSet<RuleSymbol> Headings;
    public List<RuleDefinition> Definitions;

    public UnitableDefinitionsGroups( HashSet<RuleSymbol> headings, IEnumerable<RuleDefinition> concreteDefinitions )
    {
        Headings = headings;
        Definitions = concreteDefinitions.ToList();
    }

    public static List<UnitableDefinitionsGroups> Create(
        RuleName targetRuleName,
        CommonGrammar grammar,
        Dictionary<RuleDefinition, GuidingSymbolsSet> definitionsToHeadings )
    {
        var groups = new List<UnitableDefinitionsGroups>();
        
        GrammarRule rule = grammar.Rules[targetRuleName];
        LinkedList<RuleDefinition> definitionsToProcess = new LinkedList<RuleDefinition>().AddRangeToTail( rule.Definitions );

        while( definitionsToProcess.Any() )
        {
            RuleDefinition firstDefinition = definitionsToProcess.DequeueFirst();
            GuidingSymbolsSet firstDefinitionHeadings = definitionsToHeadings[firstDefinition];
            
            if ( firstDefinition.Symbols[0].Type == RuleSymbolType.NonTerminalSymbol && 
                 firstDefinition.Symbols[0].RuleName == targetRuleName )
            {
                continue;
            }
            
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

            groups.Add( new UnitableDefinitionsGroups( 
                toUniteCommonHeadings,
                toUnite
                    .With( firstDefinition )
                    .Select( definition => new RuleDefinition( definition.Symbols ) ) ) );
        }

        return groups;
    }
}