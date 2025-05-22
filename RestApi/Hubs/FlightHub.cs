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
        /// <param name="flightNumber">Нислэгийн дугаар</param>
        /// <param name="newStatus">Шинэ төлөв</param>
        public async Task NotifyFlightStatusChanged(int flightId, string flightNumber, FlightStatus newStatus)
        {
            try
            {
                Console.WriteLine($"[FlightHub] Sending status update to clients: Flight ID={flightId}, Number={flightNumber}, Status={newStatus}");
                await Clients.All.SendAsync("ReceiveFlightStatusUpdate", flightId, flightNumber, newStatus);
                Console.WriteLine($"[FlightHub] Status update sent successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FlightHub] Error sending update: {ex.Message}");
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
