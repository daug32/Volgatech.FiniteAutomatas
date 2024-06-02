using Grammars.Common.Grammars;
using Grammars.Common.Grammars.ValueObjects.GrammarRules;
using Grammars.Common.Grammars.ValueObjects.RuleDefinitions;
using Grammars.Common.Grammars.ValueObjects.RuleNames;
using Grammars.Common.Grammars.ValueObjects.Symbols;

namespace Grammars.SLR.Convertors.Implementation;

internal class RemoveReferencesToStartRuleHandler
{
    private readonly RuleNameGenerator _ruleNameGenerator;
    private readonly CommonGrammar _grammar;

    public RemoveReferencesToStartRuleHandler( CommonGrammar grammar )
    {
        _grammar = grammar;
        _ruleNameGenerator = new RuleNameGenerator( grammar );
    }
    
    public CommonGrammar RemoveReferencesToStartRule()
    {
        if ( !HasReferencesToStartRule() )
        {
            return _grammar;
        }

        var newStartRule = new GrammarRule(
            _ruleNameGenerator.Next(),
            new List<RuleDefinition>
            {
                new RuleDefinition( new[] { RuleSymbol.NonTerminalSymbol( _grammar.StartRule ) } )   
            } );

        return new CommonGrammar(
            newStartRule.Name,
            _grammar.Rules.Values.Append( newStartRule ) );
    }
    
    private bool HasReferencesToStartRule()
    {
        foreach ( GrammarRule rule in _grammar.Rules.Values )
        {
            if ( rule.Has( _grammar.StartRule ) )
            {
                return true;
            }
        }

        return false;
    }
}