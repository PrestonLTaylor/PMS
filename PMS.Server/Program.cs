using PMS.Server.Data;
using PMS.Server.Data.Repositories;
using PMS.Server.Installers;
using PMS.Server.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DatabaseContext>();
builder.Services.AddPMSUserIdentity();

builder.Services.AddJwtBasedAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

builder.Services.AddGrpc();
builder.Services.AddRepositories();

builder.Host.UseSerilog((hostingContext, config) =>
{
    config.ReadFrom.Configuration(builder.Configuration);
});

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    await app.Services.AddDatabaseSeeding();
}

app.UseSerilogRequestLogging();

app.UseAuthentication();
app.UseAuthorization();

app.MapGrpcService<ProductLookupService>();

app.Run();

public partial class Program { }