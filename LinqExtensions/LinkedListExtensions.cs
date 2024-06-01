namespace LinqExtensions;

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

    public static T DequeueFirst<T>( this LinkedList<T> collection )
    {
        T a = collection.First();
        collection.RemoveFirst();
        return a;
    }
}