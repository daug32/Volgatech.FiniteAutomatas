using Grammars.Common.Grammars.ValueObjects.RuleDefinitions;
using Grammars.Common.Grammars.ValueObjects.RuleNames;
using Grammars.Common.Grammars.ValueObjects.Symbols;

namespace Grammars.LL.Runners;

public class ParsingTable : Dictionary<TerminalSymbol, Dictionary<RuleName, RuleDefinition>>
{
    public readonly RuleName StartRule;
    public readonly HashSet<TerminalSymbol> Alphabet;

    public ParsingTable( RuleName startRule, HashSet<TerminalSymbol> alphabet )
    {
        Alphabet = alphabet;
        StartRule = startRule;
    }
}