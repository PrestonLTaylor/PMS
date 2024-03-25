using PMS.Client.ViewModels;

namespace PMS.Client.Views;

public partial class Login : ContentPage
{
	public Login(LoginViewModel loginViewModel)
	{
		InitializeComponent();
		BindingContext = loginViewModel;
	}
}