using Grammars.Common.Grammars;
using Grammars.Common.Grammars.Extensions;
using Grammars.Common.Grammars.ValueObjects;
using Grammars.Common.Grammars.ValueObjects.GrammarRules;
using Grammars.Common.Grammars.ValueObjects.RuleDefinitions;
using Grammars.Common.Grammars.ValueObjects.RuleNames;
using Grammars.Common.Grammars.ValueObjects.Symbols;
using LinqExtensions;

namespace Grammars.Common.Convertors.Implementation.Factorization;

internal class LeftFactorizationHandler
{
    private readonly UnitableDefinitionGroupsSearcher _unitableGroupsSearcher = new();
    private readonly GrammarRulesInliner _grammarRulesInliner = new();
    private readonly RuleNameGenerator _ruleNameGenerator;
    private readonly CommonGrammar _grammar;

    public LeftFactorizationHandler( CommonGrammar grammar )
    {
        _grammar = grammar;
        _ruleNameGenerator = new RuleNameGenerator( grammar );
    }

    public CommonGrammar Factorize()
    {
        _grammarRulesInliner.InlineRulesWithOnlyTerminals( _grammar );

        HashSet<RuleName> ambiguousRules = GetAmbiguousRules();
        while ( ambiguousRules.Any() )
        {
            var rulesToProcess = ambiguousRules.ToList();
            
            for ( var index = 0; index < rulesToProcess.Count; index++ )
            {
                GrammarRule rule = _grammar.Rules[rulesToProcess[index]];

                List<UnitableDefinitionsGroups> unitableGroups = _unitableGroupsSearcher.Search( rule.Name, _grammar );
                _grammarRulesInliner.InlineFirstSymbolsFromNonTerminals( unitableGroups, _grammar );

                foreach ( UnitableDefinitionsGroups unitableGroup in unitableGroups )
                {
                    RuleName newRuleName = UniteDefinitions( unitableGroup );
                    rulesToProcess.Add( newRuleName );
                }
            }

            ambiguousRules = GetAmbiguousRules();
        }

        return _grammar;
    }

    private RuleName UniteDefinitions( UnitableDefinitionsGroups unitableGroup )
    {
        var newRule = new GrammarRule( _ruleNameGenerator.Next(), new List<RuleDefinition>() );

        RuleSymbol heading = unitableGroup.Headings.First();

        _grammar.Rules[unitableGroup.RuleName].Definitions = _grammar.Rules[unitableGroup.RuleName]
            .Definitions
            .Where( definition => !unitableGroup.Definitions.Contains( definition ) )
            .ToList()
            .With( new RuleDefinition( new[]
            {
                heading,
                RuleSymbol.NonTerminalSymbol( newRule.Name )
            } ) );

        foreach ( RuleDefinition oldDefinitionToMigrate in unitableGroup.Definitions )
        {
            var migratedDefinition = oldDefinitionToMigrate.Symbols.ToList().WithoutFirst();
            if ( !migratedDefinition.Any() )
            {
                migratedDefinition.Add(
                    RuleSymbol.TerminalSymbol(
                        TerminalSymbol.EmptySymbol() ) );
            }

            newRule.Definitions.Add( new RuleDefinition( migratedDefinition ) );
        }

        _grammar.Rules.Add( newRule.Name, new GrammarRule( newRule.Name, newRule.Definitions ) );

        return newRule.Name;
    }

    private HashSet<RuleName> GetAmbiguousRules()
    {
        var result = new HashSet<RuleName>();
        
        foreach ( GrammarRule rule in _grammar.Rules.Values )
        {
            var definitionToFirstSet = rule.Definitions.ToDictionary(
                def => def,
                def => _grammar.GetFirstSet( rule.Name, def ) );

            for ( int mainI = 0; mainI < rule.Definitions.Count; mainI++ )
            {
                RuleDefinition mainDef = rule.Definitions[mainI];

                bool hasAmbiguousReferences = false;
                for ( int secondI = mainI + 1; secondI < rule.Definitions.Count; secondI++ )
                {
                    RuleDefinition secondDef = rule.Definitions[secondI];

                    if ( definitionToFirstSet[mainDef].HasIntersections( definitionToFirstSet[secondDef] ) )
                    {
                        hasAmbiguousReferences = true;
                        break;
                    }
                }

                if ( hasAmbiguousReferences )
                {
                    result.Add( rule.Name );
                    break;
                }
            }
        }
        
        return result;
    }
}