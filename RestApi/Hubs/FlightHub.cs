using DataAccess.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace RestApi.Hubs
{
    public class FlightHub : Hub
    {
        /// <summary>
        /// Нислэгийн төлөв өөрчлөгдсөн тухай мэдэгдэл дамжуулах метод
        /// </summary>
        /// <param name="flightId">Нислэгийн ID</param>
        /// <param name="newStatus">Шинэ төлөв</param>
        public async Task NotifyFlightStatusChanged(int flightId, FlightStatus newStatus)
        {
            try
            {
                Console.WriteLine($"[FlightHub] Sending status update to clients: Flight ID={flightId}, Status={newStatus}");
                
                // Send to specific clients if needed, otherwise broadcast to all
                await Clients.All.SendAsync("ReceiveFlightStatusUpdate", flightId, newStatus);
                
                Console.WriteLine($"[FlightHub] Status update sent successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FlightHub] Error sending update: {ex.Message}");
                Console.WriteLine(ex.ToString());
            }
        }
        
        /// <summary>
        /// Нислэгийн төлөв өөрчлөгдсөн мэдэгдлийг шууд клиентуудад илгээх
        /// </summary>
        /// <param name="flightId">Нислэгийн ID</param>
        /// <param name="newStatus">Шинэ төлөв</param>
        public async Task ReceiveFlightStatusUpdate(int flightId, FlightStatus newStatus)
        {
            try
            {
                Console.WriteLine($"[FlightHub] Direct status update to clients: Flight ID={flightId}, Status={newStatus}");
                
                // Broadcast to all clients
                await Clients.All.SendAsync("ReceiveFlightStatusUpdate", flightId, newStatus);
                
                Console.WriteLine($"[FlightHub] Direct status update sent successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FlightHub] Error sending direct update: {ex.Message}");
                Console.WriteLine(ex.ToString());
            }
        }
        
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"[FlightHub] Client connected: {Context.ConnectionId}");
            
            try
            {
                // Send active flights status to newly connected client
                Console.WriteLine($"[FlightHub] Sending initial status to new client: {Context.ConnectionId}");
                
                // We'd ideally fetch current flight statuses and send them
                // This is just a demonstration of how we could send a test update
                await Task.Delay(1000); // Give client time to setup handler
                
                // Send a test notification to this specific client
                await Clients.Client(Context.ConnectionId).SendAsync(
                    "ReceiveFlightStatusUpdate", 
                    1, // MN101 flight ID
                    FlightStatus.CheckingIn); // Example status
                
                Console.WriteLine($"[FlightHub] Sent test status update to client: {Context.ConnectionId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FlightHub] Error sending initial status: {ex.Message}");
            }
            
            await base.OnConnectedAsync();
        }
        
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"[FlightHub] Client disconnected: {Context.ConnectionId}. Reason: {exception?.Message ?? "Unknown"}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
