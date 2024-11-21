﻿using Grammars.Common.Convertors;
using Grammars.Common.Convertors.Convertors.Factorization;
using Grammars.Common.Convertors.Convertors.LeftRecursions;
using Grammars.Common.Convertors.Convertors.Renaming;
using Grammars.Common.Convertors.Convertors.Whitespaces;
using Grammars.Common.Grammars;
using Grammars.Common.Grammars.ValueObjects.RuleDefinitions;
using Grammars.Common.Grammars.ValueObjects.Symbols;
using Grammars.LL.Models;
using Logging;

namespace Grammars.LL.Convertors;

public class ToLlOneGrammarConvertor : IGrammarConvertor<LlOneGrammar>
{
    private readonly ILogger? _logger;

    public ToLlOneGrammarConvertor( ILogger? logger = null )
    {
        _logger = logger;
    }

    public LlOneGrammar Convert( CommonGrammar grammar )
    {
        CommonGrammar normalizedGrammar = grammar
            // Remove terminals that defines whitespace symbols (like regex "\s+")
           .Convert( new RemoveWhitespacesConvertor() )
            // LL one grammar requires no left recursion rules 
            .Convert( new LeftRecursionRemoverConvertor() )
            // LL one grammar requires no ambiguous references
            .Convert( new LeftFactorizationConvertor() )
            // Just to make it better to see
            .Convert( new RenameRuleNamesConvertor() );

        SanitizeEndSymbols( normalizedGrammar );

        return new LlOneGrammar( 
            normalizedGrammar.StartRule,
            normalizedGrammar.Rules.Values );
    }

    private void SanitizeEndSymbols( CommonGrammar grammar )
    {
        bool hasEndSymbol = grammar.Rules.Values.Any( rule =>
            rule.Definitions.Any( definition =>
                definition.Symbols.Any( symbol =>
                    symbol.Type == RuleSymbolType.TerminalSymbol && symbol.Symbol!.Type == TerminalSymbolType.End ) ) );

        if ( hasEndSymbol )
        {
            return;
        }

        _logger?.Write(
            LogLevel.Warning,
            "End symbol was not found. It will be placed automatically in the end of all definitions of the start rule" );

        grammar.Rules[grammar.StartRule].Definitions = grammar.Rules[grammar.StartRule]
            .Definitions
            .Select( definition => new RuleDefinition( definition.Symbols.Append( RuleSymbol.TerminalSymbol( TerminalSymbol.End() ) ) ) )
            .ToList();
    }
}