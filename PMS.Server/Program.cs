using PMS.Server.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

builder.Host.UseSerilog((hostingContext, config) =>
{
    config.ReadFrom.Configuration(builder.Configuration);
});

var app = builder.Build();

app.UseSerilogRequestLogging();

app.MapGrpcService<ProductLookupService>();

app.Run();
