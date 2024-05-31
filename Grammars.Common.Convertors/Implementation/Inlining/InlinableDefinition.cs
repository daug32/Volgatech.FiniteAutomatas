using Grammars.Common.Grammars;
using Grammars.Common.Grammars.ValueObjects.RuleDefinitions;
using Grammars.Common.Grammars.ValueObjects.RuleNames;
using Grammars.Common.Grammars.ValueObjects.Symbols;
using LinqExtensions;

namespace Grammars.Common.Convertors.Implementation.Inlining;

internal class InlinableDefinition
{
    public readonly RuleName RuleName;
    
    public readonly bool IsStartRule;
    public bool StartsWithTerminal => Definitions.All( x => x.FirstSymbolType() == RuleSymbolType.TerminalSymbol );
    
    public List<RuleDefinition> Definitions;

    public InlinableDefinition( RuleName ruleName, bool isStartRule, IEnumerable<RuleDefinition> definitions )
    {
        RuleName = ruleName;
        IsStartRule = isStartRule;
        Definitions = definitions.ToList();
    }

    public bool CanInline( InlinableDefinition definitionToInline )
    {
        if ( !definitionToInline.StartsWithTerminal )
        {
            return false;
        }

        return Definitions.Any( definition =>
        {
            RuleSymbol firstSymbol = definition.Symbols.First();
            return firstSymbol.Type == RuleSymbolType.NonTerminalSymbol
                   && firstSymbol.RuleName == definitionToInline.RuleName;
        } );
    }

    public void Inline( InlinableDefinition definitionToInline )
    {
        var definitionsToProcess = new List<RuleDefinition>();
        for ( var index = 0; index < Definitions.Count; index++ )
        {
            RuleDefinition definition = Definitions[index];

            RuleSymbol firstSymbol = definition.Symbols.First();

            bool canHasInlining =
                firstSymbol.Type == RuleSymbolType.NonTerminalSymbol && firstSymbol.RuleName == definitionToInline.RuleName;

            if ( canHasInlining )
            {
                definitionsToProcess.Add( definition );
                Definitions.RemoveAt( index );
                index--;
            }
        }

        foreach ( RuleDefinition definitionToProcess in definitionsToProcess )
        {
            IEnumerable<RuleDefinition> newDefinitions = definitionToInline.Definitions
                .Select( definitionToInline =>
                {
                    var symbols = new List<RuleSymbol>( definitionToProcess.Symbols.Count + definitionToInline.Symbols.Count );
                    symbols.AddRange( definitionToInline.Symbols );
                    symbols.AddRange( definitionToProcess.Symbols.ToList().WithoutFirst() );

                    return new RuleDefinition( symbols );
                } );

            Definitions.AddRange( newDefinitions );
        }
    }

    public static List<InlinableDefinition> CreateFromGrammar( CommonGrammar grammar )
    {
        return grammar.Rules.Values
            .Select( rule => new InlinableDefinition( 
                rule.Name, 
                grammar.StartRule == rule.Name, 
                rule.Definitions ) )
            .ToList();
    }
}