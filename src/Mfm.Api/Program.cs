using Mfm.Api;
using Mfm.Api.Configuration.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureLogging();

var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();

startup.Configure(app);

app.Run();
