namespace LinqExtensions;

// ReSharper disable once InconsistentNaming
public static class IEnumerableExtensions
{
    public static string ConvertToString( this char[] array ) => new( array );
    public static string ConvertToString( this IEnumerable<char> array ) => new( array.ToArray() );

    public static void EnqueueRange<T>( this Queue<T> queue, IEnumerable<T> items )
    {
        foreach ( T item in items )
        {
            queue.Enqueue( item );
        }
    }

    public static List<T> With<T>( this List<T> collection, T item )
    {
        collection.Add( item );
        return collection;
    }

    public static List<T> With<T>( this List<T> collection, IEnumerable<T> items )
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