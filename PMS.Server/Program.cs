using PMS.Server.Services;
using PMS.Server.Data.Repositories;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddRepositories();

builder.Host.UseSerilog((hostingContext, config) =>
{
    config.ReadFrom.Configuration(builder.Configuration);
});

var app = builder.Build();

app.UseSerilogRequestLogging();

app.MapGrpcService<ProductLookupService>();

app.Run();
