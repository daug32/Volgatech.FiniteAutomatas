using Grammars.Common.Grammars.ValueObjects.RuleNames;

namespace Grammar.Parsers;

public class ParsingSettings
{
    public char ShieldingSymbol { get; init; } = '/';
    public char EndSymbol { get; init; } = '$';
    public char EmptySymbol { get; init; } = 'ε';
    public char RuleNameOpenSymbol { get; init; } = RuleName.RuleNameOpenSymbol;
    public char RuleNameCloseSymbol { get; init; } = RuleName.RuleNameCloseSymbol;
    
    public char CommentIdentifier { get; init; } = '#';
    public string RuleNameSeparator { get; init; } = "->";
    public char RuleDefinitionsSeparator { get; init; } = '|';
}