using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AirplaneTicket.WPF.Models;
using System.Collections.Generic;
//using Microsoft.AspNetCore.SignalR.Client;
using System.Windows;

namespace AirplaneTicket.WPF.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        //private readonly HubConnection _hubConnection;
        private const string BaseUrl = "http://localhost:5027/api";
        //private const string HubUrl = "https://localhost:7001/flightHub";

        //public event EventHandler<Flight> FlightUpdated;
        //public event EventHandler<BoardingPass> BoardingPassCreated;

        public ApiService()
        {
            _httpClient = new HttpClient();
            /*_hubConnection = new HubConnectionBuilder()
                .WithUrl(HubUrl)
                .WithAutomaticReconnect()
                .Build();

            InitializeSignalR();*/
        }

        /*private async void InitializeSignalR()
        {
            try
            {
                _hubConnection.On<Flight>("FlightUpdated", flight =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        FlightUpdated?.Invoke(this, flight);
                    });
                });

                _hubConnection.On<BoardingPass>("BoardingPassCreated", boardingPass =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        BoardingPassCreated?.Invoke(this, boardingPass);
                    });
                });

                await _hubConnection.StartAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error connecting to SignalR hub: {ex.Message}", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }*/

        // Flight endpoints
        public async Task<List<Flight>> GetFlightsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Flight>>($"{BaseUrl}/flights");
        }

        public async Task<Flight> GetFlightAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Flight>($"{BaseUrl}/flights/{id}");
        }

        public async Task<Flight> UpdateFlightAsync(Flight flight)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/flights/{flight.Id}", flight);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Flight>();
        }

        // Passenger endpoints
        public async Task<List<Passenger>> GetPassengersAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Passenger>>($"{BaseUrl}/passengers");
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

        // Boarding Pass endpoints
        public async Task<List<BoardingPass>> GetBoardingPassesAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<BoardingPass>>($"{BaseUrl}/boardingpasses");
        }

        public async Task<BoardingPass> GetBoardingPassAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<BoardingPass>($"{BaseUrl}/boardingpasses/{id}");
        }

        public async Task<BoardingPass> CreateBoardingPassAsync(BoardingPass boardingPass)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/boardingpasses", boardingPass);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<BoardingPass>();
        }

        public async Task<BoardingPass> UpdateBoardingPassAsync(BoardingPass boardingPass)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/boardingpasses/{boardingPass.Id}", boardingPass);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<BoardingPass>();
        }

        public async Task DeleteBoardingPassAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/boardingpasses/{id}");
            response.EnsureSuccessStatusCode();
        }

        // Seat endpoints
        public async Task<List<string>> GetAvailableSeatsAsync(int flightId)
        {
            return await _httpClient.GetFromJsonAsync<List<string>>($"{BaseUrl}/seats/available/{flightId}");
        }

        public async Task<bool> ReserveSeatAsync(int flightId, string seatNumber)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/seats/reserve", new { FlightId = flightId, SeatNumber = seatNumber });
            return response.IsSuccessStatusCode;
        }

        public void Dispose()
        {
            //_hubConnection?.DisposeAsync();
            _httpClient?.Dispose();
        }
    }
} 