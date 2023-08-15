using Blazored.LocalStorage;
using Blazored.LocalStorage.StorageOptions;
using Jobbvin.Client;
using Jobbvin.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Security.Cryptography.X509Certificates;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:44371/") });
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<IJobbvinServiceClient, JobbvinServiceClient>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

var app = builder.Build();

var authService= app.Services.GetService<IAuthenticationService>();

if (authService != null)
    await authService.Initialize();

await builder.Build().RunAsync();
