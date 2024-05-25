﻿using Grammars.Common.ValueObjects;
using Grammars.Common.ValueObjects.Symbols;

namespace Grammars.Common.Convertors.Semantics;

public class RuleNamesConvertor : IGrammarConvertor
{
    public CommonGrammar Convert( CommonGrammar grammar )
    {
        var oldNameToNewName = new Dictionary<RuleName, RuleName>();
        
        int index = 1;
        foreach ( RuleName ruleName in grammar.Rules.Keys )
        {
            oldNameToNewName[ruleName] = new RuleName( index.ToString() );
            index++;
        }

        var newRules = new List<GrammarRule>();
        foreach ( RuleName ruleName in grammar.Rules.Keys )
        {
            GrammarRule rule = grammar.Rules[ruleName];

            var newDefinitions = new List<RuleDefinition>();
            foreach ( RuleDefinition definition in rule.Definitions )
            {
                var symbols = new List<RuleSymbol>();
                foreach ( RuleSymbol symbol in definition.Symbols )
                {
                    symbols.Add( symbol.Type == RuleSymbolType.NonTerminalSymbol
                        ? RuleSymbol.NonTerminalSymbol( oldNameToNewName[symbol.RuleName!] ) 
                        : symbol );
                } 
                
                newDefinitions.Add( new RuleDefinition( symbols ) );
            }

            newRules.Add( new GrammarRule( oldNameToNewName[ruleName], newDefinitions ) );
        }

        return new CommonGrammar( oldNameToNewName[grammar.StartRule], newRules );
    }
}