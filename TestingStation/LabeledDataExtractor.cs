namespace TestingStation;

public static class LabeledDataExtractor
{
    public static void Extract( string dirWithLabeledData, string outputFile )
    {
        if ( File.Exists( outputFile ) )
        {
            File.Delete( outputFile );
        }
        
        var streamWriter = new StreamWriter( outputFile );
        
        streamWriter.WriteLine( "Path\tLabel" );

        foreach ( string directory in Directory.GetDirectories( dirWithLabeledData ) )
        {
            string label = Path.GetFileName( directory ) ?? throw new Exception();
            foreach ( string file in Directory.GetFiles( directory ) )
            {
                streamWriter.WriteLine( $"{file}\t{label}" );   
            }
        }
        
        streamWriter.Close();
    }
}