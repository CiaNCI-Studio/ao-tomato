using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using AoTomato.Admin;
using AoTomato.Admin.Abstractions;
using AoTomato.Admin.Services;
using MudBlazor.Services;
using Blazored.LocalStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();
builder.Services.AddBlazoredLocalStorageAsSingleton();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IFunctionsService, FunctionsService>();
builder.Services.AddScoped<IVariablesService, VariablesService>();
builder.Services.AddScoped<ISettingsService, SettingsService>();
builder.Services.AddScoped<IAdminService, AdminService>();

await builder.Build().RunAsync();
