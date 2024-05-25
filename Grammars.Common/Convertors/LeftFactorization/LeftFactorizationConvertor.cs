using Grammars.Common.ValueObjects;

namespace Grammars.Common.Convertors.LeftFactorization;

public class LeftFactorizationConvertor : IGrammarConvertor
{
    public CommonGrammar Convert( CommonGrammar grammar )
    {
        return new LeftFactorizationHandler().Factorize( grammar );
    }
}

public class LeftFactorizationHandler
{
    public CommonGrammar Factorize( CommonGrammar grammar )
    {
        List<GrammarRule> rules = grammar.Rules.Values.ToList();

        foreach ( GrammarRule rule in rules )
        {
            Dictionary<RuleDefinition, GuidingSymbolsSet> definitionsToHeadings = BuildDefinitionsToHeadings( grammar, rule );

            List<UnitableDefinitionGroups> unitableDefinitionGroups = UnitableDefinitionGroups.Create(
                rule.Name,
                grammar,
                definitionsToHeadings );
        }

        return null;
    }

    private static Dictionary<RuleDefinition, GuidingSymbolsSet> BuildDefinitionsToHeadings( CommonGrammar grammar, GrammarRule rule )
    {
        return rule.Definitions.ToDictionary(
            x => x,
            x => grammar.GetGuidingSymbolsSet( rule.Name, x ) );
    }
}