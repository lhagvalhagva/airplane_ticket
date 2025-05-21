using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SignalRHubLibrary
{
    public class FlightHub : Hub
    {
        public async Task JoinFlightGroup(string flightNumber)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, flightNumber);
        }

        public async Task LeaveFlightGroup(string flightNumber)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, flightNumber);
        }

        public async Task UpdateFlightStatus(string flightNumber, string status)
        {
            await Clients.Group(flightNumber).SendAsync("FlightStatusUpdated", flightNumber, status);
        }
    }
}