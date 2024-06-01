using Grammars.Common.Convertors.Convertors.Renaming.Implementation;
using Grammars.Common.Grammars;

namespace Grammars.Common.Convertors.Convertors.Renaming;

public class RenameRuleNamesConvertor : IGrammarConvertor
{
    private readonly RenameRuleNamesOptions _options;
    
    public RenameRuleNamesConvertor( RenameRuleNamesOptions? options = null )
    {
        _options = options ?? new RenameRuleNamesOptions();
    }
    
    public CommonGrammar Convert( CommonGrammar grammar )
    {
        return new RenameRuleNamesHandler( _options ).StandardizeNaming( grammar );
    }
}