using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Models;
using DataAccess.Repositories;

namespace BusinessLogic.Services
{
    public class FlightPassengerService : IFlightPassengerService
    {
        private readonly IRepository<Flight> _flightRepository;
        private readonly IRepository<Passenger> _passengerRepository;
        private readonly IRepository<FlightPassenger> _flightPassengerRepository;
        private readonly INotificationService _notificationService;

        public FlightPassengerService(
            IRepository<Flight> flightRepository,
            IRepository<Passenger> passengerRepository,
            IRepository<FlightPassenger> flightPassengerRepository,
            INotificationService notificationService)
        {
            _flightRepository = flightRepository;
            _passengerRepository = passengerRepository;
            _flightPassengerRepository = flightPassengerRepository;
            _notificationService = notificationService;
        }

        public async Task<IEnumerable<Passenger>> GetPassengersByFlightIdAsync(int flightId)
        {
            // Нислэг байгаа эсэхийг шалгах
            var flight = await _flightRepository.GetByIdAsync(flightId);
            if (flight == null)
                throw new KeyNotFoundException($"Нислэг ID {flightId} олдсонгүй.");

            // Нислэгийн бүх зорчигчдыг олох
            var flightPassengers = await _flightPassengerRepository.FindAsync(fp => fp.FlightId == flightId);
            
            // Зорчигчдын мэдээллийг цуглуулах
            var passengers = new List<Passenger>();
            foreach (var fp in flightPassengers)
            {
                var passenger = await _passengerRepository.GetByIdAsync(fp.PassengerId);
                if (passenger != null)
                {
                    passengers.Add(passenger);
                }
            }

            return passengers;
        }

        public async Task<IEnumerable<Flight>> GetFlightsByPassengerIdAsync(int passengerId)
        {
            // Зорчигч байгаа эсэхийг шалгах
            var passenger = await _passengerRepository.GetByIdAsync(passengerId);
            if (passenger == null)
                throw new KeyNotFoundException($"Зорчигч ID {passengerId} олдсонгүй.");

            // Зорчигчийн бүх нислэгүүдийг олох
            var flightPassengers = await _flightPassengerRepository.FindAsync(fp => fp.PassengerId == passengerId);
            
            // Нислэгүүдийн мэдээллийг цуглуулах
            var flights = new List<Flight>();
            foreach (var fp in flightPassengers)
            {
                var flight = await _flightRepository.GetByIdAsync(fp.FlightId);
                if (flight != null)
                {
                    flights.Add(flight);
                }
            }

            return flights;
        }

        public async Task<FlightPassenger> RegisterPassengerToFlightAsync(int flightId, int passengerId)
        {
            // Нислэг байгаа эсэхийг шалгах
            var flight = await _flightRepository.GetByIdAsync(flightId);
            if (flight == null)
                throw new KeyNotFoundException($"Нислэг ID {flightId} олдсонгүй.");

            // Зорчигч байгаа эсэхийг шалгах
            var passenger = await _passengerRepository.GetByIdAsync(passengerId);
            if (passenger == null)
                throw new KeyNotFoundException($"Зорчигч ID {passengerId} олдсонгүй.");

            // Зорчигч аль хэдийн бүртгэлтэй эсэхийг шалгах
            var isRegistered = await PassengerIsRegisteredToFlightAsync(flightId, passengerId);
            if (isRegistered)
                throw new InvalidOperationException($"Зорчигч ID {passengerId} аль хэдийн нислэг ID {flightId} дээр бүртгэлтэй байна.");

            // Шинэ холболт үүсгэх
            var flightPassenger = new FlightPassenger
            {
                FlightId = flightId,
                PassengerId = passengerId,
                RegistrationDate = DateTime.UtcNow
            };

            await _flightPassengerRepository.AddAsync(flightPassenger);
            await _flightPassengerRepository.SaveChangesAsync();

            // Нислэгийн зорчигчид мэдэгдэл илгээх
            await _notificationService.NotifyPassengerRegisteredAsync(flightId, passengerId);

            return flightPassenger;
        }

        public async Task RemovePassengerFromFlightAsync(int id)
        {
            // FlightPassenger холболт байгаа эсэхийг шалгах
            var flightPassenger = await _flightPassengerRepository.GetByIdAsync(id);
            if (flightPassenger == null)
                throw new KeyNotFoundException($"FlightPassenger ID {id} олдсонгүй.");

            // Холболт устгах
            await _flightPassengerRepository.DeleteAsync(flightPassenger);

            // Нислэгийн зорчигчид мэдэгдэл илгээх
            await _notificationService.NotifyPassengerUnregisteredAsync(
                flightPassenger.FlightId, 
                flightPassenger.PassengerId);
        }

        public async Task RemovePassengerFromFlightAsync(int flightId, int passengerId)
        {
            // Зорчигч нислэгт бүртгэлтэй эсэхийг шалгах
            var flightPassengers = await _flightPassengerRepository.FindAsync(
                fp => fp.FlightId == flightId && fp.PassengerId == passengerId);
            
            var flightPassenger = flightPassengers.FirstOrDefault();
            if (flightPassenger == null)
                throw new KeyNotFoundException($"Зорчигч ID {passengerId} нислэг ID {flightId} дээр бүртгэлгүй байна.");

            // Холболт устгах
            await _flightPassengerRepository.DeleteAsync(flightPassenger);

            // Нислэгийн зорчигчид мэдэгдэл илгээх
            await _notificationService.NotifyPassengerUnregisteredAsync(flightId, passengerId);
        }

        public async Task<bool> FlightPassengerExistsAsync(int id)
        {
            var flightPassenger = await _flightPassengerRepository.GetByIdAsync(id);
            return flightPassenger != null;
        }

        public async Task<bool> PassengerIsRegisteredToFlightAsync(int flightId, int passengerId)
        {
            var flightPassengers = await _flightPassengerRepository.FindAsync(
                fp => fp.FlightId == flightId && fp.PassengerId == passengerId);
            
            return flightPassengers.Any();
        }
    }
}
