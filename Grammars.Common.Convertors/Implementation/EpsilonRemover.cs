using Grammars.Common.Grammars;
using Grammars.Common.Grammars.Extensions;
using Grammars.Common.Grammars.ValueObjects;
using Grammars.Common.Grammars.ValueObjects.GrammarRules;
using Grammars.Common.Grammars.ValueObjects.RuleDefinitions;
using Grammars.Common.Grammars.ValueObjects.Symbols;
using LinqExtensions;

namespace Grammars.Common.Convertors.Implementation;

internal class EpsilonRemover
{
    public CommonGrammar RemoveEpsilons( CommonGrammar grammar )
    {
        var processedRules = new HashSet<RuleName>();

        while ( true )
        {
            GrammarRule? ruleWithEpsilon = GetRuleWithEpsilon( grammar, processedRules );
            if ( ruleWithEpsilon is null )
            {
                break;
            }

            bool hasNonEpsilonProductions = ruleWithEpsilon.Definitions
                .Any( definition =>
                    definition.FirstSymbol().Type != RuleSymbolType.TerminalSymbol
                    || definition.FirstSymbol().Symbol!.Type != TerminalSymbolType.EmptySymbol );

            if ( !hasNonEpsilonProductions )
            {
                RemoveRule( ruleWithEpsilon.Name, grammar );
                continue;
            }

            EnumerateRuleInUsages( ruleWithEpsilon.Name, grammar );
            RemoveEpsilonDefinition( ruleWithEpsilon );

            processedRules.Add( ruleWithEpsilon.Name );
        }

        return grammar;
    }

    private void RemoveEpsilonDefinition( GrammarRule ruleWithEpsilon )
    {
        ruleWithEpsilon.Definitions = ruleWithEpsilon.Definitions
            .Where( definition => !definition.Has( TerminalSymbolType.EmptySymbol ) )
            .ToList();
    }

    private void EnumerateRuleInUsages( RuleName name, CommonGrammar grammar )
    {
        foreach ( GrammarRule rule in grammar.Rules.Values )
        {
            for ( var definitionIndex = 0; definitionIndex < rule.Definitions.Count; definitionIndex++ )
            {
                RuleDefinition definition = rule.Definitions[definitionIndex];

                for ( var symbolIndex = 0; symbolIndex < definition.Symbols.Count; symbolIndex++ )
                {
                    RuleSymbol symbol = definition.Symbols[symbolIndex];
                    if ( symbol.Type != RuleSymbolType.NonTerminalSymbol || symbol.RuleName! != name )
                    {
                        continue;
                    }

                    List<RuleSymbol> newDefinition = definition.Symbols.ToListExcept( symbolIndex );
                    if ( !newDefinition.Any() )
                    {
                        newDefinition.Add( RuleSymbol.TerminalSymbol( TerminalSymbol.EmptySymbol() ) );
                    }

                    rule.Definitions.Add( new RuleDefinition( newDefinition ) );
                }
            }
        }
    }

    private void RemoveRule( RuleName ruleToRemoveName, CommonGrammar grammar )
    {
        foreach ( GrammarRule rule in grammar.Rules.Values )
        {
            if ( rule.Name == ruleToRemoveName )
            {
                continue;
            }

            for ( var index = 0; index < rule.Definitions.Count; index++ )
            {
                RuleDefinition definition = rule.Definitions[index];

                var newDefinition = new List<RuleSymbol>();
                foreach ( RuleSymbol symbol in definition.Symbols )
                {
                    if ( symbol.Type == RuleSymbolType.NonTerminalSymbol && symbol.RuleName! == ruleToRemoveName )
                    {
                        continue;
                    }

                    newDefinition.Add( symbol );
                }

                if ( !newDefinition.Any() )
                {
                    newDefinition.Add( RuleSymbol.TerminalSymbol( TerminalSymbol.EmptySymbol() ) );
                }

                rule.Definitions[index] = new RuleDefinition( newDefinition );
            }
        }

        grammar.Rules.Remove( ruleToRemoveName );
        grammar.RemoveAllDuplicateDefinitions();
        grammar.Validate();
    }

    private GrammarRule? GetRuleWithEpsilon( CommonGrammar grammar, HashSet<RuleName> processedRules )
    {
        foreach ( GrammarRule rule in grammar.Rules.Values )
        {
            if ( rule.Name == grammar.StartRule )
            {
                continue;
            }

            if ( processedRules.Contains( rule.Name ) )
            {
                continue;
            }

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
}