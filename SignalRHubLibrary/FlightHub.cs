using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using System.Text.Json;
using System.Linq;

namespace SignalRHubLibrary
{
    public class FlightHub : Hub
    {
        /// <summary>
        /// Нислэгийн төлөв өөрчлөгдсөн үед клиентүүдэд мэдэгдэх
        /// </summary>
        /// <param name="flightId">Нислэгийн ID</param>
        /// <param name="status">Шинэ төлөвийн тоон утга</param>
        public async Task NotifyFlightStatusChanged(int flightId, int status)
        {
            Console.WriteLine($"========== FLIGHT STATUS CHANGED ===========");
            Console.WriteLine($"FlightHub: Flight ID: {flightId}, new status: {status}");
            Console.WriteLine($"Connected clients count: {Clients.All.ToString()}");

            try 
            {
                // Notify all clients
                await Clients.All.SendAsync("FlightStatusChanged", flightId, status);
                Console.WriteLine($"Notification sent successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending notification: {ex.Message}");
            }
        }

        /// <summary>
        /// Тодорхой нислэгийн мэдээллийг хүлээн авах бүлэгт нэгдэх
        /// </summary>
        /// <param name="flightId">Нислэгийн ID</param>
        public async Task JoinFlightGroup(int flightId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"flight_{flightId}");
        }

        /// <summary>
        /// Холболтын үед дуудагдах callback
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("Connected", "Нислэгийн мэдээллийн серверт амжилттай холбогдлоо");
            await base.OnConnectedAsync();
        }
    }
}