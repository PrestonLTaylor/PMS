using PMS.Client.ViewModels;

namespace PMS.Client.Views;

public sealed partial class MainMenu : ContentPage
{
    public MainMenu(MainMenuViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
