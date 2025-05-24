using FlightDashboard.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace FlightDashboard.Services
{
    public interface IFlightService
    {
        Task<List<Flight>> GetAllFlightsAsync();
        Task<Flight> GetFlightByIdAsync(int id);
    }

    public class FlightService : IFlightService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public FlightService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ApiSettings:BaseUrl"] ?? "http://10.3.132.225:5027/api/Flights";
        }

        public async Task<List<Flight>> GetAllFlightsAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Flight>>(_baseUrl);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Нислэгийн жагсаалт авах үед алдаа гарлаа: {ex.Message}");
                return new List<Flight>();
            }
        }

        public async Task<Flight> GetFlightByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Flight>($"{_baseUrl}/{id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Нислэгийн дэлгэрэнгүй авах үед алдаа гарлаа: {ex.Message}");
                return null;
            }
        }
    }
}
