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
}