using Grammars.Common.Grammars;
using Grammars.Common.Grammars.Extensions;
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

        var hasChanges = true;
        while ( hasChanges )
        {
            hasChanges = false;

            var grammarRules = _grammar.Rules.Keys.ToList();
            for ( var index = 0; index < grammarRules.Count; index++ )
            {
                GrammarRule rule = _grammar.Rules[grammarRules[index]];

                var unitableGroups = _unitableGroupsSearcher.Search( rule.Name, _grammar );

                hasChanges |= _grammarRulesInliner.InlineFirstSymbolsFromNonTerminals( unitableGroups, _grammar );

                foreach ( UnitableDefinitionsGroups unitableGroup in unitableGroups )
                {
                    hasChanges = true;

                    RuleName newRuleName = UniteDefinitions( _grammar, unitableGroup );
                    grammarRules.Add( newRuleName );
                }
            }
        }

        _grammar.RemoveAllDuplicateDefinitions();

        return _grammar;
    }

    private RuleName UniteDefinitions(
        CommonGrammar grammar,
        UnitableDefinitionsGroups unitableGroup )
    {
        var newRule = new GrammarRule( _ruleNameGenerator.Next(), new List<RuleDefinition>() );

        RuleSymbol heading = unitableGroup.Headings.First();

        grammar.Rules[unitableGroup.RuleName].Definitions = grammar.Rules[unitableGroup.RuleName]
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

        grammar.Rules.Add( newRule.Name, new GrammarRule( newRule.Name, newRule.Definitions ) );

        return newRule.Name;
    }
}