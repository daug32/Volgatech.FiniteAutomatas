using Microsoft.ML;
using Microsoft.ML.Data;

namespace TestingStation;

public class ImageData
{
    public static readonly DataViewSchema Scheme = new DataViewSchemeBuilder()
        .AddColumn( nameof( Path ), TextDataViewType.Instance )
        .AddColumn( nameof( Label ), TextDataViewType.Instance )
        .ToScheme();
    
    [LoadColumn( 0 )]
    public string Path { get; }

    [LoadColumn( 1 )]
    public string Label { get; }

    public ImageData( string path, string label )
    {
        Path = path;
        Label = label;
    }
}