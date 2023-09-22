using Commons;
using Microsoft.AspNetCore.SignalR.Client;
using System.Data.Common;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Text.Json;
using System.Xml.Linq;

namespace CleanDisher;

[QueryProperty(nameof(DistributeInfo), "distributeInfo")]
public partial class MainPage : ContentPage
{
    private ServiceProvider _serviceProvider;
    private State dataGetted = new();

    public string DistributeInfo
    {
        set
        {
            LoadDistributeInfo(value);
        }
    }

    private string distributeInfo;

    private void LoadDistributeInfo(string value)
    {
        distributeInfo = value;
    }

    public MainPage(ServiceProvider serviceProvider)
    {
        InitializeComponent();

        _serviceProvider = serviceProvider;
        var connection = _serviceProvider.Connection;

        connection.On("DataReceived", (Action<State>)((state) =>
        {
            if (state == null)
                return;

            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (!state.IsActionOn)
                    CheckCurrentBtnState(dataGetted.CanActionNow, state.CanActionNow, BtnAction, "Action\nREADY", "Action\nOFF", "015CAD");
                else
                    CheckCurrentBtnState(dataGetted.IsActionOn, state.IsActionOn, BtnAction, "Action\nON", "Action\nOFF");

                CheckCurrentBtnState(dataGetted.IsReadyOn, state.IsReadyOn, BtnReady, "Ready\nON", "Ready\nOFF");
                CheckCurrentBtnState(dataGetted.IsCleaningOn, state.IsCleaningOn, BtnCleaning, "Cleaning\nON", "Cleaning\nOFF");


                LbTemperatureInside.Text = state.TemperatureAir.ToString("F1");
                LbHumidity.Text = state.Humidity.ToString("F1");

                switch (state.ParticulateMatter)
                {
                    case >= 151:
                        LbDust.Text = "매우 나쁨";
                        LbDust.TextColor = Colors.Red;
                        break;
                    case >= 81:
                        LbDust.Text = "매우 나쁨";
                        LbDust.TextColor = Colors.Orange;
                        break;
                    case >= 31:
                        LbDust.Text = "보통";
                        LbDust.TextColor = Colors.Green;
                        break;
                    default:
                        LbDust.Text = "좋음";
                        LbDust.TextColor = Colors.Blue;
                        break;
                }

                LbWaterUsageToday.Text = state.WaterUsageToday.ToString();
                LbWaterUsageMonth.Text = state.WaterUsageMonth.ToString();
                LbWaterUsageAmount.Text = state.WaterUsageAmount.ToString();

                LbTemperatureWasingTank.Text = $"{state.TemperatureWashingTank / 10}도";
                LbTemperatureRinseTank.Text = $"{state.TemperatureRinseTank / 10}도";
                LbTemperatureDryTray.Text = $"{state.TemperatureDryTray / 10}도";
                LbTemperatureDryDishes.Text = $"{state.TemperatureDryDishes / 10}도";

                dataGetted = state;

            });


        }));

        _serviceProvider.ReconnectAction = async () =>
        {
            await connection.InvokeAsync("SendMessageToServer", CommandType.Reconnected, new string[] { distributeInfo });

        };
        //connection.Closed += Connection_Closed;

        //connection.Reconnecting += error =>
        //{
        //    Debug.Assert(connection.State == HubConnectionState.Reconnecting);
        //    DisplayAlert("error", "connection lost", "yes");

        //    return Task.CompletedTask;
        //};

        //connection.Reconnected += (connectionId) =>
        //{
        //    DisplayAlert("error", "connection ReConnected", "yes");

        //    return null;
        //};

        static void CheckCurrentBtnState(bool originalValue, bool newValue, Button btnToCheck, string onText, string offText, string onColor = "80A9F6")
        {
            if (newValue)
            {
                if (!originalValue)
                {
                    btnToCheck.Background = new SolidColorBrush(Color.FromArgb(onColor));
                    btnToCheck.Text = onText;
                }
            }
            else
            {
                if (!originalValue)
                {
                    btnToCheck.Background = new SolidColorBrush(Colors.Gray);
                    btnToCheck.Text = offText;
                }
            }
        }
    }

    //public MainPage()
    //{
    //    //.WithUrl("http://192.168.219.101:5001/chat")

    //    InitializeComponent();
    //    _connection = new HubConnectionBuilder()
    //        .WithUrl("http://112.144.208.123:5001/chat")
    //        .WithAutomaticReconnect()
    //        .Build();


    //    _connection.On("DataReceived", (Action<DataToGet>)((message) =>
    //    {
    //        if (message == null)
    //            return;

    //        MainThread.BeginInvokeOnMainThread(() =>
    //        {
    //            CheckCurrentBtnState(dataGetted.IsReadyOn, message.IsReadyOn, BtnReady, "Ready\nON", "Ready\nOFF");
    //            CheckCurrentBtnState(dataGetted.IsActionOn, message.IsActionOn, BtnAction, "Action\nON", "Action\nOFF");
    //            CheckCurrentBtnState(dataGetted.IsCleaningOn, message.IsCleaningOn, BtnCleaning, "Cleaning\nON", "Cleaning\nOFF");

    //            LbWaterUsageToday.Text = message.WaterUsageToday.ToString();
    //            LbWaterUsageMonth.Text = message.WaterUsageMonth.ToString();
    //            LbWaterUsageAmount.Text = message.WaterUsageAmount.ToString();

    //            Debug.WriteLine("t");
    //            //LbStartTime.Text = $"{message[0]}";
    //            //LbElapsedTime.Text = $"{message[1]}";
    //            //LbInSideTemperature.Text = $"{message[2]}도";
    //            //LbInSideHumidity.Text = $"{message[3]}도";
    //            //LbWaterTemperature.Text = $"{message[4]}도";
    //            //LbWaterLevel.Text = $"{message[5]}";
    //            dataGetted = message;

    //        });


    //    }));
    //    _connection.Closed += Connection_Closed;

    //    _connection.Reconnecting += error =>
    //    {
    //        Debug.Assert(_connection.State == HubConnectionState.Reconnecting);
    //        DisplayAlert("error", "connection lost", "yes");

    //        return Task.CompletedTask;
    //    };

    //    _connection.Reconnected += (connectionId) =>
    //    {
    //        DisplayAlert("error", "connection ReConnected", "yes");

    //        return null;
    //    };

    //    static void CheckCurrentBtnState(bool originalValue, bool newValue, Button btnToCheck, string onText, string offText)
    //    {
    //        if (newValue)
    //        {
    //            if (!originalValue)
    //            {
    //                btnToCheck.BackgroundColor = Color.FromArgb("80A9F6");
    //                btnToCheck.Text = onText;
    //            }
    //        }
    //        else
    //        {
    //            if (!originalValue)
    //            {
    //                btnToCheck.BackgroundColor = Colors.Gray;
    //                btnToCheck.Text = offText;
    //            }
    //        }
    //    }
    //}

    private Task Connection_Closed(Exception arg)
    {
        return null;
    }

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        //Task.Run(async () =>
        //{

        //    while (true)
        //    {
        //        try
        //        {
        //            await Task.Delay(5000);

        //            //if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        //            //{
        //            //    await DisplayAlert("전송 실패", "데이터 통신이 불가능합니다.\n" +
        //            //        "통신환경을 확인하세요(Wifi 혹은 Lte)", "예");
        //            //    return;
        //            //}

        //            var connection = _serviceProvider.Connection;
        //            if (connection.State == HubConnectionState.Disconnected)
        //                await connection.StartAsync();

        //            (bool _, string dataJson) = await connection.InvokeAsync<Tuple<bool, string>>("SendMessageToServer", CommandType.GetData, new string[] { distributeInfo });
        //            //var items = JsonSerializer.Deserialize<State>(dataJson, new JsonSerializerOptions() { WriteIndented = true });

        //            //if (items == null)
        //            //    continue;

        //            //Dispatcher.Dispatch(() =>
        //            //{
        //            //    LbTemperatureWasingTank.Text = $"{items.TemperatureWashingTank}도";
        //            //    LbTemperatureUltraSonic.Text = $"{items.TemperatureUltraSonic}도";
        //            //    LbTemperatureRinseTank.Text = $"{items.TemperatureRinseTank}도";
        //            //    LbTemperatureDryTank.Text = $"{items.TemperatureDryTank}도";
        //            //});

        //            //LbWaterUsageToday.Text = message.WaterUsageToday.ToString();
        //            //LbWaterUsageMonth.Text = message.WaterUsageMonth.ToString();
        //            //LbWaterUsageAmount.Text = message.WaterUsageAmount.ToString();

        //            //CheckCurrentBtnState(dataGetted.IsReadyOn, message.IsReadyOn, BtnReady, "Ready\nON", "Ready\nOFF");
        //            //CheckCurrentBtnState(dataGetted.IsActionOn, message.IsActionOn, BtnAction, "Action\nON", "Action\nOFF");
        //            //CheckCurrentBtnState(dataGetted.IsCleaningOn, message.IsCleaningOn, BtnCleaning, "Cleaning\nON", "Cleaning\nOFF");
        //        }
        //        catch (Exception ex)
        //        {

        //        }
        //    }
        //});

    }

    private async void BtnReady_Clicked(object sender, EventArgs e)
    {
        try
        {
            // (bool _, string dataJson) = await connection.InvokeAsync<Tuple<bool, string>>("SendMessageToServer", CommandType.GetData, new string[] { distributeInfo });
            if (!await CheckCanCommunicate())
                return;

            HubConnection connection = await GetConnection();

            int value = 0;
            if (!dataGetted.IsReadyOn)
                value = 1;

            await connection.InvokeAsync("SendMessageToServer", CommandType.SendDataToEndpoints, new string[] { distributeInfo, "word", "110", "0", value.ToString() });

            //(bool _, string dataJson) = await connection.InvokeAsync<Tuple<bool, string>>("SendMessageToServer", CommandType.GetData, new string[] { distributeInfo });


            //var items = JsonSerializer.Deserialize<State>(dataJson, new JsonSerializerOptions() { WriteIndented = true });


            //DisplayAlert("data",data,"yes");
            //await connection.InvokeCoreAsync("SendMessageToServer", args: new[] { new { dataType = "bit", ioNumber = 10, data = 100, bitNumber = 30 } });
        }
        catch (Exception ex)
        {
            await DisplayAlert("전송 실패", "전송에 실패했습니다.", "예");
        }
    }

    private async Task<HubConnection> GetConnection()
    {
        var connection = _serviceProvider.Connection;
        if (connection.State == HubConnectionState.Disconnected)
            await connection.StartAsync();
        return connection;
    }

    protected override bool OnBackButtonPressed()
    {
        Dispatcher.Dispatch(async () =>
        {
            var leave = await DisplayAlert("앱 종료", "어플리케이션을 종료하시겠습니까?", "예", "아니오");

            if (leave)
            {
                Application.Current.Quit();

            }
        });

        return true;
    }

    private async Task<bool> CheckCanCommunicate()
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            await DisplayAlert("전송 실패", "데이터 통신이 불가능합니다.\n" +
                "통신환경을 확인하세요(Wifi 혹은 Lte)", "예");
            return false;
        }

        return true;
    }

    private async void BtnAction_Clicked(object sender, EventArgs e)
    {
        int value = 0;
        if (!dataGetted.CanActionNow)
            await DisplayAlert("전송 실패", "작업이 준비되지 않았습니다.", "예");
        else
            value = 1;

        if (!await CheckCanCommunicate())
            return;

        HubConnection connection = await GetConnection();
        await connection.InvokeAsync("SendMessageToServer", CommandType.SendDataToEndpoints, new string[] { distributeInfo, "word", "112", "0", value.ToString() });
    }

    private async void BtnCleaning_Clicked(object sender, EventArgs e)
    {
        if (!await CheckCanCommunicate())
            return;

        HubConnection connection = await GetConnection();
        await connection.InvokeAsync("SendMessageToServer", CommandType.SendDataToEndpoints, new string[] { distributeInfo, "word", "111", "0", "1" });

    }
}

