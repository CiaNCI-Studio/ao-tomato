using System.Text.Json.Serialization;
using AoTomato.API.DependencyInjection;
using AoTomato.API.Endpoints.FunctionLogs;
using AoTomato.API.Endpoints.Functions;
using AoTomato.API.Endpoints.Login;
using AoTomato.API.Endpoints.Settings;
using AoTomato.API.Endpoints.Users;
using AoTomato.API.Endpoints.Variables;
using AoTomato.API.Extensions;
using AoTomato.API.Middlewares;
using AoTomato.Domain.Common.Models;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

Log.Logger = new LoggerConfiguration()
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .CreateLogger();

builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
builder.AddConfiguration<DbSettings>();
builder.ConfigureCors();

builder.Services.AddSingleton(Log.Logger);
builder.Services.AddRepositories();
builder.Services.AddServices();
builder.Services.AddSettings();
builder.Services.AddMappers();
builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.SetServiceProviderHelper();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.MapOpenApi();
app.MapScalarApiReference("/docs");

app.UseHttpsRedirection();
app.AddCorsCofiguration();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.AddAuthentication();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseRouting();
app.MapLoginEndpoints();
app.MapUsersEndpoints();
app.MapSettingsEndpoints();
app.MapFunctionsEndpoints();
app.MapFunctionLogsEndpoints();
app.MapVariablesEndpoints();
app.MapAoEndpoints();
app.MapRazorPages();
app.MapFallbackToFile("index.html");

app.Run();