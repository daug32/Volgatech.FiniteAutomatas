using Grammars.Common.Grammars;
using Grammars.Common.Grammars.ValueObjects.GrammarRules;
using Grammars.Common.Grammars.ValueObjects.RuleDefinitions;
using Grammars.Common.Grammars.ValueObjects.RuleNames;
using Grammars.Common.Grammars.ValueObjects.Symbols;

namespace Grammars.SLR.Models;

public class SlrOneGrammar : CommonGrammar
{
    public SlrOneGrammar( RuleName startRule, IEnumerable<GrammarRule> rules )
        : base( startRule, rules )
    {
        AssumeNoEmptySymbolsAreUsed( this );
        AssumeStartRuleIsNotUsedAnywhere( this );
    }

    private void AssumeStartRuleIsNotUsedAnywhere( CommonGrammar grammar )
    {
        RuleName startRule = grammar.StartRule;
        var startRuleUsages = new List<RuleName>();
        foreach ( GrammarRule rule in grammar.Rules.Values )
        {
            if ( rule.Has( startRule ) )
            {
                startRuleUsages.Add( rule.Name );
            }
        }

        if ( !startRuleUsages.Any() )
        {
            return;
        }

        string serializedRules = String.Join(
            "\n",
            startRuleUsages.Select( ruleName =>
            {
                string serializedDefinitions = String.Join(
                    "|",
                    grammar.Rules[ruleName]
                        .Definitions.Select( definition =>
                        {
                            var serializedSymbols = definition.Symbols.Select( symbol => symbol.ToString() );
                            return String.Join( " ", serializedSymbols );
                        } ) );

                return $"<{ruleName}> -> {serializedDefinitions}";
            } ) );

        throw new ArgumentException( $"Start rule must not be referenced in any rule. Rules using the start rule:\n{serializedRules}" );
    }

    private void AssumeNoEmptySymbolsAreUsed( CommonGrammar grammar )
    {
        foreach ( GrammarRule rule in grammar.Rules.Values )
        {
            var definitionsWithEmptySymbol = new List<RuleDefinition>();
            foreach ( RuleDefinition definition in rule.Definitions )
            {
                if ( !definition.Has( TerminalSymbolType.EmptySymbol ) )
                {
                    continue;
                }

                definitionsWithEmptySymbol.Add( definition );
            }

            if ( !definitionsWithEmptySymbol.Any() )
            {
                continue;
            }

            string serializedDefinitions = String.Join(
                "\n",
                definitionsWithEmptySymbol.Select( definition =>
                {
                    var serializedSymbols = definition.Symbols.Select( symbol => symbol.ToString() );
                    return $"<{rule.Name}> -> {String.Join( " ", serializedSymbols )}";
                } ) );

            throw new ArgumentException(
                $"Rule {rule.Name} has epsilon symbol at on eof its definitions.\n{serializedDefinitions}" );
        }
    }
}