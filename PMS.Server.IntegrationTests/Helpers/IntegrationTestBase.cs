using Microsoft.AspNetCore.Mvc.Testing;

namespace PMS.Server.IntegrationTests.Helpers;

// Slightly edited from: https://github.com/dotnet/AspNetCore.Docs/blob/main/aspnetcore/grpc/test-services/sample/Tests/Server/IntegrationTests/IntegrationTestBase.cs
internal class IntegrationTestBase
{
    public WebApplicationFactory<Program> Factory { get; } = new();
}
