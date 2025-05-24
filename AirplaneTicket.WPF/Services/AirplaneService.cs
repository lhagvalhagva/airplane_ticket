using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirplaneTicket.WPF.Models;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace AirplaneTicket.WPF.Services
{
    public class AirplaneService : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public AirplaneService()
        {
            _httpClient = new HttpClient();
            
            // Load configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
            
            _baseUrl = configuration["ServerSettings:ApiBaseUrl"] ?? "http://10.3.132.225:5027/api";
        }

        #region Flight Operations
        public async Task<List<Flight>> GetFlightsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Flight>>($"{_baseUrl}/Flights");
        }

        public async Task<Flight> GetFlightAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Flight>($"{_baseUrl}/Flights/{id}");
        }

        public async Task<Flight> CreateFlightAsync(Flight flight)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/Flights", flight);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Flight>();
        }

        public async Task<Flight> UpdateFlightAsync(Flight flight)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/Flights/{flight.Id}", flight);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Flight>();
        }

        public async Task DeleteFlightAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/Flights/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<FlightStatus> UpdateFlightStatusAsync(int flightId, FlightStatus status)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/Flights/{flightId}/status", status);
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
            return await _httpClient.GetFromJsonAsync<IEnumerable<Seat>>($"{_baseUrl}/Flights/{flightId}/seats");
        }

        public async Task<IEnumerable<Seat>> GetAvailableSeatsAsync(int flightId)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Seat>>($"{_baseUrl}/Flights/{flightId}/seats/available");
        }

        public async Task<bool> AssignSeatAsync(int flightId, int passengerId, int seatId)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/Flights/{flightId}/seats/assign", new
            {
                PassengerId = passengerId,
                SeatId = seatId
            });
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> ReleaseSeatAsync(int flightId, int seatId, int newPassengerId)
        {
            try
            {
                // 1. Одоогийн суудлыг чөлөөлөх
                var releaseResponse = await _httpClient.PutAsync($"{_baseUrl}/Flights/{flightId}/seats/{seatId}/release", null);
                if (!releaseResponse.IsSuccessStatusCode)
                    return false;
                
                // 2. Одоогийн суудлыг авах
                var seat = (await GetFlightSeatsAsync(flightId)).FirstOrDefault(s => s.Id == seatId);
                if (seat == null)
                    return false;
                    
                // 3. Шинэ суудлыг авах (бид зорчигчийн байсан суудлыг авч байна)
                var oldSeats = await GetPassengerSeatsAsync(flightId, newPassengerId);
                var oldSeat = oldSeats.FirstOrDefault();
                
                // 4. Шинэ зорчигчийг суудалд оноох
                var assignResponse = await AssignSeatAsync(flightId, newPassengerId, seatId);
                if (!assignResponse)
                    return false;
                    
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ReserveSeatAsync(int flightId, string seatNumber)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/seats/reserve", new { FlightId = flightId, SeatNumber = seatNumber });
            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<Seat>> GetPassengerSeatsAsync(int flightId, int passengerId)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Seat>>($"{_baseUrl}/Flights/{flightId}/seats/passenger/{passengerId}");
        }
        #endregion

        #region Passenger Operations
        public async Task<List<Passenger>> GetPassengersAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Passenger>>($"{_baseUrl}/Passengers");
        }

        public async Task<List<BoardingPass>> GetBoardingPassesAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<BoardingPass>>($"{_baseUrl}/boardingpasses");
        }

        public async Task<List<Passenger>> GetPassengersByFlightAsync(int flightId)
        {
            return await _httpClient.GetFromJsonAsync<List<Passenger>>($"{_baseUrl}/Flights/{flightId}/passengers");
        }

        public async Task<Passenger> GetPassengerAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Passenger>($"{_baseUrl}/passengers/{id}");
        }

        public async Task<Passenger> CreatePassengerAsync(Passenger passenger)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/Passengers", passenger);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Passenger>();
        }

        public async Task<Passenger> UpdatePassengerAsync(Passenger passenger)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/passengers/{passenger.Id}", passenger);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Passenger>();
        }

        public async Task DeletePassengerAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/passengers/{id}");
            response.EnsureSuccessStatusCode();
        }
        #endregion

        #region Flight Passenger Operations
        public async Task<FlightPassenger> RegisterPassengerToFlightAsync(int flightId, int passengerId)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/Flights/{flightId}/passengers", new { PassengerId = passengerId });
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<FlightPassenger>();
        }

        public async Task RemovePassengerFromFlightAsync(int flightId, int passengerId)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/Flights/{flightId}/passengers/{passengerId}");
            response.EnsureSuccessStatusCode();
        }
        #endregion

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
} 