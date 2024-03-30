using PMS.Client.ViewModels;
using PMS.Lib.Services;

namespace PMS.Client.Views;

public partial class Login : ContentPage
{
	public Login(LoginViewModel loginViewModel, TokenService tokenService)
	{
		InitializeComponent();
		BindingContext = loginViewModel;
		_tokenService = tokenService;
	}

    protected override void OnNavigatedTo(NavigatedToEventArgs _)
    {
		_tokenService.ResetToken();
    }

	private readonly TokenService _tokenService;
}