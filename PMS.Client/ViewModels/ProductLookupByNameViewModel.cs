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

        Products.Clear();

        var response = await _productService.GetProductsByPartialNameAsync(PartialProductName);
        response.Switch(
           HandleFoundProducts,
           async _ => await HandleNotFound(),
           async error => await HandleGrpcError(error)
        );

        IsBusy = false;
    }

    private void HandleFoundProducts(IReadOnlyList<Product> products)
    {
        foreach (var product in products)
        {
            Products.Add(product);
        }
    }

    private async Task HandleNotFound()
    {
        await Shell.Current.DisplayAlert("Not Found", $"No products were found with the name \"{PartialProductName}\"", "OK");
    }

    public string PartialProductName { get; set; } = null;
    public ObservableCollection<Product> Products { get; } = [];
    private readonly IProductService _productService;
}
