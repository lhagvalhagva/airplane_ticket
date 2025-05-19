using DataAccess.Models;
using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class SeatService : ISeatService
    {
        private readonly IRepository<Seat> _seatRepository;
        private readonly IRepository<Flight> _flightRepository;
        private readonly IRepository<Passenger> _passengerRepository;
        private readonly IRepository<BoardingPass> _boardingPassRepository;

        public SeatService(
            IRepository<Seat> seatRepository,
            IRepository<Flight> flightRepository, 
            IRepository<Passenger> passengerRepository,
            IRepository<BoardingPass> boardingPassRepository)
        {
            _seatRepository = seatRepository;
            _flightRepository = flightRepository;
            _passengerRepository = passengerRepository;
            _boardingPassRepository = boardingPassRepository;
        }

        public async Task<IEnumerable<Seat>> GetSeatsByFlightIdAsync(int flightId, bool? isOccupied = null)
        {
            // Нислэг байгаа эсэхийг шалгах
            var flight = await _flightRepository.GetByIdAsync(flightId);
            if (flight == null)
                throw new KeyNotFoundException($"Flight with ID {flightId} not found.");

            // Нислэгийн бүх суудлуудыг авах
            var seats = await _seatRepository.FindAsync(s => s.FlightId == flightId);

            // Захиалгатай эсвэл захиалгагүй суудлуудаар шүүх шаардлагатай бол
            if (isOccupied.HasValue)
            {
                seats = seats.Where(s => s.IsOccupied == isOccupied.Value);
            }

            return seats.ToList();
        }

        public async Task<IEnumerable<Seat>> GetAvailableSeatsByFlightIdAsync(int flightId)
        {
            return await GetSeatsByFlightIdAsync(flightId, false);
        }

        public async Task<Seat?> GetSeatByIdAsync(int seatId)
        {
            return await _seatRepository.GetByIdAsync(seatId);
        }

        public async Task<bool> AssignSeatAsync(int flightId, int passengerId, int seatId)
        {
            // Нислэг, зорчигч, суудал бүгд байгаа эсэхийг шалгах
            var flight = await _flightRepository.GetByIdAsync(flightId);
            if (flight == null)
                throw new KeyNotFoundException($"Flight with ID {flightId} not found.");

            var passenger = await _passengerRepository.GetByIdAsync(passengerId);
            if (passenger == null)
                throw new KeyNotFoundException($"Passenger with ID {passengerId} not found.");

            var seat = await _seatRepository.GetByIdAsync(seatId);
            if (seat == null)
                throw new KeyNotFoundException($"Seat with ID {seatId} not found.");

            // Суудал нислэгт хамаарах эсэхийг шалгах
            if (seat.FlightId != flightId)
                throw new InvalidOperationException($"Seat {seatId} does not belong to flight {flightId}.");

            // Суудал захиалагдсан эсэхийг шалгах
            if (seat.IsOccupied)
                throw new InvalidOperationException($"Seat {seat.SeatNumber} is already occupied.");

            // Зорчигч өөр суудалд бүртгэлтэй байгаа эсэхийг шалгах
            var existingPass = await _boardingPassRepository.FindAsync(
                bp => bp.FlightId == flightId && bp.PassengerId == passengerId);
            
            if (existingPass.Any())
                throw new InvalidOperationException($"Passenger {passengerId} already has a seat assigned on flight {flightId}.");

            // Суудлыг захиалах
            seat.IsOccupied = true;
            await _seatRepository.UpdateAsync(seat);

            // Boarding pass үүсгэх
            var boardingPass = new BoardingPass
            {
                CheckInTime = DateTime.Now,
                FlightId = flightId,
                PassengerId = passengerId,
                SeatId = seatId
            };

            await _boardingPassRepository.AddAsync(boardingPass);
            await _boardingPassRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ReleaseSeatAsync(int flightId, int seatId)
        {
            // Нислэг, суудал бүгд байгаа эсэхийг шалгах
            var flight = await _flightRepository.GetByIdAsync(flightId);
            if (flight == null)
                throw new KeyNotFoundException($"Flight with ID {flightId} not found.");

            var seat = await _seatRepository.GetByIdAsync(seatId);
            if (seat == null)
                throw new KeyNotFoundException($"Seat with ID {seatId} not found.");

            // Суудал нислэгт хамаарах эсэхийг шалгах
            if (seat.FlightId != flightId)
                throw new InvalidOperationException($"Seat {seatId} does not belong to flight {flightId}.");

            // Суудал захиалагдсан эсэхийг шалгах
            if (!seat.IsOccupied)
                return true; // Суудал аль хэдийн чөлөөтэй бол зүгээр

            // Холбогдох boarding pass-ыг олох
            var boardingPasses = await _boardingPassRepository.FindAsync(
                bp => bp.FlightId == flightId && bp.SeatId == seatId);
            
            var boardingPass = boardingPasses.FirstOrDefault();

            // BoardingPass олдвол устгах
            if (boardingPass != null)
            {
                await _boardingPassRepository.DeleteAsync(boardingPass);
            }

            // Суудлыг чөлөөлөх
            seat.IsOccupied = false;
            await _seatRepository.UpdateAsync(seat);
            await _seatRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SeatExistsAsync(int seatId)
        {
            var seat = await _seatRepository.GetByIdAsync(seatId);
            return seat != null;
        }

        public async Task<bool> IsSeatAvailableAsync(int seatId)
        {
            var seat = await _seatRepository.GetByIdAsync(seatId);
            if (seat == null)
                throw new KeyNotFoundException($"Seat with ID {seatId} not found.");

            return !seat.IsOccupied;
        }
    }
}
