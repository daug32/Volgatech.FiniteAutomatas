using Grammars.Common;
using Grammars.Common.Convertors;
using Grammars.Common.Convertors.Epsilons;
using Grammars.Common.Convertors.LeftRecursions;
using Grammars.Common.Convertors.Semantics;
using Grammars.Common.ValueObjects;
using Grammars.Common.ValueObjects.Symbols;
using Grammars.Console.Parsers;
using Grammars.Visualization;

namespace Grammars.Console;

public class Program
{
    private static readonly GrammarParser _grammarParser = new();
    
    public static void Main()
    {
        CommonGrammar grammar = _grammarParser
            .ParseFile( @"../../../Grammars/common.txt" )
            .Convert( new WhitespacesRemoveConvertor() )
            .ToConsole( "Parse" )
            
            .Convert( new RemoveEpsilonsConvertor() )
            .ToConsole( "Epsilon eliminations" )

            .Convert( new LeftRecursionRemoverConvertor() )
            .ToConsole( "Left recursion" )

            .Convert( new RemoveEpsilonsConvertor() )
            .Convert( new RenameRuleNamesConvertor() )
            .ToConsole( "Epsilon eliminations" );
    }
}

public class WhitespacesRemoveConvertor : IGrammarConvertor
{
    public CommonGrammar Convert( CommonGrammar grammar )
    {
        return new CommonGrammar(
            grammar.StartRule,
            grammar.Rules.Values.Select( rule => new GrammarRule(
                rule.Name,
                rule.Definitions.Select( definition => new RuleDefinition( definition.Symbols.Where( symbol =>
                    symbol.Type == RuleSymbolType.NonTerminalSymbol || 
                    symbol.Type == RuleSymbolType.TerminalSymbol && 
                    symbol.Symbol.Type != TerminalSymbolType.WhiteSpace ) ) ) ) ) );
    }
}