using Commons;
using Microsoft.AspNetCore.SignalR.Client;

namespace CleanDisher;

public partial class LoginPage : ContentPage
{
    ServiceProvider _serviceProvider;
    public LoginPage(ServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new RegisterForm(_serviceProvider.Connection));
    }

    private async void LoginBtn_Clicked(object sender, EventArgs e)
    {
        string id = InputID.Text;
        string password = InputPassword.Text;

        
        Results result = null;
        if (_serviceProvider.Connection != null)
        {
            result = await _serviceProvider.Connection.InvokeAsync<Results>("SendMessageToServer", CommandType.RequestLogin, new string[] { id, password });
        }
        else
            await Dispatcher.DispatchAsync(async () =>
                await DisplayAlert("", "연결이 정상적으로 수행되지 않았습니다.\n어플리케이션을 다시 실행하세요", "ok"));

        if (result == null)
            await DisplayAlert("", "연결이 정상적으로 수행되지 않았습니다.\n어플리케이션을 다시 실행하세요", "ok");


        if (!result.HasError)
        {

            InputID.IsEnabled = false;
            InputID.IsEnabled = true;

            InputPassword.IsEnabled = false;
            InputPassword.IsEnabled = true;

            await Shell.Current.GoToAsync($"MainPage?distributeInfo={result.ResultStr}");
        }
        else
            await DisplayAlert("", result.ResultStr, "ok");
    }

    private void Input_Completed(object sender, EventArgs e)
    {
        (sender as View).IsEnabled = false;
        (sender as View).IsEnabled = true;
    }
}