using DataAccess.Models;
using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class PassengerService : IPassengerService
    {
        private readonly IRepository<Passenger> _passengerRepository;
        private readonly IRepository<FlightPassenger> _flightPassengerRepository;

        public PassengerService(IRepository<Passenger> passengerRepository, IRepository<FlightPassenger> flightPassengerRepository)
        {
            _passengerRepository = passengerRepository;
            _flightPassengerRepository = flightPassengerRepository;
        }

        public async Task<IEnumerable<Passenger>> GetAllPassengersAsync()
        {
            return await _passengerRepository.GetAllAsync();
        }

        public async Task<Passenger?> GetPassengerByIdAsync(int id)
        {
            return await _passengerRepository.GetByIdAsync(id);
        }

        public async Task<Passenger?> GetPassengerByPassportNumberAsync(string passportNumber)
        {
            var passengers = await _passengerRepository.FindAsync(p => p.PassportNumber == passportNumber);
            return passengers.FirstOrDefault();
        }

        public async Task AddPassengerAsync(Passenger passenger)
        {
            if (passenger == null)
                throw new ArgumentNullException(nameof(passenger));

            await _passengerRepository.AddAsync(passenger);
            await _passengerRepository.SaveChangesAsync();
        }

        public async Task UpdatePassengerAsync(Passenger passenger)
        {
            if (passenger == null)
                throw new ArgumentNullException(nameof(passenger));

            await _passengerRepository.UpdateAsync(passenger);
            await _passengerRepository.SaveChangesAsync();
        }

        public async Task<bool> PassengerExistsAsync(int passengerId)
        {
            var passenger = await _passengerRepository.GetByIdAsync(passengerId);
            return passenger != null;
        }
        
        public async Task<IEnumerable<Passenger>> GetPassengersByFlightIdAsync(int flightId)
        {
            // Тухайн нислэгт бүртгэлтэй зорчигчдын холбоосыг авах
            var flightPassengers = await _flightPassengerRepository.FindAsync(fp => fp.FlightId == flightId);
            if (!flightPassengers.Any())
            {
                throw new KeyNotFoundException($"{flightId} дугаартай нислэгт бүртгэлтэй зорчигч олдсонгүй.");
            }
            
            // Зорчигчдын ID-г цуглуулах
            var passengerIds = flightPassengers.Select(fp => fp.PassengerId).ToList();
            
            // Зорчигчдын мэдээллийг авах
            var result = new List<Passenger>();
            foreach (var id in passengerIds)
            {
                var passenger = await _passengerRepository.GetByIdAsync(id);
                if (passenger != null)
                {
                    result.Add(passenger);
                }
            }
            
            return result;
        }
    }
} 