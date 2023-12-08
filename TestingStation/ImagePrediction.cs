using Microsoft.ML.Data;

namespace TestingStation;

public class ImagePrediction
{
    [ColumnName( "softmax2" )]
    public float[] PredictedLabels { get; set; }
}