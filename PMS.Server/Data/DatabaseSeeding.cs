using PMS.Server.Models.Fakers;

namespace PMS.Server.Data;

public static class DatabaseSeeding
{
	static public async Task AddDatabaseSeeding(this IServiceProvider provider)
	{
		using var scope = provider.CreateScope();

		var databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
		await databaseContext.Database.EnsureCreatedAsync();

		if (databaseContext.Products.Any())
		{
			return;
		}

		await SeedProductsIntoDatabase(databaseContext);
		await databaseContext.SaveChangesAsync();
	}

	static private async Task SeedProductsIntoDatabase(DatabaseContext databaseContext)
	{
		var productFaker = new ProductFaker();
		var fakedProducts = productFaker.GenerateForever().Take(NUMBER_OF_FAKES_TO_CREATE);
		await databaseContext.Products.AddRangeAsync(fakedProducts);
	}

	private const int NUMBER_OF_FAKES_TO_CREATE = 100;
}
