using Microsoft.EntityFrameworkCore;
using DataAccess;
using DataAccess.Repositories;
using BusinessLogic.Services;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SignalR;
using SignalRHubLibrary;

var builder = WebApplication.CreateBuilder(args);

// Set text encoding for Mongolian characters
System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS with explicit WebSocket support
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder
                .SetIsOriginAllowed(_ => true) // Allow any origin for testing
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .WithExposedHeaders("X-Requested-With");
        });
});

// Configure DbContext
builder.Services.AddDbContext<AirportDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Register services
builder.Services.AddScoped<IFlightService, FlightService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IPassengerService, PassengerService>();
builder.Services.AddScoped<ISeatService, SeatService>();
builder.Services.AddScoped<IFlightPassengerService, FlightPassengerService>();

// Add Response Compression Middleware
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        ["application/octet-stream"]);
});

// Add SignalR with improved stability configuration
builder.Services.AddSignalR(options => {
    // Increase timeout settings to prevent disconnects
    options.ClientTimeoutInterval = TimeSpan.FromMinutes(2);
    options.KeepAliveInterval = TimeSpan.FromMinutes(1);
    // Enable detailed error reporting
    options.EnableDetailedErrors = true;
});

// DO NOT manually register IHubContext - it's automatically provided by AddSignalR

var app = builder.Build();

// Use Response Compression
app.UseResponseCompression();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();

app.MapControllers();

// Configure SignalR Hub
app.MapHub<FlightHub>("/flightHub");

// Use URLs from configuration or default ports for all interfaces
// Comment out hardcoded localhost URLs to use appsettings.json configuration
// app.Urls.Add("http://localhost:5000");
// app.Urls.Add("http://localhost:5027");

app.Run();