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
        private readonly IRepository<Passenger> _passengerRepository;
        private readonly IRepository<Flight> _flightRepository;
        private readonly IRepository<Seat> _seatRepository;
        private readonly IRepository<BoardingPass> _boardingPassRepository;
        private readonly IRepository<FlightPassenger> _flightPassengerRepository;
        private readonly INotificationService _notificationService;

        public BoardingService(
            IRepository<Passenger> passengerRepository,
            IRepository<Flight> flightRepository,
            IRepository<Seat> seatRepository,
            IRepository<BoardingPass> boardingPassRepository,
            IRepository<FlightPassenger> flightPassengerRepository,
            INotificationService notificationService)
        {
            _passengerRepository = passengerRepository;
            _flightRepository = flightRepository;
            _seatRepository = seatRepository;
            _boardingPassRepository = boardingPassRepository;
            _flightPassengerRepository = flightPassengerRepository;
            _notificationService = notificationService;
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

            // Try to assign the seat with retry mechanism
            bool seatAssigned = await AssignSeatToPassengerAsync(flightId, passenger.Id, seatNumber);
            if (!seatAssigned)
                throw new InvalidOperationException($"{seatNumber} суудал өөр зорчигчид хуваарилагдсан байна.");

            var boardingPass = new BoardingPass
            {
                PassengerId = passenger.Id,
                FlightId = flightId,
                SeatId = seat.Id,
                CheckInTime = DateTime.Now
            };

            await _boardingPassRepository.AddAsync(boardingPass);
            await _boardingPassRepository.SaveChangesAsync();

            // Notify about the boarding pass
            await _notificationService.NotifyBoardingPassIssuedAsync(boardingPass);

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
            const int maxRetries = 3;
            int retryDelay = 100; // Initial delay in milliseconds

            for (int attempt = 0; attempt < maxRetries; attempt++)
            {
                try
                {
                    var flightPassengers = await _flightPassengerRepository.FindAsync(fp => 
                        fp.FlightId == flightId && fp.PassengerId == passengerId);
                    
                    if (!flightPassengers.Any())
                        throw new InvalidOperationException($"{passengerId} дугаартай зорчигч {flightId} дугаартай нислэгт бүртгэлгүй байна.");
                        
                    var seats = await _seatRepository.FindAsync(s => 
                        s.FlightId == flightId && s.SeatNumber == seatNumber);
                    var seat = seats.FirstOrDefault();
                    
                    if (seat == null)
                        throw new KeyNotFoundException($"{flightId} дугаартай нислэгт {seatNumber} суудал олдсонгүй.");
                        
                    if (seat.IsOccupied)
                        throw new InvalidOperationException($"{seatNumber} суудал аль хэдийн эзэмшигдсэн байна.");
                    
                    seat.IsOccupied = true;
                    await _seatRepository.UpdateAsync(seat);
                    await _seatRepository.SaveChangesAsync();
                    
                    // Notify about seat assignment
                    await _notificationService.NotifySeatAssignedAsync(flightId, seatNumber, passengerId);
                    
                    return true;
                }
                catch (Exception ex) when (ex.InnerException?.Message.Contains("database is locked") == true)
                {
                    if (attempt < maxRetries - 1)
                    {
                        await Task.Delay(retryDelay);
                        retryDelay *= 2;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            
            return false;
        }
    }
} 