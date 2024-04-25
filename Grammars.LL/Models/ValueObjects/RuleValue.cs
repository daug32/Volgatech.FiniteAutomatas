namespace Grammars.Grammars.LeftRoRightOne.Models.ValueObjects;

public class RuleValue
{
    public readonly List<RuleValueItem> Items;

    public RuleValue( List<RuleValueItem> items )
    {
        Items = items;
    }

    public override bool Equals( object? obj ) => obj is RuleValue other && Equals( other );

    public override int GetHashCode()
    {
        return Items.GetHashCode();
    }

    public bool Equals( RuleValue other )
    {
        if ( other.Items.Count != Items.Count )
        {
            return false;
        }

        for ( var i = 0; i < Items.Count; i++ )
        {
            RuleValueItem item = Items[i];
            RuleValueItem otherItem = other.Items[i];

            if ( !item.Equals( otherItem ) )
            {
                return false;
            }
        }

        return true;
    }
    
    public static bool operator == ( RuleValue a, RuleValue b ) => a.Equals( b );

    public static bool operator !=( RuleValue a, RuleValue b ) => !a.Equals( b );

    public override string ToString() => String.Join( "", Items );
}