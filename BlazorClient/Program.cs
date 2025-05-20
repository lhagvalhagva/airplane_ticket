using BlazorClient.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http;
using SignalRHubLibrary;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add HttpClient service
builder.Services.AddScoped<HttpClient>();

// Add SignalR client
builder.Services.AddSingleton<HubConnection>(sp =>
{
    var hubUrl = builder.Configuration["SignalR:HubUrl"] ?? "https://localhost:5027/flightHub";
    return new HubConnectionBuilder()
        .WithUrl(hubUrl)
        .WithAutomaticReconnect()
        .Build();
});

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

// Configure SignalR Hub endpoint
app.MapHub<SignalRHubLibrary.FlightHub>("/flightHub");

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
