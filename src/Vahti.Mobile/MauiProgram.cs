

using Maui.FixesAndWorkarounds;
using Microsoft.Extensions.Configuration;
using SkiaSharp.Views.Maui.Controls.Hosting;
using System.Reflection;

namespace Vahti.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {        
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Vahti.Mobile.appsettings.json");
        var config = new ConfigurationBuilder()
                    .AddJsonStream(stream)
                    .Build();

        var builder = MauiApp.CreateBuilder();
        builder.ConfigureShellWorkarounds();
        builder.Configuration.AddConfiguration(config);
        builder
            .UseSkiaSharp(true)
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {                
                fonts.AddFont("FontAwesomeSolid.otf", "FontAwesomeSolid");
                fonts.AddFont("WeatherIconsRegular.ttf", "WeatherIconsRegular");
            });

        var app = builder.Build();
        Services = app.Services;

        return app;
    }

    public static IServiceProvider Services { get; private set; }
}
