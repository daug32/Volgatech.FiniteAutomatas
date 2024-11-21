using Grammars.Common.Convertors;
using Grammars.Common.Convertors.Convertors.Epsilons;
using Grammars.Common.Convertors.Convertors.Renaming;
using Grammars.Common.Convertors.Convertors.Whitespaces;
using Grammars.Common.Grammars;
using Grammars.Common.Grammars.ValueObjects.RuleDefinitions;
using Grammars.Common.Grammars.ValueObjects.Symbols;
using Grammars.SLR.Convertors.Implementation;
using Grammars.SLR.Models;
using Logging;

namespace Grammars.SLR.Convertors;

public class ToSlrOneGrammarConvertor : IGrammarConvertor<SlrOneGrammar>
{
    private readonly ILogger? _logger;

    public ToSlrOneGrammarConvertor( ILogger? logger = null )
    {
        _logger = logger;
    }

    public SlrOneGrammar Convert( CommonGrammar grammar )
    {
        CommonGrammar normalizedGrammar = grammar
            // Remove terminals that defines whitespace symbols (like regex "\s+")
           .Convert( new RemoveWhitespacesConvertor() )
            // There can be only one start rule
            .Convert( new RemoveReferencesToStartRuleConvertor() )
            // SLR requires no epsilon production in grammar
            .Convert( new RemoveEmptySymbolConvertor() )
            // Just to make it better to see
            .Convert( new RenameRuleNamesConvertor() );
        
        SanitizeEndSymbols( normalizedGrammar );

        return new SlrOneGrammar(
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