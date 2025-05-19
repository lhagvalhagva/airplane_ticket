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

        public async Task<PassengerDto?> GetPassengerByIdAsync(int passengerId)
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Passengers/{passengerId}");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<PassengerDto>(content);
            }
            
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            
            throw new Exception($"Зорчигчийн мэдээлэл авах үед алдаа гарлаа: {response.StatusCode}");
        }
        
        
        public async Task<PassengerDto> GetPassengerByPassportNumberAsync(string passportNumber)
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Passengers/passport/{passportNumber}");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<PassengerDto>(content);
            }
            
            throw new Exception($"Зорчигчийн мэдээлэл авахад алдаа гарлаа: {response.StatusCode}");
        }
        


        public async Task<List<PassengerDto>> GetPassengersByFlightIdAsync(int flightId)
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Passengers/flight/{flightId}");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<PassengerDto>>(content) ?? new List<PassengerDto>();
            }
            
            throw new Exception($"Нислэгийн зорчигчдын мэдээлэл авахад алдаа гарлаа: {response.StatusCode}");
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
            try
            {
                // Эхлээд хэрэглэгчид суудал оноох API-г ашиглах (новый метод)
                int passengerId = 0;
                
                // Паспортаар хэрэглэгчийн ID-г хайх
                var passenger = await GetPassengerByPassportNumberAsync(passportNumber);
                if (passenger != null && passenger.Id > 0)
                {
                    passengerId = passenger.Id;
                    
                    // Суудал оноох API-г дуудах
                    var assignResult = await AssignSeatToPassengerAsync(flightId, passengerId, seatNumber);
                    
                    if (assignResult)
                    {
                        // Хэрэв суудал амжилттай оноогдсон бол, CheckIn хийх
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
                            return JsonConvert.DeserializeObject<BoardingPassDto>(responseContent) ?? new BoardingPassDto();
                        }
                        
                        throw new Exception($"Зорчигч бүртгэх: {response.StatusCode}");
                    }
                    else
                    {
                        throw new Exception("Суудал оноох үйлдэл амжилтгүй болсон.");
                    }
                }
                else
                {
                    throw new Exception($"{passportNumber} паспортын дугаартай зорчигч олдсонгүй.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Зорчигч бүртгэх алдаа: {ex.Message}");
            }
        }
        
        public async Task<bool> AssignSeatToPassengerAsync(int flightId, int passengerId, string seatNumber)
        {
            try
            {
                // REST API руу шууд хүсэлт илгээх
                var response = await _httpClient.PostAsync(
                    $"{_apiBaseUrl}/Boarding/flights/{flightId}/passengers/{passengerId}/seat?seatNumber={seatNumber}", 
                    new StringContent("", Encoding.UTF8, "application/json"));
                
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Суудал оноох алдаа: {ex.Message}");
                return false;
            }
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
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Boarding/flights/{flightId}/seats/available");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var seats = JsonConvert.DeserializeObject<List<SeatDto>>(content);
                    
                    if (seats != null)
                    {
                        // SeatNumber-ээс Row ба Column талбаруудыг бөглөх
                        foreach (var seat in seats)
                        {
                            ParseSeatNumber(seat);
                        }
                    }
                    
                    return seats ?? new List<SeatDto>();
                }
                
                // API-ээс өгөгдөл авч чадаагүй үед
                Console.WriteLine($"Failed to get available seats: {response.StatusCode}");
                return new List<SeatDto>(); // Хоосон жагсаалт буцаана
            }
            catch (Exception ex)
            {
                // Алдаа гарсан үед
                Console.WriteLine($"Error getting available seats: {ex.Message}");
                return new List<SeatDto>(); // Хоосон жагсаалт буцаана
            }
        }
        
        public async Task<List<SeatDto>> GetAllSeatsForFlightAsync(int flightId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Boarding/seats/{flightId}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var seats = JsonConvert.DeserializeObject<List<SeatDto>>(content);
                    
                    if (seats != null)
                    {
                        // SeatNumber-ээс Row ба Column талбаруудыг бөглөх
                        foreach (var seat in seats)
                        {
                            ParseSeatNumber(seat);
                        }
                    }
                    
                    return seats ?? new List<SeatDto>();
                }
                
                // API-ээс өгөгдөл авч чадаагүй үед
                Console.WriteLine($"Failed to get all seats for flight: {response.StatusCode}");
                return new List<SeatDto>(); // Хоосон жагсаалт буцаана
            }
            catch (Exception ex)
            {
                // Алдаа гарсан үед
                Console.WriteLine($"Error getting all seats for flight: {ex.Message}");
                return new List<SeatDto>(); // Хоосон жагсаалт буцаана
            }
        }
        
        /// <summary>
        /// Суудлын дугаараас мөр ба баганыг задлах
        /// Жишээ нь: "1A" -> Row=1, Column="A"
        /// </summary>
        /// <param name="seat">Суудлын мэдээлэл</param>
        // private void ParseSeatNumber(SeatDto seat)
        // {
        //     if (string.IsNullOrEmpty(seat.SeatNumber))
        //         return;
            
        //     // Тоо ба үсгийг ангилах
        //     string rowString = "";
        //     string colString = "";
            
        //     foreach (char c in seat.SeatNumber)
        //     {
        //         if (char.IsDigit(c))
        //             rowString += c;
        //         else
        //             colString += c;
        //     }
            
        //     // Row талбарт тоон утга оноох
        //     if (int.TryParse(rowString, out int rowNumber))
        //         seat.Row = rowNumber;
        //     else
        //         seat.Row = 0; // Дараах үнэ
            
        //     // Column талбарт үсгийг оноох
        //     seat.Column = colString;
        // }
        
        private void ParseSeatNumber(SeatDto seat)
{
    if (!string.IsNullOrEmpty(seat.SeatNumber) && seat.SeatNumber.Length >= 2)
    {
        var rowPart = seat.SeatNumber.Substring(0, seat.SeatNumber.Length - 1);
        var columnPart = seat.SeatNumber[^1].ToString();
        if (int.TryParse(rowPart, out int row))
        {
            seat.Row = row;
            seat.Column = columnPart;
        }
    }
}

        public async Task<List<SeatDto>> GetAllSeatsAsync(int flightId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Boarding/seats/{flightId}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var seats = JsonConvert.DeserializeObject<List<SeatDto>>(content);
                    
                    if (seats != null)
                    {
                        // SeatNumber-ээс Row ба Column талбаруудыг бөглөх
                        foreach (var seat in seats)
                        {
                            ParseSeatNumber(seat);
                        }
                    }
                    
                    return seats ?? new List<SeatDto>();
                }
                
                // API-ээс өгөгдөл авч чадаагүй үед
                Console.WriteLine($"Failed to get all seats: {response.StatusCode}");
                return new List<SeatDto>(); // Хоосон жагсаалт буцаана
            }
            catch (Exception ex)
            {
                // Алдаа гарсан үед
                Console.WriteLine($"Error getting all seats: {ex.Message}");
                return new List<SeatDto>(); // Хоосон жагсаалт буцаана
            }
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