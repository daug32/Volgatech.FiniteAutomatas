namespace FluentAssertions;

public static class FluentAsserts
{
    public static T ThrowIfNull<T>( this T? value ) => value == null
        ? throw new ArgumentNullException( nameof( value ), "Value must not be null" )
        : value;
    
    public static void ThrowIfNotNull<T>( this T? value )
    {
        if ( value is not null )
        {
            throw new ArgumentException( "Argument was not expected to be passed", nameof( value ) );
        }
    }
}