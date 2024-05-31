using Grammars.Common.Grammars;
using Grammars.Common.Grammars.Extensions;
using Grammars.Common.Grammars.ValueObjects;
using Grammars.Common.Grammars.ValueObjects.GrammarRules;
using Grammars.Common.Grammars.ValueObjects.RuleDefinitions;
using Grammars.Common.Grammars.ValueObjects.RuleNames;
using Grammars.Common.Grammars.ValueObjects.Symbols;
using LinqExtensions;

namespace Grammars.Common.Convertors.Implementation.Factorization;

internal class UnitableDefinitionGroupsSearcher
{
    public List<UnitableDefinitionsGroups> Search( RuleName targetRuleName, CommonGrammar grammar )
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
                .Where( x => x.Type != RuleSymbolType.TerminalSymbol || x.Symbol!.Type != TerminalSymbolType.EmptySymbol )
                .ToHashSet();

            if ( commonHeadings.Any() )
            {
                groups.Add( new UnitableDefinitionsGroups(
                    rule.Name,
                    commonHeadings,
                    toUnite
                        .Append( firstDefinition )
                        .Select( definition => definition.Copy() ) ) );
            }
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