using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using FlightDashboard;
using FlightDashboard.Services;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Энгийн HttpClient бүртгэх
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Configuration үүсгэх (appsettings.json уншихын оронд хатуу кодоор тохируулах)
var configuration = new ConfigurationBuilder()
    .AddInMemoryCollection(new Dictionary<string, string>
    {
        ["ApiSettings:BaseUrl"] = "http://10.3.132.225:5027/api/Flights",
        ["ApiSettings:SignalRUrl"] = "http://10.3.132.225:5027/flightHub"
    })
    .Build();
builder.Services.AddSingleton<IConfiguration>(configuration);

// Services бүртгэх
builder.Services.AddScoped<IFlightService, FlightService>();
builder.Services.AddSingleton<ISignalRService, SignalRService>();

await builder.Build().RunAsync();
