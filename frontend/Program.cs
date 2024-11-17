using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using frontend;
using Shared.DTOs;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Authorization;
using frontend.Services;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components; // Ensures NavigationManager is recognized

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add the root component
builder.RootComponents.Add<App>("#app");

// Add HeadOutlet component
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HttpClient with base address
builder.Services.AddScoped(sp =>
{
    var navigationManager = sp.GetRequiredService<NavigationManager>();
    return new HttpClient
    {
        BaseAddress = new Uri(navigationManager.BaseUri)
    };
});

// Add Authentication services
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<ILoginService, LoginService>();

await builder.Build().RunAsync();