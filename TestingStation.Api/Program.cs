using Microsoft.AspNetCore;

namespace TestingStation.Api;

public class Program
{
    public static Task Main( string[] args )
    {
        IWebHost host = BuildHost( args );
        return host.StartAsync();
    }

    private static IWebHost BuildHost( string[] args ) => WebHost
        .CreateDefaultBuilder( args )
        .UseStartup<Startup>()
        .Build();
}

internal class Startup : IStartup
{
    private readonly IConfiguration _configuration;

    public Startup( IConfiguration configuration )
    {
        _configuration = configuration;
    }

    public IServiceProvider ConfigureServices( IServiceCollection services )
    {
#pragma warning disable ASP0000
        return ApplyServices( services ).BuildServiceProvider();
#pragma warning restore ASP0000
    }

    public IServiceCollection ApplyServices( IServiceCollection services )
    {
        services.AddSignalR();
        services.AddSwaggerGen();
        services.ConfigureSwaggerGen( swaggerGenOptions =>
        {
        } );
        
        return services;
    }

    public void Configure( IApplicationBuilder app )
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
}