using Microsoft.ML;
using Microsoft.ML.Data;

namespace TestingStation;

public class DataViewSchemeBuilder
{
    private DataViewSchema.Builder _builder = new();
    
    public DataViewSchemeBuilder AddColumn( string name, DataViewType type )
    {
        _builder.AddColumn( name, type );
        return this;
    }

    public DataViewSchema ToScheme() => _builder.ToSchema();
}