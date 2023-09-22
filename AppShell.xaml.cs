namespace CleanDisher;

public partial class AppShell : Shell
{
	public AppShell(LoginPage loginPage)
    {
		InitializeComponent();

        Routing.RegisterRoute("MainPage", typeof(MainPage));

        this.CurrentItem = loginPage;
    }
}
