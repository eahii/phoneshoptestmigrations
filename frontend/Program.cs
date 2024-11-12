using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using frontend;
using Shared.Models;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Authorization;
using frontend.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Lisää sovelluksen pääkomponentti App. Tämä komponentti renderöidään HTML-sivulle kohteessa, jossa on id="app".
builder.RootComponents.Add<App>("#app");

// Lisää HeadOutlet-komponentti <head>-elementtiin. Tämä mahdollistaa Blazor-sovelluksen dynaamisen sisältöjen lisäyksen head-osaan.
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HttpClient without ConfigureHttpClient method
builder.Services.AddScoped(sp =>
{
    var client = new HttpClient { BaseAddress = new Uri("http://localhost:5088") };
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    return client;
});

// Add Authentication services
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<ILoginService, LoginService>();

// Rakentaa ja käynnistää Blazor WebAssembly -sovelluksen
await builder.Build().RunAsync();
