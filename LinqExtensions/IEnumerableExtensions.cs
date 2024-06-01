namespace LinqExtensions;

// ReSharper disable once InconsistentNaming
public static class IEnumerableExtensions
{
    public static string ConvertToString( this IEnumerable<char> array ) => new( array.ToArray() );

    public static Queue<T> EnqueueRange<T>( this Queue<T> queue, IEnumerable<T> items )
    {
        foreach ( T item in items )
        {
            queue.Enqueue( item );
        }

        return queue;
    }

    public static List<T> ToListExcept<T>( this IEnumerable<T> items, int indexToExclude )
    {
        var result = new List<T>();

        int index = 0;
        foreach ( T item in items )
        {
            if ( index == indexToExclude )
            {
                index++;
                continue;
            }
            
            result.Add( item );
            index++;
        }

        return result;
    }
}