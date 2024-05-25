namespace LinqExtensions;

// ReSharper disable once InconsistentNaming
public static class IEnumerableExtensions
{
    public static string ConvertToString( this char[] array ) => new( array );
    public static string ConvertToString( this IEnumerable<char> array ) => new( array.ToArray() );

    public static Queue<T> EnqueueRange<T>( this Queue<T> queue, IEnumerable<T> items )
    {
        foreach ( T item in items )
        {
            queue.Enqueue( item );
        }

        return queue;
    }
}

public static class LinkedListExtensions
{
    public static LinkedList<T> AddRangeToTail<T>( this LinkedList<T> collection, IEnumerable<T> items )
    {
        foreach ( T item in items )
        {
            collection.AddLast( item );
        }
        
        return collection;
    }

    public static T GetByIndex<T>( this LinkedList<T> collection, int index )
    {
        var current = collection.First;
        for ( int i = 0; i <= index; i++ )
        {
            current = current?.Next;
        }

        if ( current == null )
        {
            throw new IndexOutOfRangeException();
        }

        return current.Value;
    }

    public static T DequeueFirst<T>( this LinkedList<T> collection )
    {
        T a = collection.First();
        collection.RemoveFirst();
        return a;
    }
}