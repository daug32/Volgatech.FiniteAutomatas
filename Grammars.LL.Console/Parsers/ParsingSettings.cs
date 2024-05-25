using Grammars.LL.Models.ValueObjects;

namespace Grammars.LL.Console.Parsers;

public static class ParsingSettings
{
    public const char ShieldingSymbol = '/';
    public const char EndSymbol = '$';
    public const char EmptySymbol = 'ε';
    public const char RuleNameOpenSymbol = RuleName.RuleNameOpenSymbol;
    public const char RuleNameCloseSymbol = RuleName.RuleNameCloseSymbol;
    
    public const char CommentIdentifier = '#';
    public const string RuleNameSeparator = "->";
    public const char RuleValueSeparator = '|';
}