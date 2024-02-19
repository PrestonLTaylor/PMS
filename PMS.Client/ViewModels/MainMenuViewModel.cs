using PMS.Client.Views;

namespace PMS.Client.ViewModels;

public sealed class MainMenuViewModel : BaseViewModel
{
	public MainMenuViewModel()
	{
		GoToProductLookupByIdCommand = new Command(async () => await GoToProductLookupByIdAsync());
        GoToProductLookupByNameCommand = new Command(async () => await GoToProductLookupByNameAsync());
	}

	public Command GoToProductLookupByIdCommand { get; }

	private async Task GoToProductLookupByIdAsync()
	{
		await Shell.Current.GoToAsync(nameof(ProductLookupById));
	}

	public Command GoToProductLookupByNameCommand { get; }

    private async Task GoToProductLookupByNameAsync()
    {
        await Shell.Current.GoToAsync(nameof(ProductLookupByName));
    }
}
