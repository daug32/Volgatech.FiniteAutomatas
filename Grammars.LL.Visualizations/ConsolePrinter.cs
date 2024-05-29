using ConsoleTables;
using Grammars.Common.Grammars.ValueObjects;
using Grammars.Common.Grammars.ValueObjects.RuleDefinitions;
using Grammars.Common.Grammars.ValueObjects.Symbols;
using Grammars.LL.Runners;

namespace Grammars.LL.Visualizations;

public static class ConsolePrinter
{
    public static ParsingTable ToConsole( this ParsingTable table )
    {
        string[] columns = BuildColumns( table ).ToArray();
        IEnumerable<string[]> rows = BuildRows( table );

        var consoleTable = new ConsoleTable( columns );
        foreach ( string[] row in rows )
        {
            consoleTable.AddRow( row );
        }

        consoleTable.Write();

        return table;
    }

    private static IEnumerable<string[]> BuildRows( ParsingTable table )
    {
        List<RuleName> rules = table.Values
            .SelectMany( x => x.Keys )
            .Distinct()
            .OrderBy( rule => rule == table.StartRule ? 0 : 1 )
            .ToList();
        
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
    }

    private static List<TerminalSymbol> OrderTerminalSymbols( IEnumerable<TerminalSymbol> terminalSymbols )
    {
        int maxPriority = Int32.MaxValue;
        return terminalSymbols.OrderBy( x =>
        {
            if ( x.Type == TerminalSymbolType.WhiteSpace )
            {
                return maxPriority - 2;
            }
            
            if ( x.Type == TerminalSymbolType.EmptySymbol )
            {
                return maxPriority - 1;
            }
                
            if ( x.Type == TerminalSymbolType.End )
            {
                return maxPriority;
            }

            var serializedText = x.ToString();
            if ( Char.IsLetter( serializedText.First() ) )
            {
                return 0;
            }

            return serializedText.First();
        } ).ToList();
    }
}