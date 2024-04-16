using Microsoft.ML;

namespace TestingStation;

public class DataSettings
{
    public const string ModelFile = @"SymbolsPredictor.zip";
  
    public const string ImageFolder = @"D:\Development\Models\Symbols\DefaultSymbols"; 
    public const string DataPath = @"D:\Development\Models\Symbols\data.txt";
    public static readonly DataViewSchema Scheme = ImageData.Scheme;
    
    public const int Width = 45;
    public const int Height = 45;
}

public class Program
{
    public static void Main()
    {
        try
        {
            throw new Exception( "qwqaeqweqwe" );
        }
        catch ( Exception ex )
        {
            Console.WriteLine( $"Message: {ex.Message}" );
            Console.WriteLine( $"Source: {ex.Source}" );
        }
        
        // Console.WriteLine( "Creating a context" );
        // var context = new MLContext();
        //
        // Console.WriteLine( "Creating a model" );
        // ITransformer model = LoadOrCreateAndSaveModel( context );
        //
        // Console.WriteLine( "Creating a predictor" );
        // var predictor = context.Model.CreatePredictionEngine<ImageData, ImagePrediction>( model );
        //
        // Console.WriteLine( "Making a prediction" );
        // ImagePrediction? result = predictor.Predict( new ImageData( 
        //     @"D:\Development\Projects\TestingStation\TestingStation\ZSymbol.png", 
        //     "Z" ) );
        //
        // if ( result is null )
        // {
        //     Console.WriteLine( "Couldn't predict. Result is null" );
        //     return;
        // }
        //
        // foreach ( float label in result.PredictedLabels )
        // {
        //     Console.WriteLine( label );
        // }
    }

    private static ITransformer LoadOrCreateAndSaveModel( MLContext context )
    {
        return File.Exists( DataSettings.ModelFile )
            ? LoadModel( context )
            : CreateAndSaveNewModel( context );
    }

    private static ITransformer LoadModel( MLContext context )
    {
        Console.WriteLine( "Loading an existent model" );
        return context.Model.Load( DataSettings.ModelFile, out DataViewSchema _ );
    }

    private static ITransformer CreateAndSaveNewModel( MLContext context )
    {
        Console.WriteLine( "Creating a new model" );
        Console.WriteLine( "Loading data" );
        IDataView data = context.Data.LoadFromTextFile<ImageData>( DataSettings.DataPath ) ?? throw new FileNotFoundException();

        Console.WriteLine( "Fitting the model" );
        ITransformer model = FitData( context, data );
        
        Console.WriteLine( "Saving the model" );
        context.Model.Save( model, DataSettings.Scheme, DataSettings.ModelFile );

        return model;
    }

    private static ITransformer FitData( MLContext context, IDataView data )
    {
        var pipeline = context.Transforms
            .LoadImages(
                outputColumnName: "input",
                imageFolder: DataSettings.ImageFolder,
                inputColumnName: nameof( ImageData.Path ) )
            .Append(context.Transforms.ResizeImages(
                outputColumnName: "input",
                imageWidth: DataSettings.Width,
                imageHeight: DataSettings.Height,
                inputColumnName: "input"))
            .Append(context.Transforms.ExtractPixels(
                outputColumnName: "input"))
            .Append(context.Model
                .LoadTensorFlowModel(DataSettings.DataPath)
                .ScoreTensorFlowModel(
                    outputColumnNames: new[] { "output" },
                    inputColumnNames: new[] { "input" },
                    addBatchDimensionInput: true));

        return pipeline.Fit( data );
    }
}