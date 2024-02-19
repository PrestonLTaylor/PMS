using PMS.Client.ViewModels;

namespace PMS.Client.Views;

public partial class ProductLookupByName : ContentPage
{
	public ProductLookupByName(ProductLookupByNameViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}