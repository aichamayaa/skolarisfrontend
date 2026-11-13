using Skolaris.Client.Components;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Render fournit le port via la variable PORT
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// URL publique de l'API (variable d'env ApiUrl, ex: https://skolaris-api.onrender.com/)
var apiUrl = (builder.Configuration["ApiUrl"] ?? "http://localhost:5043").TrimEnd('/') + "/";

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Options JSON partagées (case-insensitive + enums en string)
var jsonOptions = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true
};
jsonOptions.Converters.Add(new JsonStringEnumConverter());
builder.Services.AddSingleton(jsonOptions);

builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri(apiUrl)
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

// HTTPS gere par Render (reverse proxy)
app.UseAntiforgery();
app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();