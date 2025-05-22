using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;

namespace FlightStatusHub;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private HubConnection _hubConnection;
    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
        _hubConnection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5000/flightStatusHub")
            .WithAutomaticReconnect()
            .Build();
        
        _hubConnection.On<string>("ReceiveFlightStatus", (status) =>
        {
            _logger.LogInformation("Received flight status: {status}", status);
        });

    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Start the connection
        await _hubConnection.StartAsync(stoppingToken);
        _logger.LogInformation("Hub connection started.");

        // Send a message to the server
        await _hubConnection.InvokeAsync("SendFlightStatus", "123", "boarding...", stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }
            await Task.Delay(1000, stoppingToken);
        }
    }
}
