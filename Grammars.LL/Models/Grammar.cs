using Grammars.Grammars.LeftRoRightOne.Models.ValueObjects;

namespace Grammars.Grammars.LeftRoRightOne.Models;

public class Grammar
{
    public RuleName StartRule { get; }
    public IReadOnlyDictionary<RuleName, GrammarRule> Rules { get; }

    public Grammar(
        RuleName startRule,
        IEnumerable<GrammarRule> rules )
    {
        StartRule = startRule;
        Rules = rules.ToDictionary(
            x => x.Name,
            x => x );

        ValidateOrThrow();
    }

    private void ValidateOrThrow()
    {
        var exceptions = new List<Exception>();
        
        foreach ( GrammarRule rule in Rules.Values )
        {
            if ( !rule.Values.Any() )
            {
                exceptions.Add( new ArgumentException( $"Rule must contain at least one rule value. RuleName: {rule.Name}" ) );
            }

            foreach ( RuleValue ruleValue in rule.Values )
            foreach ( RuleValueItem valueItem in ruleValue.Items )
            {
                if ( valueItem.Type != RuleValueItemType.NonTerminalSymbol )
                {
                    continue;
                }

                if ( !Rules.ContainsKey( valueItem.RuleName! ) )
                {
                    exceptions.Add( new ArgumentException(
                        $"There is a mention about a rule that is not presented in the given values. RuleName: {valueItem.RuleName}" ) );
                }
            }
        }

        if ( exceptions.Any() )
        {
            throw new AggregateException( "Grammar rules validation", exceptions );
        }
    }
}