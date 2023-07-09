

using Maui.FixesAndWorkarounds;
using Microsoft.Extensions.Configuration;
using SkiaSharp.Views.Maui.Controls.Hosting;
using System.Reflection;
using Vahti.Mobile.Forms.Effects;

namespace Vahti.Mobile.Forms;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {        
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Vahti.Mobile.Forms.appsettings.json");
        var config = new ConfigurationBuilder()
                    .AddJsonStream(stream)
                    .Build();

        var builder = MauiApp.CreateBuilder();
        builder.ConfigureShellWorkarounds();
        builder.Configuration.AddConfiguration(config);
        builder
            .UseSkiaSharp(true)
            .UseMauiApp<App>()
            .ConfigureEffects(effects => { effects.Add<LineColorEffect, EditTextLineColorEffect>(); });

        var app = builder.Build();
        Services = app.Services;

        return app;
    }

    public static IServiceProvider Services { get; private set; }
}
