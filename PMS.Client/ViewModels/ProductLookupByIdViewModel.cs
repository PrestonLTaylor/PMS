using PMS.Lib.Data;
using PMS.Lib.Services;

namespace PMS.Client.ViewModels;

public sealed class ProductLookupByIdViewModel : BaseViewModel
{
    public ProductLookupByIdViewModel(IProductService productService)
    {
        _productService = productService;

        GetProductByIdCommand = new Command(async () => await GetProductByIdAsync());
    }

    public Command GetProductByIdCommand { get; }

    private async Task GetProductByIdAsync()
    {
        if (IsBusy || IdToLookup is null)
            return;

        IsBusy = true;

        var product = await _productService.GetProductByIdAsync(IdToLookup.Value);
        if (product is null)
        {
            await Shell.Current.DisplayAlert("Not Found", $"Unable to find product with an id of {IdToLookup}", "OK");
        }

        CurrentProduct = product;

        IsBusy = false;
    }

    public int? IdToLookup { get; set; } = null;
    private Product _currentProduct;
    public Product CurrentProduct
    {
        get => _currentProduct;
        internal set
        {
            // TODO: Override Equals for Product
            if (_currentProduct is not null && _currentProduct.Equals(value))
                return;

            _currentProduct = value;
            OnPropertyChanged();
        }
    }
    private readonly IProductService _productService;
}
