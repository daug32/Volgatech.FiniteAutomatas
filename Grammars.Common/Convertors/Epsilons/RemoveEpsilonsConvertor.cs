using System.Security;
using Grammars.Common.Convertors.LeftFactorization.Implementation.Inlinings;
using Grammars.Common.Extensions;
using Grammars.Common.ValueObjects;
using Grammars.Common.ValueObjects.Symbols;
using LinqExtensions;

namespace Grammars.Common.Convertors.Epsilons;

public class RemoveEpsilonsConvertor : IGrammarConvertor
{
    public CommonGrammar Convert( CommonGrammar grammar )
    {
        return RemoveEpsilons( grammar );
    }

    private CommonGrammar RemoveEpsilons( CommonGrammar grammar )
    {
        while ( true )
        {
            GrammarRule? ruleWithEpsilon = GetRuleWithEpsilon( grammar );
            if ( ruleWithEpsilon is null )
            {
                break;
            }

            bool hasNonEpsilonProductions = ruleWithEpsilon.Definitions
                .Any( definition =>
                    definition.FirstSymbol().Type != RuleSymbolType.TerminalSymbol ||
                    definition.FirstSymbol().Symbol!.Type != TerminalSymbolType.EmptySymbol );

            List<RuleName> usages = GetRulesUsingTargetRule( ruleWithEpsilon.Name, grammar );

            foreach ( RuleName usage in usages )
            {
                GrammarRule rule = grammar.Rules[usage];

                for ( var definitionIndex = 0; definitionIndex < rule.Definitions.Count; definitionIndex++ )
                {
                    RuleDefinition definition = rule.Definitions[definitionIndex];

                    if ( !hasNonEpsilonProductions )
                    {
                        rule.Definitions = rule.Definitions
                            .Where( x => !x.Has( ruleWithEpsilon.Name ) )
                            .ToList();
                        continue;
                    }

                    for ( var symbolIndex = 0; symbolIndex < definition.Symbols.Count; symbolIndex++ )
                    {
                        RuleSymbol symbol = definition.Symbols[symbolIndex];

                        bool symbolWithRuleToRemove =
                            symbol.Type == RuleSymbolType.NonTerminalSymbol &&
                            symbol.RuleName == ruleWithEpsilon.Name;

                        if ( symbolWithRuleToRemove )
                        {
                            rule.Definitions.Add( new RuleDefinition( definition.Symbols.ToListExcept( symbolIndex ) ) );
                        }
                    }
                }
            }

            ruleWithEpsilon.Definitions = ruleWithEpsilon.Definitions.Where( x => !x.Has( TerminalSymbolType.EmptySymbol ) ).ToList();
        }

        return grammar;
    }

    private GrammarRule? GetRuleWithEpsilon( CommonGrammar grammar )
    {
        foreach ( GrammarRule rule in grammar.Rules.Values )
        {
            foreach ( RuleDefinition definition in rule.Definitions )
            {
                foreach ( RuleSymbol symbol in definition.Symbols )
                {
                    if ( symbol.Type != RuleSymbolType.TerminalSymbol )
                    {
                        continue;
                    }

                    if ( symbol.Symbol!.Type == TerminalSymbolType.EmptySymbol )
                    {
                        return rule;
                    }
                }
            }
        }

        return null;
    }

    public List<RuleName> GetRulesUsingTargetRule( RuleName targetRule, CommonGrammar grammar )
    {
        var result = new List<RuleName>();
        
        foreach ( GrammarRule rule in grammar.Rules.Values )
        {
            foreach ( RuleDefinition ruleDefinition in rule.Definitions )
            {
                foreach ( RuleSymbol symbol in ruleDefinition.Symbols )
                {
                    if ( symbol.Type != RuleSymbolType.NonTerminalSymbol )
                    {
                        continue;
                    }

                    if ( symbol.RuleName! == targetRule )
                    {
                        result.Add( rule.Name );
                    }
                }
            }
        }

        return result;
    }
}