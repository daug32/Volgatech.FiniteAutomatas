using Grammars.Common;
using Grammars.Common.ValueObjects;
using Grammars.LL.Models.Validations;

namespace Grammars.LL.Models;

public class LlOneGrammar : CommonGrammar
{
    public LlOneGrammar( RuleName startRule, IEnumerable<GrammarRule> rules )
        : base( startRule, rules )
    {
        ValidateOrThrow();
    }

    private void ValidateOrThrow()
    {
        var errors = new List<Exception>();

        errors.AddRange( new GrammarRulesAchievabilityValidator()
            .CheckAndGetFailed( StartRule, Rules )
            .Select( x => new ArgumentException( $"Rule is not achievable. RuleName: {x.Value}" ) ) );

        errors.AddRange( new GrammarRulesDeclarationCheck()
            .CheckAndGetFailed( StartRule, Rules )
            .Select( x => new ArgumentException( $"Rule was not declared. RuleName: {x.Value}" ) ) );

        if ( errors.Any() )
        {
            throw new AggregateException( errors );
        }
    }
}