using Mfm.Api.Configuration.ResponseStandardization;
using Mfm.Application.Configuration;
using Mfm.Infrastructure.Data.Configuration;
using Mfm.Infrastructure.Messaging.Configuration;

var builder = WebApplication.CreateBuilder(args);

_ = builder.Services.AddControllers();

_ = builder.Services.AddEndpointsApiExplorer();
_ = builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(TimeProvider.System);

builder.Services.ConfigureApplication();
builder.Services.ConfigureData(builder.Configuration);
builder.Services.ConfigureMessaging(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
    app.ApplyMigrations();
}

_ = app.UseMiddleware<ErrorMiddleware>();

_ = app.UseHttpsRedirection();

_ = app.UseAuthorization();

_ = app.MapControllers();

app.Run();
