using Grammars.Common.Grammars;
using Grammars.Common.Grammars.ValueObjects.GrammarRules;
using Grammars.Common.Grammars.ValueObjects.RuleDefinitions;
using Grammars.Common.Grammars.ValueObjects.RuleNames;
using Grammars.Common.Grammars.ValueObjects.Symbols;
using LinqExtensions;

namespace Grammars.Common.Convertors.Implementation.Factorization;

internal class LeftFactorizationHandler
{
    private readonly RuleNameGenerator _ruleNameGenerator;
    private readonly UnitableDefinitionsSearcher _unitableDefinitionsSearcher;

    private readonly CommonGrammar _grammar;

    public LeftFactorizationHandler( CommonGrammar grammar )
    {
        _grammar = grammar;
        _ruleNameGenerator = new RuleNameGenerator( grammar );
        _unitableDefinitionsSearcher = new UnitableDefinitionsSearcher();
    }

    public CommonGrammar Factorize()
    {
        List<UnitableDefinitionsGroup> unitableGroups = _unitableDefinitionsSearcher.Search( _grammar );
        while ( unitableGroups.Any() )
        {
            foreach ( UnitableDefinitionsGroup group in unitableGroups )
            {
                UniteDefinitions( group );
            }

            unitableGroups = _unitableDefinitionsSearcher.Search( _grammar );
        }

        return _grammar;
    }

    private void UniteDefinitions( UnitableDefinitionsGroup unitableGroup )
    {
        GrammarRule newRule = new GrammarRule( _ruleNameGenerator.Next(), new List<RuleDefinition>() );

        GrammarRule currentRule = _grammar.Rules[unitableGroup.RuleName];
        currentRule.Definitions = currentRule.Definitions
            // Take only definitions that will not be united
            .Where( definition => !unitableGroup.Definitions.Contains( definition ) )
            // Include new rule
            .Append( new RuleDefinition( unitableGroup.CommonPrefix.Append( RuleSymbol.NonTerminalSymbol( newRule.Name ) ) ) )
            .ToList();

        foreach ( RuleDefinition oldDefinition in unitableGroup.Definitions )
        {
            List<RuleSymbol> migratedDefinition = oldDefinition.Symbols
                // Remove common prefix
                .Skip( unitableGroup.CommonPrefix.Count )
                .ToList();

            if ( !migratedDefinition.Any() )
            {
                migratedDefinition.Add( RuleSymbol.TerminalSymbol( TerminalSymbol.EmptySymbol() ) );
            }

            newRule.Definitions.Add( new RuleDefinition( migratedDefinition ) );
        }

        _grammar.Rules.Add( newRule.Name, newRule );
    }
}

internal class UnitableDefinitionsGroup
{
    public RuleName RuleName;
    public List<RuleSymbol> CommonPrefix;
    public List<RuleDefinition> Definitions;

    public UnitableDefinitionsGroup( RuleName ruleName, List<RuleSymbol> commonPrefix, List<RuleDefinition> definitions )
    {
        RuleName = ruleName;
        CommonPrefix = commonPrefix;
        Definitions = definitions;
    }
}

internal class UnitableDefinitionsSearcher
{
    public List<UnitableDefinitionsGroup> Search( CommonGrammar grammar )
    {
        var result = new List<UnitableDefinitionsGroup>();
        
        foreach ( GrammarRule rule in grammar.Rules.Values )
        {
            var unprocessedDefinitions = rule.Definitions.ToList();

            for ( var mainIndex = 0; mainIndex < unprocessedDefinitions.Count; mainIndex++ )
            {
                RuleDefinition mainDefinition = unprocessedDefinitions[mainIndex];

                int finalEndIndexOfCommonPrefix = -1;
                List<RuleDefinition> definitions = new List<RuleDefinition>().With( mainDefinition );
                
                for ( int secondIndex = mainIndex + 1; secondIndex < unprocessedDefinitions.Count; secondIndex++ )
                {
                    RuleDefinition secondDefinition = unprocessedDefinitions[secondIndex];
                    int endIndexOfCommonPrefix = -1;
                    
                    for ( int symbolIndex = 0; symbolIndex < mainDefinition.Symbols.Count; symbolIndex++ )
                    {
                        if ( symbolIndex >= secondDefinition.Symbols.Count )
                        {
                            break;
                        }

                        if ( secondDefinition.Symbols[symbolIndex] != mainDefinition.Symbols[symbolIndex] )
                        {
                            break;
                        }

                        endIndexOfCommonPrefix = symbolIndex;
                    }

                    if ( endIndexOfCommonPrefix > -1 )
                    {
                        finalEndIndexOfCommonPrefix = Math.Min( finalEndIndexOfCommonPrefix < 0 ? 0 : finalEndIndexOfCommonPrefix, endIndexOfCommonPrefix );
                        definitions.Add( secondDefinition );
                        
                        unprocessedDefinitions.RemoveAt( secondIndex );
                        secondIndex--;
                    }
                }

                if ( finalEndIndexOfCommonPrefix > -1 )
                {
                    List<RuleSymbol> commonPrefix = mainDefinition.Symbols.Take( finalEndIndexOfCommonPrefix + 1 ).ToList();
                    result.Add( new UnitableDefinitionsGroup( rule.Name, commonPrefix, definitions ) );
                    
                    unprocessedDefinitions.RemoveAt( mainIndex );
                    mainIndex--;
                }
            }
        }

        return result;
    } 
}