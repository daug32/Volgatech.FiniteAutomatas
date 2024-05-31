using ConsoleTables;
using Grammars.Common.Grammars.Extensions;
using Grammars.Common.Grammars.ValueObjects;
using Grammars.Common.Grammars.ValueObjects.GrammarRules;
using Grammars.Common.Grammars.ValueObjects.RuleDefinitions;
using Grammars.Common.Grammars.ValueObjects.RuleNames;
using Grammars.Common.Grammars.ValueObjects.Symbols;
using Grammars.LL.Models;
using Grammars.LL.Runners;

namespace Grammars.LL.Visualizations;

public static class ConsolePrinter
{
    public static ParsingTable ToConsole( this ParsingTable table, LlOneGrammar grammar )
    {
        string[] columns = BuildColumns( table ).ToArray();
        IEnumerable<string[]> rows = BuildRows( table, grammar );

        var consoleTable = new ConsoleTable( new ConsoleTableOptions()
        {
            Columns = columns,
            EnableCount = false
        } );
        foreach ( string[] row in rows )
        {
            consoleTable.AddRow( row );
        }

        consoleTable.Write();

        return table;
    }

    private static IEnumerable<string[]> BuildRows( ParsingTable table, LlOneGrammar grammar )
    {
        List<RuleName> rules = OrderRules( table );
        
        List<TerminalSymbol> orderTerminalSymbols = OrderTerminalSymbols( table.Keys );

        foreach ( RuleName rule in rules )
        {
            var values = new List<string>
            {
                rule.ToString()
            };

            foreach ( TerminalSymbol terminalSymbol in orderTerminalSymbols )
            {
                Dictionary<RuleName, RuleDefinition> ruleToDefinition = table[terminalSymbol];
                if ( !ruleToDefinition.ContainsKey( rule ) )
                {
                    values.Add( String.Empty );
                    continue;
                }
                
                values.Add( String.Join( " ", ruleToDefinition[rule].Symbols ) );
            }
            
            values.Add( String.Join( ",", grammar.GetFirstSet( rule ).GuidingSymbols ) );
            
            values.Add( String.Join( ",", grammar.GetFollowSet( rule ).GuidingSymbols ) );

            yield return values.ToArray();
        }
    }

    private static IEnumerable<string> BuildColumns( ParsingTable table )
    {
        yield return "S/T";
        
        foreach ( TerminalSymbol x in OrderTerminalSymbols( table.Keys ) )
        {
            yield return x.ToString();
        }

        yield return "FIRST";

        yield return "FOLLOW";
    }

    private static List<RuleName> OrderRules( ParsingTable table )
    {
        return table.Values
            .SelectMany( x => x.Keys )
            .Distinct()
            .OrderBy( rule =>
            {
                if ( rule == table.StartRule )
                {
                    return 0;
                }

                return 1;
            } )
            .ThenBy( rule => rule.ToString() )
            .ToList();
    }

    private static List<TerminalSymbol> OrderTerminalSymbols( IEnumerable<TerminalSymbol> terminalSymbols )
    {
        return terminalSymbols
            .OrderBy( x =>
            {
                if ( x.Type == TerminalSymbolType.WhiteSpace )
                {
                    return 3;
                }
                
                if ( x.Type == TerminalSymbolType.EmptySymbol )
                {
                    return 2;
                }
                    
                if ( x.Type == TerminalSymbolType.End )
                {
                    return 1;
                }

                return 0;
            } )
            .ThenBy( x => x.ToString() )
            .ToList();
    }
}