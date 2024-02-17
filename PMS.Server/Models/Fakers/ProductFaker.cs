using Bogus;

namespace PMS.Server.Models.Fakers;

internal sealed class ProductFaker : Faker<ProductModel>
{
	public ProductFaker()
	{
		StrictMode(true);

		RuleFor(p => p.Id, f => currentProductId++);

		RuleFor(p => p.Name, f => f.Commerce.Product());

		RuleFor(p => p.Price, f => f.Random.Number(MAX_PRODUCT_PRICE_IN_POUNDS));
	}

	private const int MAX_PRODUCT_PRICE_IN_POUNDS = 100;
	private int currentProductId = 0;
}
