using DataAccess.Models;
using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class BoardingService : IBoardingService
    {
        private readonly IRepository<BoardingPass> _boardingPassRepository;
        private readonly IRepository<Flight> _flightRepository;
        private readonly IRepository<Passenger> _passengerRepository;
        private readonly IRepository<Seat> _seatRepository;
        private readonly IRepository<FlightPassenger> _flightPassengerRepository;

        public BoardingService(
            IRepository<BoardingPass> boardingPassRepository,
            IRepository<Flight> flightRepository,
            IRepository<Passenger> passengerRepository,
            IRepository<Seat> seatRepository,
            IRepository<FlightPassenger> flightPassengerRepository)
        {
            _boardingPassRepository = boardingPassRepository;
            _flightRepository = flightRepository;
            _passengerRepository = passengerRepository;
            _seatRepository = seatRepository;
            _flightPassengerRepository = flightPassengerRepository;
        }
        // C:\Users\User\Desktop\WINDOWS_PROJECT\RestApi\airport.db
        public async Task<BoardingPass> CheckInPassengerAsync(int flightId, string passportNumber, string seatNumber)
        {
            var passengers = await _passengerRepository.FindAsync(p => p.PassportNumber == passportNumber);
            var passenger = passengers.FirstOrDefault();

            if (passenger == null)
                throw new KeyNotFoundException($"{passportNumber} паспортын дугаартай зорчигч олдсонгүй.");

            var flight = await _flightRepository.GetByIdAsync(flightId);
            if (flight == null)
                throw new KeyNotFoundException($"{flightId} дугаартай нислэг олдсонгүй.");

            var flightPassengers = await _flightPassengerRepository.FindAsync(
                fp => fp.FlightId == flightId && fp.PassengerId == passenger.Id);

            if (!flightPassengers.Any())
                throw new InvalidOperationException($"{passportNumber} паспортын дугаартай зорчигч {flightId} дугаартай нислэгт захиалга хийгээгүй байна.");

            var seats = await _seatRepository.FindAsync(s => s.FlightId == flightId && s.SeatNumber == seatNumber);
            var seat = seats.FirstOrDefault();

            if (seat == null)
                throw new KeyNotFoundException($"{flightId} дугаартай нислэгт {seatNumber} суудал олдсонгүй.");

            if (seat.IsOccupied)
                throw new InvalidOperationException($"{seatNumber} суудал аль хэдийн захиалагдсан байна.");

            seat.IsOccupied = true;
            await _seatRepository.UpdateAsync(seat);

            var boardingPass = new BoardingPass
            {
                PassengerId = passenger.Id,
                FlightId = flightId,
                SeatId = seat.Id,
                CheckInTime = DateTime.Now
            };

            await _boardingPassRepository.AddAsync(boardingPass);
            await _boardingPassRepository.SaveChangesAsync();

            return boardingPass;
        }

        public async Task<IEnumerable<Seat>> GetAvailableSeatsAsync(int flightId)
        {
            var allSeats = await _seatRepository.FindAsync(s => s.FlightId == flightId);
            return allSeats.Where(s => !s.IsOccupied);
        }

        public async Task<BoardingPass?> GetBoardingPassAsync(int boardingPassId)
        {
            return await _boardingPassRepository.GetByIdAsync(boardingPassId);
        }

        public async Task<IEnumerable<BoardingPass>> GetBoardingPassesByFlightAsync(int flightId)
        {
            return await _boardingPassRepository.FindAsync(bp => bp.FlightId == flightId);
        }

        public async Task<bool> IsSeatAvailableAsync(int flightId, string seatNumber)
        {
            var seats = await _seatRepository.FindAsync(s => s.FlightId == flightId && s.SeatNumber == seatNumber);
            var seat = seats.FirstOrDefault();
            
            if (seat == null)
                return false;
                
            return !seat.IsOccupied;
        }

        /// <summary>
        /// Хэрэглэгчид суудал оноох
        /// </summary>
        public async Task<bool> AssignSeatToPassengerAsync(int flightId, int passengerId, string seatNumber)
        {
            // Retry logic for handling potential database locking
            int maxRetries = 3;
            int retryDelay = 500; // milliseconds
            
            // Нислэг байгаа эсэхийг шалгах
            var flight = await _flightRepository.GetByIdAsync(flightId);
            if (flight == null)
                throw new KeyNotFoundException($"{flightId} дугаартай нислэг олдсонгүй.");
                
            // Зорчигч байгаа эсэхийг шалгах
            var passenger = await _passengerRepository.GetByIdAsync(passengerId);
            if (passenger == null)
                throw new KeyNotFoundException($"{passengerId} дугаартай зорчигч олдсонгүй.");
            
            for (int attempt = 0; attempt < maxRetries; attempt++)
            {
                try
                {
                    // Нислэгийн зорчигчдод шалгах
                    var flightPassengers = await _flightPassengerRepository.FindAsync(fp => fp.FlightId == flightId && fp.PassengerId == passengerId);
                    
                    if (!flightPassengers.Any())
                        throw new InvalidOperationException($"{passengerId} дугаартай зорчигч {flightId} дугаартай нислэгт бүртгэлгүй байна.");
                        
                    // Суудал байгаа эсэхийг шалгах
                    var seats = await _seatRepository.FindAsync(s => s.FlightId == flightId && s.SeatNumber == seatNumber);
                    var seat = seats.FirstOrDefault();
                    
                    if (seat == null)
                        throw new KeyNotFoundException($"{flightId} дугаартай нислэгт {seatNumber} суудал олдсонгүй.");
                        
                    // Суудал сул эсэхийг шалгах
                    if (seat.IsOccupied)
                        throw new InvalidOperationException($"{seatNumber} суудал аль хэдийн эзэмшигдсэн байна.");
                    
                    // Суудлыг эзэмшүүлэх
                    seat.IsOccupied = true;
                    await _seatRepository.UpdateAsync(seat);
                    
                    // Өөрчлөлтүүдийг хадгалж, транзакцийг хаах - immediately execute SaveChanges
                    await _seatRepository.SaveChangesAsync();
                    
                    // If successful, exit the retry loop
                    return true;
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException ex) when (ex.InnerException is Microsoft.Data.Sqlite.SqliteException sqlEx && sqlEx.Message.Contains("database is locked"))
                {
                    // Only retry if this is specifically a database lock exception
                    if (attempt < maxRetries - 1)
                    {
                        Console.WriteLine($"Database locked, attempt {attempt + 1} of {maxRetries}. Waiting {retryDelay}ms before retry.");
                        await Task.Delay(retryDelay);
                        // Increase retry delay for next attempt (exponential backoff)
                        retryDelay *= 2;
                    }
                    else
                    {
                        // This was the last attempt, rethrow the exception
                        throw;
                    }
                }
            }
            
            // If we've exhausted all retries without success or exceptions
            return false;
        }
    }
} 