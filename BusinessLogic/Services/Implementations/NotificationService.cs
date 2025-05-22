using DataAccess.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class NotificationService : INotificationService
    {
        private readonly NotificationTarget _defaultTarget;
        private readonly HubConnection _hubConnection;
        
        public NotificationService()
        {
            _defaultTarget = NotificationTarget.Both;
            
            // Create SignalR client connection
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5027/flighthub")
                .WithAutomaticReconnect(new[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(5) })
                .Build();
            
            // Register for connection events
            _hubConnection.Closed += async (error) => {
                Console.WriteLine($"SignalR connection closed: {error?.Message}");
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await EnsureConnectedAsync();
            };
            
            // Open connection asynchronously - avoid blocking with Wait()
            Task.Run(async () => {
                try {
                    await EnsureConnectedAsync();
                    Console.WriteLine("Initial SignalR connection established asynchronously");
                }
                catch (Exception ex) {
                    Console.WriteLine($"Initial SignalR connection failed: {ex.Message}");
                }
            });
        }
        
        private async Task<bool> EnsureConnectedAsync()
        {
            try
            {
                if (_hubConnection.State != HubConnectionState.Connected)
                {
                    Console.WriteLine($"Connecting to SignalR Hub (current state: {_hubConnection.State})...");
                    
                    // Set a timeout for connection attempt
                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                    await _hubConnection.StartAsync(cts.Token);
                    
                    Console.WriteLine($"Successfully connected to SignalR Hub (ID: {_hubConnection.ConnectionId})");
                    return true;
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to SignalR Hub: {ex.Message}");
                Console.WriteLine($"Exception type: {ex.GetType().Name}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        public async Task NotifyFlightStatusChangedAsync(int flightId, FlightStatus newStatus)
        {
            Console.WriteLine($"========== SENDING NOTIFICATION ===========");
            Console.WriteLine($"Flight ID: {flightId}, status changed to {newStatus} (value: {(int)newStatus})");
            
            // Create notification data with timestamp
            var notificationData = new {
                FlightId = flightId,
                NewStatus = newStatus,
                NewStatusValue = (int)newStatus,
                Timestamp = DateTime.Now
            };

            try
            {
                // Ensure connection before sending
                Console.WriteLine($"Checking SignalR connection state: {_hubConnection.State}");
                bool isConnected = await EnsureConnectedAsync();
                
                if (isConnected)
                {
                    // Call NotifyFlightStatusChanged method on the hub
                    Console.WriteLine($"Preparing to send via SignalR: {JsonSerializer.Serialize(notificationData)}");
                    Console.WriteLine($"Hub method name: NotifyFlightStatusChanged");
                    Console.WriteLine($"Parameters: [{flightId}, {newStatus}] (value: {(int)newStatus})");
                    
                    // Try multiple times if needed
                    for (int attempt = 1; attempt <= 3; attempt++)
                    {
                        try
                        {
                            Console.WriteLine($"Sending attempt {attempt}/3...");
                            
                            // Send directly to ReceiveFlightStatusUpdate for more reliable delivery
                            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                            await _hubConnection.InvokeAsync(
                                "ReceiveFlightStatusUpdate",
                                flightId,
                                newStatus,
                                cts.Token);
                                
                            Console.WriteLine($"SUCCESS: Direct notification sent");
                            break; // Success, exit retry loop
                        }
                        catch (Exception sendEx) when (attempt < 3)
                        {
                            Console.WriteLine($"FAILED: Attempt {attempt} error: {sendEx.Message}");
                            Console.WriteLine($"Exception type: {sendEx.GetType().Name}");
                            Console.WriteLine($"Will retry in {500 * attempt}ms...");
                            await Task.Delay(500 * attempt); // Increasing delay between retries
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"ERROR: Cannot send notification - Unable to establish SignalR connection");
                }
                
                Console.WriteLine($"========== NOTIFICATION COMPLETE ============");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: Flight status notification failed: {ex.Message}");
                Console.WriteLine($"Exception type: {ex.GetType().Name}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        public async Task NotifySeatAssignedAsync(int flightId, string seatNumber, int passengerId)
        {
            Console.WriteLine($"Нислэг ID: {flightId}, {seatNumber} суудал зорчигч ID: {passengerId}-д хуваарилагдлаа");
            
            await Task.CompletedTask;
        }
        
        // public async Task NotifyPassengerRegisteredAsync(int flightId, int passengerId){     Console.WriteLine($"Зорчигч ID: {passengerId} нь нислэг ID: {flightId} дээр бүртгэгдлээ");     await Task.CompletedTask;}
        // public async Task NotifyPassengerUnregisteredAsync(int flightId, int passengerId){     Console.WriteLine($"Зорчигч ID: {passengerId} нь нислэг ID: {flightId} дээр хасагдлаа");     await Task.CompletedTask;}
    }
}