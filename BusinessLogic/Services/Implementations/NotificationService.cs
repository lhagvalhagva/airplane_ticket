using DataAccess.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using System;
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
                .WithAutomaticReconnect()
                .Build();
            
            // Open connection
            try
            {
                _hubConnection.StartAsync().Wait(5000); // Wait for connection to establish with timeout
                Console.WriteLine("Successfully connected to SignalR Hub");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to SignalR Hub: {ex.Message}");
            }
        }

        public async Task NotifyFlightStatusChangedAsync(int flightId, FlightStatus newStatus)
        {
            Console.WriteLine($"Flight ID: {flightId}, status changed to {newStatus}");
            
            string flightNumber = $"MGL{flightId}";
            
            try
            {
                // Check if connected to hub
                if (_hubConnection.State != HubConnectionState.Connected)
                {
                    Console.WriteLine($"SignalR Hub not connected. Current state: {_hubConnection.State}. Attempting to reconnect...");
                    try {
                        // Try to reconnect
                        await _hubConnection.StartAsync();
                        Console.WriteLine("Reconnected to SignalR Hub successfully");
                    }
                    catch (Exception reconnectEx) {
                        Console.WriteLine($"Failed to reconnect to SignalR Hub: {reconnectEx.Message}");
                    }
                }

                if (_hubConnection.State == HubConnectionState.Connected)
                {
                    Console.WriteLine($"Sending flight status update via SignalR: Flight ID={flightId}, Number={flightNumber}, Status={newStatus}");
                    await _hubConnection.InvokeAsync("NotifyFlightStatusChanged", flightId, flightNumber, newStatus);
                    Console.WriteLine($"Successfully sent flight status change notification via SignalR");
                }
                else
                {
                    Console.WriteLine($"Cannot send notification - SignalR Hub still not connected. Current state: {_hubConnection.State}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending flight status change notification: {ex.Message}");
                Console.WriteLine($"Exception details: {ex.StackTrace}");
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