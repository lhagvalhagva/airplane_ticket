using BlazorClient.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.SignalR;
using System.Net.Http;
using RestApi.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Set text encoding for Mongolian characters
System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add HttpClient service
builder.Services.AddScoped<HttpClient>();

// Add HttpClient with base address
builder.Services.AddScoped(sp => 
{
    var client = new HttpClient { BaseAddress = new Uri("http://localhost:5027/") };
    return client;
});

// Add SignalR client factory
builder.Services.AddSingleton<Func<HubConnection>>(sp => () => 
{
    return new HubConnectionBuilder()
        .WithUrl("http://localhost:5027/flighthub")
        .WithAutomaticReconnect()
        .Build();
});

builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

// Enable CORS for SignalR connections
app.UseCors(cors => cors
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true)
    .AllowCredentials()
);

// Configure SignalR Hub endpoint
app.MapHub<FlightHub>("/flighthub");

// Map Razor components
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
