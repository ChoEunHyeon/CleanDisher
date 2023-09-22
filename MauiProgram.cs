using Microsoft.AspNetCore.SignalR.Client;

namespace CleanDisher;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();


		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});


        builder.Services.AddSingleton<LoginPage>();
        builder.Services.AddSingleton<MainPage>();

        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddSingleton<ServiceProvider>();


        return builder.Build();
	}
}
