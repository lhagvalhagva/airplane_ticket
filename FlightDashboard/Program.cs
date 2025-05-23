using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using FlightDashboard;
using FlightDashboard.Services;
using System;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// API сервер рүү хандах HttpClient бүртгэх
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// FlightService-ийг бүртгэх
builder.Services.AddScoped<IFlightService, FlightService>();

await builder.Build().RunAsync();
