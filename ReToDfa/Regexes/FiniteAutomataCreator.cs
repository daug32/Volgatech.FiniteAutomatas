using ReToDfa.FiniteAutomatas;
using ReToDfa.Regexes.Models;

namespace ReToDfa.Regexes;

public class FiniteAutomataCreator
{
    public FiniteAutomata CreateFromRegex( string regex )
    {
        RegexNode node = RegexNode.Parse( $"{regex}" );
        

        return null;
    }
}