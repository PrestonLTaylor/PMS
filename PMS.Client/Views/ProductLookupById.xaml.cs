using PMS.Client.ViewModels;

namespace PMS.Client.Views;

public partial class ProductLookupById : ContentPage
{
	public ProductLookupById(ProductLookupByIdViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}