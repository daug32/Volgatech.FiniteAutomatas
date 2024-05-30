using System.Collections.Immutable;
using Grammars.Common.Convertors;
using Grammars.Common.Convertors.Convertors;
using Grammars.Common.Grammars;
using Grammars.Common.Grammars.Extensions;
using Grammars.Common.Grammars.ValueObjects.GrammarRules;
using Grammars.Common.Grammars.ValueObjects.RuleDefinitions;
using Grammars.Common.Grammars.ValueObjects.Symbols;
using Grammars.LL.Models;
using LinqExtensions;
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
            .Convert( new LeftRecursionRemoverConvertor() )
            .Convert( new LeftFactorizationConvertor() )
            .Convert( new RemoveEpsilonsConvertor() )
            .Convert( new RenameRuleNamesConvertor() );

        SanitizeEndSymbols( normalizedGrammar );
        ValidateAmbiguousRuleDefinitionsOrThrow( normalizedGrammar );

        return new LlOneGrammar( 
            normalizedGrammar.StartRule,
            normalizedGrammar.Rules.Values );
    }

    private void SanitizeEndSymbols( CommonGrammar grammar )
    {
        bool hasEndSymbol = grammar.Rules.Values.Any( rule =>
            rule.Definitions.Any( definition =>
                definition.Symbols.Any( symbol =>
                    symbol.Type == RuleSymbolType.TerminalSymbol && symbol.Symbol.Type == TerminalSymbolType.End ) ) );

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

    private void ValidateAmbiguousRuleDefinitionsOrThrow( CommonGrammar grammar )
    {
        foreach ( GrammarRule rule in grammar.Rules.Values )
        {
            List<(RuleDefinition Definition, IImmutableSet<RuleSymbol> FirstSet)> headingSymbols = rule.Definitions
                .Select( definition => 
                    (
                        Definition: definition,
                        FirstSet: grammar.GetFirstSet( rule.Name, definition ).GuidingSymbols
                    ) )
                .ToList();

            List<(RuleDefinition Definition, IImmutableSet<RuleSymbol> FirstSet)> duplicateHeadingSets = FindDuplicateHeadingSets( headingSymbols );

            if ( duplicateHeadingSets.Any() )
            {
                string serializedDefinitions = String.Join(
                    "\n",
                    duplicateHeadingSets.Select( definition =>
                    {
                        string serializedDefinition = String.Join( " ", definition.Definition.Symbols );
                        string serializedSet = String.Join( ",", definition.FirstSet );
                        return $"<{rule.Name}> -> {serializedDefinition}\tFirstSet: {serializedSet}";
                    } ) );
                
                throw new ArgumentException(
                    $"Rule {rule.Name} has duplicate FIRST for different definitions\n{serializedDefinitions}" );
            }
        }
    }

    private static List<(RuleDefinition Definition, IImmutableSet<RuleSymbol> FirstSet)> FindDuplicateHeadingSets(
        List<(RuleDefinition Definition, IImmutableSet<RuleSymbol> FirstSet)> definitionsWithFirstSet )
    {
        var result = new List<(RuleDefinition Definition, IImmutableSet<RuleSymbol> FirstSet)>();

        for ( var mainIterator = 0; mainIterator < definitionsWithFirstSet.Count; mainIterator++ )
        {
            var mainDefinition = definitionsWithFirstSet[mainIterator];

            var duplicates = new List<(RuleDefinition Definition, IImmutableSet<RuleSymbol> FirstSet)>()
            {
                mainDefinition
            };
            
            for ( int secondIterator = mainIterator + 1; secondIterator < definitionsWithFirstSet.Count; secondIterator++ )
            {
                var secondDefinition = definitionsWithFirstSet[secondIterator];
                if ( secondDefinition.FirstSet.SetEquals( mainDefinition.FirstSet ) )
                {
                    duplicates.Add( secondDefinition );
                    definitionsWithFirstSet.RemoveAt( secondIterator );
                    secondIterator--;
                }
            }

            if ( duplicates.Count > 1 )
            {
                result.AddRange( duplicates );
                definitionsWithFirstSet.RemoveAt( mainIterator );
                mainIterator--;
            }
        }

        return result
            .OrderBy( x => String.Join( 
                "", 
                x.FirstSet
                    .OrderBy( i => i.ToString() )
                    .Select( i => i.ToString() ) ) )
            .ToList();
    }
}