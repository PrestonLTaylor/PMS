using PMS.Client.Views;
using PMS.Lib.Services;

namespace PMS.Client.ViewModels;

public sealed class LoginViewModel : BaseViewModel
{
    public LoginViewModel(ILoginService loginService)
    {
        _loginService = loginService;

        LoginCommand = new Command(async () => await LoginAsync());
    }

    public Command LoginCommand { get; }

    private async Task LoginAsync()
    {
        if (IsBusy || Username is null || Password is null)
            return;

        IsBusy = true;

        var response = await _loginService.LoginAsync(Username, Password);
        response.Switch(
            async _ => await HandleAuthenticated(),
            async _ => await HandleUnauthenticated(),
            async grpcError => await HandleGrpcError(grpcError)
        );

        IsBusy = false;
    }

    private async Task HandleAuthenticated()
    {
		await Shell.Current.GoToAsync(nameof(MainMenu));
    }

    private async Task HandleUnauthenticated()
    {
        await Shell.Current.DisplayAlert("Login Failed", $"Unable to login, invalid username or password.", "OK");
    }

    public string Username { get; set; } = null;
    public string Password { get; set; } = null;

    private readonly ILoginService _loginService;
}
