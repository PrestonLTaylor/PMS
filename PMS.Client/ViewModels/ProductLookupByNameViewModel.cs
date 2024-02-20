using PMS.Lib.Data;
using PMS.Lib.Services;
using System.Collections.ObjectModel;

namespace PMS.Client.ViewModels;

public sealed class ProductLookupByNameViewModel : BaseViewModel
{
    public ProductLookupByNameViewModel(IProductService productService)
    {
        _productService = productService;

        GetProductsByNameCommand = new Command(async () => await GetProductsByNameAsync());
    }

    public Command GetProductsByNameCommand { get; }

    private async Task GetProductsByNameAsync()
    {
        if (IsBusy || PartialProductName is null)
            return;

        IsBusy = true;

        var foundProducts = await _productService.GetProductsByPartialNameAsync(PartialProductName);
        if (foundProducts.Count == 0)
        {
            await Shell.Current.DisplayAlert("Not Found", $"No products were found with the name \"{PartialProductName}\"", "OK");
        }

        Products.Clear();
        foreach (var foundProduct in foundProducts)
        {
            Products.Add(foundProduct);
        }

        IsBusy = false;
    }

    public string PartialProductName { get; set; } = null;
    public ObservableCollection<Product> Products { get; } = [];
    private readonly IProductService _productService;
}
