using Commons;
using Microsoft.AspNetCore.SignalR.Client;

namespace CleanDisher;

public partial class RegisterForm : ContentPage
{
    HubConnection _connection;
    public RegisterForm(HubConnection connection)
    {
        InitializeComponent();
        _connection = connection;
    }

    private async void BtnIdDuplicateCheck_Clicked(object sender, EventArgs e)
    {
        (var result, var resultStr) = await _connection.InvokeAsync<Tuple<bool, string>>("SendMessageToServer", CommandType.CheckIdIsDuplicate, new string[] { InputId.Text });

        if (!result)
            await DisplayAlert("사용 가능", "해당하는 아이디는 사용 가능합니다.", "ok");
        else
            await DisplayAlert("사용 불가", resultStr, "ok");
    }

    private async void BtnPreviousView_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void BtnRegister_Clicked(object sender, EventArgs e)
    {
        var id = InputId.Text;
        var password = InputPassword.Text;
        var name = InputName.Text;
        var belong = InputBelong.Text;


        if (password != InputPasswordConfirm.Text)
        {
            await DisplayAlert("비밀번호 설정 오류", "해당하는 비밀번호가 일치하지 않습니다.", "ok");
            return;
        }

        (var result, var resultStr) = await _connection.InvokeAsync<Tuple<bool, string>>("SendMessageToServer", CommandType.ConfirmSignUp, new string[] { id, password, name, belong });

        if (!result)
        {
            await DisplayAlert("등록 오류", resultStr, "ok");
            return;
        }

        await DisplayAlert("등록 성공", "성공적으로 등록했습니다.", "ok");
        await Navigation.PopAsync();
    }

    //await DisplayAlert("등록 완료", "등록되었습니다.\n관리자의 승인을 기다려주세요.","ok");

}