using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirplaneTicket.WPF.Models;
using System.Collections.Generic;
using System.Windows;

namespace AirplaneTicket.WPF.Services
{
    public class AirplaneService : IDisposable
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://localhost:5027/api";

        public AirplaneService()
        {
            _httpClient = new HttpClient();
        }

        #region Flight Operations
        public async Task<List<Flight>> GetFlightsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Flight>>($"{BaseUrl}/flights");
        }

        public async Task<Flight> GetFlightAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Flight>($"{BaseUrl}/flights/{id}");
        }

        public async Task<Flight> CreateFlightAsync(Flight flight)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/flights", flight);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Flight>();
        }

        public async Task<Flight> UpdateFlightAsync(Flight flight)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/flights/{flight.Id}", flight);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Flight>();
        }

        public async Task DeleteFlightAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/flights/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<FlightStatus> UpdateFlightStatusAsync(int flightId, FlightStatus status)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/flights/{flightId}/status", status);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<FlightStatus>();
            }
            throw new Exception("Failed to update flight status");
        }
        #endregion

        #region Seat Operations
        public async Task<IEnumerable<Seat>> GetFlightSeatsAsync(int flightId)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Seat>>($"{BaseUrl}/flights/{flightId}/seats");
        }

        public async Task<IEnumerable<Seat>> GetAvailableSeatsAsync(int flightId)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Seat>>($"{BaseUrl}/flights/{flightId}/seats/available");
        }

        public async Task<bool> AssignSeatAsync(int flightId, int passengerId, int seatId)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/flights/{flightId}/seats/assign", new
            {
                PassengerId = passengerId,
                SeatId = seatId
            });
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ReleaseSeatAsync(int flightId, int seatId)
        {
            var response = await _httpClient.PutAsync($"{BaseUrl}/flights/{flightId}/seats/{seatId}/release", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ReleaseSeatWithNewPassengerAsync(int flightId, int seatId, int newPassengerId)
        {
            var response = await _httpClient.PutAsync($"{BaseUrl}/flights/{flightId}/seats/{seatId}/release?newPassengerId={newPassengerId}", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ReserveSeatAsync(int flightId, string seatNumber)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/seats/reserve", new { FlightId = flightId, SeatNumber = seatNumber });
            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<Seat>> GetPassengerSeatsAsync(int flightId, int passengerId)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Seat>>($"{BaseUrl}/flights/{flightId}/seats/passenger/{passengerId}");
        }
        #endregion

        #region Passenger Operations
        public async Task<List<Passenger>> GetPassengersAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Passenger>>($"{BaseUrl}/passengers");
        }

        public async Task<List<BoardingPass>> GetBoardingPassesAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<BoardingPass>>($"{BaseUrl}/boardingpasses");
        }

        public async Task<List<Passenger>> GetPassengersByFlightAsync(int flightId)
        {
            return await _httpClient.GetFromJsonAsync<List<Passenger>>($"{BaseUrl}/flights/{flightId}/passengers");
        }

        public async Task<Passenger> GetPassengerAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Passenger>($"{BaseUrl}/passengers/{id}");
        }

        public async Task<Passenger> CreatePassengerAsync(Passenger passenger)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/passengers", passenger);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Passenger>();
        }

        public async Task<Passenger> UpdatePassengerAsync(Passenger passenger)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/passengers/{passenger.Id}", passenger);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Passenger>();
        }

        public async Task DeletePassengerAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/passengers/{id}");
            response.EnsureSuccessStatusCode();
        }
        #endregion

        #region Flight Passenger Operations
        public async Task<FlightPassenger> RegisterPassengerToFlightAsync(int flightId, int passengerId)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/flights/{flightId}/passengers", new { PassengerId = passengerId });
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<FlightPassenger>();
        }

        public async Task RemovePassengerFromFlightAsync(int flightId, int passengerId)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/flights/{flightId}/passengers/{passengerId}");
            response.EnsureSuccessStatusCode();
        }
        #endregion

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
} 