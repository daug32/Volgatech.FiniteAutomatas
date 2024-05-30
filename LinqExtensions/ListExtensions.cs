namespace LinqExtensions;

public static class ListExtensions
{
    public static List<T> With<T>( this List<T> collection, T item )
    {
        collection.Add( item );
        return collection;
    }

    public static List<T> WithMany<T>( this List<T> collection, IEnumerable<T> items )
    {
        collection.AddRange( items );
        return collection;
    }

    public static List<T> WithoutFirst<T>( this List<T> collection )
    {
        collection.RemoveAt( 0 );
        return collection;
    }
}