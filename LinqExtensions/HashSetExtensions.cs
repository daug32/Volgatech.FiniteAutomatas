namespace LinqExtensions;

public static class HashSetExtensions
{
    public static THashSet AddRange<THashSet, TItem>( this THashSet collection, IEnumerable<TItem> itemsToAdd )
        where THashSet : HashSet<TItem>
    {
        foreach ( TItem item in itemsToAdd )
        {
            collection.Add( item );
        }

        return collection;
    }
}