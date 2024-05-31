using Grammars.Common.Grammars;
using Grammars.Common.Grammars.Extensions;
using Grammars.Common.Grammars.ValueObjects;
using Grammars.Common.Grammars.ValueObjects.GrammarRules;
using Grammars.Common.Grammars.ValueObjects.RuleDefinitions;
using Grammars.Common.Grammars.ValueObjects.RuleNames;
using Grammars.Common.Grammars.ValueObjects.Symbols;
using Grammars.LL.Models;
using Grammars.LL.Runners.Implementation;

namespace Grammars.LL.Runners;

public class ParsingTableCreator
{
    public ParsingTable Create( LlOneGrammar grammar )
    {
        HashSet<TerminalSymbol> alphabet = GetAlphabet( grammar );

        var table = new ParsingTable( grammar.StartRule, alphabet );
        foreach ( TerminalSymbol terminalSymbol in alphabet )
        {
            table[terminalSymbol] = new Dictionary<RuleName, RuleDefinition>();
        }

        List<ConcreteDefinition> definitions = ConcreteDefinition.FromGrammar( grammar );
        Dictionary<RuleName, GuidingSymbolsSet> follows = grammar.GetFollowSet();
        
        foreach ( ConcreteDefinition definition in definitions )
        {
            RuleSymbol firstSymbol = definition.Definition.FirstSymbol();

            if ( firstSymbol.Type == RuleSymbolType.TerminalSymbol &&
                 firstSymbol.Symbol.Type == TerminalSymbolType.EmptySymbol )
            {
                foreach ( RuleSymbol followSymbol in follows[definition.RuleName].GuidingSymbols )
                {
                    table[followSymbol.Symbol!][definition.RuleName] = definition.Definition;
                }
                
                continue;
            }
            
            foreach ( TerminalSymbol headingSymbol in definition.HeadingSymbols )
            {
                if ( !table.ContainsKey( headingSymbol ) )
                {
                    continue;
                }

                table[headingSymbol][definition.RuleName] = definition.Definition;
            }
        }

        return table;
    }

    private HashSet<TerminalSymbol> GetAlphabet( CommonGrammar grammar )
    {
        var alphabet = new HashSet<TerminalSymbol>();
        alphabet.Add( TerminalSymbol.End() );
        
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

                    if ( symbol.Symbol.Type == TerminalSymbolType.EmptySymbol )
                    {
                        continue;
                    }

                    alphabet.Add( symbol.Symbol! );
                }
            }
        }

        return alphabet;
    }
}