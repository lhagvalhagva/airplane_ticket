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

        public async Task<BoardingPass> CreateBoardingPassAsync(BoardingPass boardingPass)
        {
            // Шалгалт хийх
            var validation = await ValidateBoardingPassDataAsync(
                boardingPass.FlightId, 
                boardingPass.PassengerId, 
                boardingPass.SeatId);

            if (!validation.IsValid)
                throw new InvalidOperationException(validation.ErrorMessage);

            // Хэрэглэгч нь тасалбарын авсан цагийг дамжуулахгүй бол одоогийн цагийг ашиглах
            if (boardingPass.CheckInTime == default)
                boardingPass.CheckInTime = DateTime.UtcNow;

            // Суудлыг захиалж тэмдэглэх
            var seat = await _seatRepository.GetByIdAsync(boardingPass.SeatId);
            if (seat != null)
            {
                seat.IsOccupied = true;
                await _seatRepository.UpdateAsync(seat);
            }

            // Зорчигчийг бүртгэгдсэн гэж тэмдэглэх
            var passenger = await _passengerRepository.GetByIdAsync(boardingPass.PassengerId);
            if (passenger != null)
            {
                // CheckedIn property байвал шинэчлэх
                if (passenger.GetType().GetProperty("CheckedIn") != null)
                {
                    passenger.GetType().GetProperty("CheckedIn").SetValue(passenger, true);
                    await _passengerRepository.UpdateAsync(passenger);
                }
            }

            // Тасалбар үүсгэх
            await _boardingPassRepository.AddAsync(boardingPass);
            await _boardingPassRepository.SaveChangesAsync();

            // Мэдэгдэл илгээх
            await _notificationService.NotifyBoardingPassIssuedAsync(boardingPass);

            return boardingPass;
        }

        public async Task DeleteBoardingPassAsync(int id)
        {
            var boardingPass = await _boardingPassRepository.GetByIdAsync(id);
            if (boardingPass == null)
                throw new KeyNotFoundException($"Тасалбар ID {id} олдсонгүй.");

            // Суудлыг чөлөөлөх
            var seat = await _seatRepository.GetByIdAsync(boardingPass.SeatId);
            if (seat != null)
            {
                seat.IsOccupied = false;
                await _seatRepository.UpdateAsync(seat);
                await _seatRepository.SaveChangesAsync();
            }

            // Тасалбар устгах
            await _boardingPassRepository.DeleteAsync(boardingPass);
        }

        public async Task<BoardingPass?> GetBoardingPassByIdAsync(int id)
        {
            return await _boardingPassRepository.GetByIdAsync(id);
        }

        public async Task<BoardingPass?> GetBoardingPassByFlightAndPassengerAsync(int flightId, int passengerId)
        {
            var boardingPasses = await _boardingPassRepository.FindAsync(
                bp => bp.FlightId == flightId && bp.PassengerId == passengerId);
            return boardingPasses.FirstOrDefault();
        }

        public async Task<IEnumerable<BoardingPass>> GetBoardingPassesByFlightAsync(int flightId)
        {
            return await _boardingPassRepository.FindAsync(bp => bp.FlightId == flightId);
        }

        public async Task<int?> GetSeatIdByFlightAndNumberAsync(int flightId, string seatNumber)
        {
            var seats = await _seatRepository.FindAsync(s => 
                s.FlightId == flightId && 
                s.SeatNumber == seatNumber);
                
            var seat = seats.FirstOrDefault();
            if (seat == null)
                return null;
                
            return seat.Id;
        }

        public async Task<BoardingPass> CreateBoardingPassWithSeatInfoAsync(int flightId, int passengerId, string seatNumber)
        {
            // Суудлын дугаараар суудлын ID-г олох
            var seatId = await GetSeatIdByFlightAndNumberAsync(flightId, seatNumber);
            if (!seatId.HasValue)
                throw new KeyNotFoundException($"Нислэг ID {flightId} дээр '{seatNumber}' дугаартай суудал олдсонгүй.");

            // Суудал захиалах боломжтой эсэхийг шалгах
            var validation = await ValidateBoardingPassDataAsync(flightId, passengerId, seatId.Value);
            if (!validation.IsValid)
                throw new InvalidOperationException(validation.ErrorMessage);

            // Тасалбар үүсгэх
            var boardingPass = new BoardingPass
            {
                FlightId = flightId,
                PassengerId = passengerId,
                SeatId = seatId.Value,
                CheckInTime = DateTime.UtcNow
            };

            return await CreateBoardingPassAsync(boardingPass);
        }

        public async Task<(bool IsValid, string? ErrorMessage)> ValidateBoardingPassDataAsync(int flightId, int passengerId, int seatId)
        {
            // Нислэг байгаа эсэхийг шалгах
            var flight = await _flightRepository.GetByIdAsync(flightId);
            if (flight == null)
                return (false, $"Нислэг ID {flightId} олдсонгүй.");

            // Зорчигч байгаа эсэхийг шалгах
            var passenger = await _passengerRepository.GetByIdAsync(passengerId);
            if (passenger == null)
                return (false, $"Зорчигч ID {passengerId} олдсонгүй.");

            // Зорчигч тухайн нислэгт бүртгэлтэй эсэхийг шалгах
            var flightPassengers = await _flightPassengerRepository.FindAsync(
                fp => fp.FlightId == flightId && fp.PassengerId == passengerId);
            if (!flightPassengers.Any())
                return (false, $"Зорчигч ID {passengerId} нь нислэг ID {flightId} дээр бүртгэлгүй байна.");

            // Суудал байгаа эсэхийг шалгах
            var seat = await _seatRepository.GetByIdAsync(seatId);
            if (seat == null)
                return (false, $"Суудал ID {seatId} олдсонгүй.");

            // Суудал тухайн нислэгт хамаарах эсэхийг шалгах
            if (seat.FlightId != flightId)
                return (false, $"Суудал ID {seatId} нь нислэг ID {flightId} дээр хамаарахгүй байна.");

            // Суудал захиалагдаагүй эсэхийг шалгах
            if (seat.IsOccupied)
                return (false, $"Суудал {seat.SeatNumber} аль хэдийн захиалагдсан байна.");

            // Зорчигч энэ нислэгт тасалбар авсан эсэхийг шалгах
            var existingBoardingPass = await GetBoardingPassByFlightAndPassengerAsync(flightId, passengerId);
            if (existingBoardingPass != null)
                return (false, $"Зорчигч ID {passengerId} нь энэ нислэгт аль хэдийн тасалбар авсан байна.");

            // Бүх шалгалт үүссэн
            return (true, null);
        }
    }
}