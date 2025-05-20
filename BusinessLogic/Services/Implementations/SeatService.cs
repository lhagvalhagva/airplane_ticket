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
        private readonly IRepository<FlightPassenger> _flightPassengerRepository;
        private readonly INotificationService _notificationService;

        public SeatService( 
            IRepository<Seat> seatRepository,
            IRepository<Flight> flightRepository, 
            IRepository<Passenger> passengerRepository,
            IRepository<FlightPassenger> flightPassengerRepository,
            INotificationService notificationService)
        {
            _seatRepository = seatRepository;
            _flightRepository = flightRepository;
            _passengerRepository = passengerRepository;
            _flightPassengerRepository = flightPassengerRepository;
            _notificationService = notificationService;
        }

        public async Task<IEnumerable<Seat>> GetSeatsByFlightIdAsync(int flightId, bool? isOccupied = null)
        {
            var flight = await _flightRepository.GetByIdAsync(flightId);
            if (flight == null)
                throw new KeyNotFoundException($"Flight with ID {flightId} not found.");

            var seats = await _seatRepository.FindAsync(s => s.FlightId == flightId);

            if (isOccupied.HasValue)
            {
                seats = seats.Where(s => s.IsOccupied == isOccupied.Value);
            }

            return seats.ToList();
        }


        public async Task<Seat?> GetSeatByIdAsync(int seatId)
        {
            return await _seatRepository.GetByIdAsync(seatId);
        }

        public async Task<bool> AssignSeatAsync(int flightId, int passengerId, int seatId)
        {
            // Нислэгийн байгаа эсэхийг шалгах
            var flight = await _flightRepository.GetByIdAsync(flightId);
            if (flight == null)
                throw new KeyNotFoundException($"Нислэг ID {flightId} олдсонгүй.");

            // Зорчигчийн байгаа эсэхийг шалгах
            var passenger = await _passengerRepository.GetByIdAsync(passengerId);
            if (passenger == null)
                throw new KeyNotFoundException($"Зорчигч ID {passengerId} олдсонгүй.");

            // Зорчигч нислэгт бүртгэлтэй эсэхийг шалгах
            var flightPassenger = (await _flightPassengerRepository.FindAsync(
                fp => fp.FlightId == flightId && fp.PassengerId == passengerId)).FirstOrDefault();
                
            if (flightPassenger == null)
                throw new InvalidOperationException($"Зорчигч ID {passengerId} нь нислэг ID {flightId} дээр бүртгүүлээгүй байна.");

            // Суудлын байгаа эсэхийг шалгах
            var seat = await _seatRepository.GetByIdAsync(seatId);
            if (seat == null)
                throw new KeyNotFoundException($"Суудал ID {seatId} олдсонгүй.");
                
            // Суудал нислэгт хамаарах эсэхийг шалгах
            if (seat.FlightId != flightId)
                throw new InvalidOperationException($"Суудал ID {seatId} нь нислэг ID {flightId} дээр хамаарахгүй байна.");

            // Суудал захиалагдсан эсэхийг шалгах
            if (seat.IsOccupied)
                throw new InvalidOperationException($"Суудал {seat.SeatNumber} аль хэдийн захиалагдсан байна.");

            // Зорчигч өөр суудалд бүртгэгдсэн эсэхийг шалгах
            var existingSeats = await _seatRepository.FindAsync(
                s => s.FlightId == flightId && s.PassengerId == passengerId);
            
            if (existingSeats.Any())
                throw new InvalidOperationException($"Зорчигч ID {passengerId} нь нислэг ID {flightId} дээр өөр суудалд бүртгегдсэн байна.");

            // Суудлыг захиалах
            seat.IsOccupied = true;
            seat.PassengerId = passengerId;
            seat.CheckInTime = DateTime.UtcNow;

            await _seatRepository.UpdateAsync(seat);
            await _seatRepository.SaveChangesAsync();
            
            // Суудал оноогдсон тухай мэдэгдэл илгээх
            await _notificationService.NotifySeatAssignedAsync(flightId, seat.SeatNumber, passengerId);

            return true;
        }

        public async Task<bool> ReleaseSeatAsync(int flightId, int seatId)
        {
            // Нислэг, суудал бүгд байгаа эсэхийг шалгах
            var flight = await _flightRepository.GetByIdAsync(flightId);
            if (flight == null)
                throw new KeyNotFoundException($"Нислэг ID {flightId} олдсонгүй.");

            var seat = await _seatRepository.GetByIdAsync(seatId);
            if (seat == null)
                throw new KeyNotFoundException($"Суудал ID {seatId} олдсонгүй.");

            // Суудал нислэгт хамаарах эсэхийг шалгах
            if (seat.FlightId != flightId)
                throw new InvalidOperationException($"Суудал ID {seatId} нь нислэг ID {flightId} дээр хамаарахгүй байна.");

            if (!seat.IsOccupied)
                return true; 

            int? oldPassengerId = seat.PassengerId;
            string seatNumber = seat.SeatNumber;

            seat.IsOccupied = false;
            seat.PassengerId = null;
            seat.CheckInTime = null;

            await _seatRepository.UpdateAsync(seat);
            await _seatRepository.SaveChangesAsync();
            
            // Хэрэв хуучин зорчигч мэдээлэл байвал мэдэгдлийг илгээх
            if (oldPassengerId.HasValue)
            {
                // Суудал чөлөөлсөн тухай мэдэгдлийг шууд илгээж болох боловч энд хийхгүй байна
                Console.WriteLine($"Суудал {seatNumber} нь нислэг ID {flightId} дээр чөлөөлөгдлөө");
            }

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
        
        /// <summary>
        /// Зорчигчийн суудлыг олох
        /// </summary>
        /// <param name="flightId">Нислэгийн ID</param>
        /// <param name="passengerId">Зорчигчийн ID</param>
        /// <returns>Зорчигчийн суудлын мэдээлэл, эсвэл null</returns>
        public async Task<Seat?> GetPassengerSeatAsync(int flightId, int passengerId)
        {
            var flight = await _flightRepository.GetByIdAsync(flightId);
            if (flight == null)
                throw new KeyNotFoundException($"Нислэг ID {flightId} олдсонгүй.");

            var passenger = await _passengerRepository.GetByIdAsync(passengerId);
            if (passenger == null)
                throw new KeyNotFoundException($"Зорчигч ID {passengerId} олдсонгүй.");

            var flightPassenger = (await _flightPassengerRepository.FindAsync(
                fp => fp.FlightId == flightId && fp.PassengerId == passengerId)).FirstOrDefault();
                
            if (flightPassenger == null)
                throw new InvalidOperationException($"Зорчигч ID {passengerId} нь нислэг ID {flightId} дээр бүртгүүлээгүй байна.");

            var seat = (await _seatRepository.FindAsync(
                s => s.FlightId == flightId && s.PassengerId == passengerId)).FirstOrDefault();
            
            return seat;
        }
    }
}
