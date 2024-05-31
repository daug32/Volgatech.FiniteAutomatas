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
        _grammarRulesInliner.InlineRules( _grammar );

        HashSet<RuleName> ambiguousRules = GetAmbiguousRules();
        while ( ambiguousRules.Any() )
        {
            List<RuleName> rulesToProcess = ambiguousRules.ToList();

            foreach ( RuleName ruleName in rulesToProcess )
            {
                GrammarRule rule = _grammar.Rules[ruleName];

                List<UnitableDefinitionsGroups> definitionsGroupsToUnite = _unitableGroupsSearcher.Search( rule.Name, _grammar );

                var hadInlining = _grammarRulesInliner.InlineFirstSymbolsFromNonTerminals( definitionsGroupsToUnite, _grammar );
                if ( hadInlining )
                {
                    break;
                }

                foreach ( UnitableDefinitionsGroups definitionsGroup in definitionsGroupsToUnite )
                {
                    UniteDefinitions( definitionsGroup );
                }
            }

            ambiguousRules = GetAmbiguousRules();
        }

        return _grammar;
    }

    private void UniteDefinitions( UnitableDefinitionsGroups unitableGroup )
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
            RuleSymbol firstSymbol = oldDefinitionToMigrate.FirstSymbol();
            if ( firstSymbol.Type == RuleSymbolType.NonTerminalSymbol )
            {
                throw new ArgumentException( "ХУЙЦА САСНИ" );
            }
            
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
    }

    private HashSet<RuleName> GetAmbiguousRules()
    {
        var result = new HashSet<RuleName>();
        
        foreach ( GrammarRule rule in _grammar.Rules.Values )
        {
            if ( rule.Definitions.Count < 2 )
            {
                continue;
            }
            
            var definitionToFirstSet = rule.Definitions.ToDictionary(
                def => def,
                def => _grammar.GetFirstSet( rule.Name, def ).Exclude( RuleSymbol.TerminalSymbol( TerminalSymbol.EmptySymbol() ) ) );

            for ( int mainI = 0; mainI < rule.Definitions.Count; mainI++ )
            {
                RuleDefinition mainDef = rule.Definitions[mainI];
                if ( mainDef.Has( TerminalSymbolType.EmptySymbol ) )
                {
                    continue;
                }

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