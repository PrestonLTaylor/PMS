using PMS.Client.Views;

namespace PMS.Client.ViewModels;

public sealed class MainMenuViewModel : BaseViewModel
{
	public MainMenuViewModel()
	{
		GoToProductLookupByIdCommand = new Command(async () => await GoToProductLookupByIdAsync());
	}

	public Command GoToProductLookupByIdCommand { get; }

	async Task GoToProductLookupByIdAsync()
	{
		await Shell.Current.GoToAsync(nameof(ProductLookupById));
	}
}
