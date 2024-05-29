using Grammars.Common.Grammars.ValueObjects;
using Grammars.Common.Grammars.ValueObjects.RuleDefinitions;
using Grammars.Common.Grammars.ValueObjects.Symbols;

namespace Grammars.LL.Runners;

public class ParsingTable : Dictionary<TerminalSymbol, Dictionary<RuleName, RuleDefinition>>
{
    public RuleName StartRule;

    public ParsingTable( RuleName startRule )
    {
        StartRule = startRule;
    }
}