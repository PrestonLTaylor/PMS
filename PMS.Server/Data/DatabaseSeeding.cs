using Microsoft.AspNetCore.Identity;
using PMS.Server.Models;
using PMS.Server.Models.Fakers;

namespace PMS.Server.Data;

public static class DatabaseSeeding
{
	static public async Task AddDatabaseSeeding(this IServiceProvider provider)
	{
		using var scope = provider.CreateScope();

		var databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
		await databaseContext.Database.EnsureDeletedAsync();
		await databaseContext.Database.EnsureCreatedAsync();

		await SeedProductsIntoDatabase(databaseContext);
		await SeedDefaultUserIntoDatabase(scope);

		await databaseContext.SaveChangesAsync();
	}

	static private async Task SeedProductsIntoDatabase(DatabaseContext databaseContext)
	{
		var productFaker = new ProductFaker();
		var fakedProducts = productFaker.GenerateLazy(NUMBER_OF_FAKES_TO_CREATE);
		await databaseContext.Products.AddRangeAsync(fakedProducts);
	}

    static private async Task SeedDefaultUserIntoDatabase(IServiceScope scope)
    {
		const string defaultUsername = "user";
		const string defaultPassword = "pass";

		var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserModel>>();
		var user = new UserModel(defaultUsername);
		await userManager.CreateAsync(user, defaultPassword);
    }

    private const int NUMBER_OF_FAKES_TO_CREATE = 100;
}
