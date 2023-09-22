using Microsoft.AspNetCore.SignalR.Client;

namespace CleanDisher;

public partial class App : Application
{
	public App(AppShell appShell)
    {
		InitializeComponent();
        MainPage = appShell;

    
	}

}
