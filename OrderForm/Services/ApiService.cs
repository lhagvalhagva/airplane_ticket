using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OrderForm.Models;

namespace OrderForm.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public ApiService(string apiBaseUrl)
        {
            _httpClient = new HttpClient();
            _apiBaseUrl = apiBaseUrl;
        }

        #region Flights

        public async Task<List<FlightDto>> GetAllFlightsAsync()
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Flights");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var flights = JsonConvert.DeserializeObject<List<FlightDto>>(content);
                return flights ?? new List<FlightDto>();
            }
            
            throw new Exception($"Нислэгүүдийн мэдээлэл авахад алдаа гарлаа: {response.StatusCode}");
        }

        public async Task<bool> RegisterFlightAsync(FlightDto flight)
        {
            var json = JsonConvert.SerializeObject(flight);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/Flights", content);
            
            return response.IsSuccessStatusCode;
        }

        public async Task<FlightDto> GetFlightByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Flights/{id}");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<FlightDto>(content);
            }
            
            throw new Exception($"Нислэгийн мэдээлэл авахад алдаа гарлаа: {response.StatusCode}");
        }

        public async Task<bool> UpdateFlightStatusAsync(int flightId, FlightStatus newStatus)
        {
            var statusDto = new UpdateFlightStatusDto { Status = newStatus };
            var json = JsonConvert.SerializeObject(statusDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PutAsync($"{_apiBaseUrl}/Flights/{flightId}/status", content);
            
            return response.IsSuccessStatusCode;
        }

        #endregion

        #region Passengers

        public async Task<PassengerDto> GetPassengerAsync(string passportNumber)
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Passengers/passport/{passportNumber}");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<PassengerDto>(content);
            }
            
            throw new Exception($"Зорчигчийн мэдээлэл авахад алдаа гарлаа: {response.StatusCode}");
        }
        
        // Алдааг засахад зориулж хуучн нэрийг хадгалав
        public async Task<PassengerDto> GetPassengerByPassportNumberAsync(string passportNumber)
        {
            return await GetPassengerAsync(passportNumber);
        }

        public async Task<List<PassengerDto>> GetFlightPassengersAsync(int flightId)
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Passengers/flight/{flightId}");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<PassengerDto>>(content) ?? new List<PassengerDto>();
            }
            
            throw new Exception($"Нислэгийн зорчигчдын мэдээлэл авахад алдаа гарлаа: {response.StatusCode}");
        }
        
        // Алдааг засахад зориулж хуучн нэрийг хадгалав
        public async Task<List<PassengerDto>> GetPassengersByFlightIdAsync(int flightId)
        {
            return await GetFlightPassengersAsync(flightId);
        }
        
        public async Task<List<PassengerDto>> GetAllPassengersAsync()
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Passengers");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var passengers = JsonConvert.DeserializeObject<List<PassengerDto>>(content);
                return passengers ?? new List<PassengerDto>();
            }
            
            throw new Exception($"Зорчигчдын жагсаалт авахад алдаа гарлаа: {response.StatusCode}");
        }
        
        public async Task<bool> RegisterPassengerAsync(PassengerDto passenger)
        {
            var json = JsonConvert.SerializeObject(passenger);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/Passengers", content);
            
            return response.IsSuccessStatusCode;
        }

        #endregion

        #region Check-in

        public async Task<BoardingPassDto> CheckInPassengerAsync(int flightId, string passportNumber, string seatNumber)
        {
            var checkInDto = new CheckInDto
            {
                FlightId = flightId,
                PassportNumber = passportNumber,
                SeatNumber = seatNumber
            };
            
            var json = JsonConvert.SerializeObject(checkInDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/Boarding/checkin", content);
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<BoardingPassDto>(responseContent) ?? 
                       new BoardingPassDto { FlightId = flightId, SeatNumber = seatNumber };
            }
            
            throw new Exception($"Зорчигч бүртгэхэд алдаа гарлаа: {response.StatusCode}");
        }
        
        public async Task<BoardingPassDto> GetBoardingPassByFlightAndPassengerAsync(int flightId, int passengerId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Boarding/flights/{flightId}/boardingpasses");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var boardingPasses = JsonConvert.DeserializeObject<List<BoardingPassDto>>(content);
                    return boardingPasses?.FirstOrDefault(bp => bp.PassengerId == passengerId && bp.FlightId == flightId);
                }
                
                return null;
            }
            catch
            {
                return null; // Алдаа гарвал null буцаана
            }
        }

        #endregion

        #region Seats

        public async Task<List<SeatDto>> GetAvailableSeatsAsync(int flightId)
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Flights/{flightId}/seats/available");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<SeatDto>>(content) ?? new List<SeatDto>();
            }
            
            return new List<SeatDto>();
        }
        
        public async Task<List<SeatDto>> GetAllSeatsForFlightAsync(int flightId)
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Flights/{flightId}/seats");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<SeatDto>>(content) ?? new List<SeatDto>();
            }
            
            return new List<SeatDto>();
        }
        
        public async Task<List<SeatDto>> GetAllSeatsAsync(int flightId)
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Boarding/flights/{flightId}/seats/all");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<SeatDto>>(content) ?? new List<SeatDto>();
            }
            
            throw new Exception($"Суудлын мэдээлэл авахад алдаа гарлаа: {response.StatusCode}");
        }

        #endregion

        #region Database Initialization

        public async Task<bool> InitializeDatabaseAsync()
        {
            try
            {
                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/DbInitializer/initialize", null);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Өгөгдлийн санг эхлүүлэхэд алдаа гарлаа: {ex.Message}");
                return false;
            }
        }

        #endregion
    }

    public class UpdateFlightStatusDto
    {
        public FlightStatus Status { get; set; }
    }

    public class CheckInDto
    {
        public int FlightId { get; set; }
        public string PassportNumber { get; set; }
        public string SeatNumber { get; set; }
    }
}