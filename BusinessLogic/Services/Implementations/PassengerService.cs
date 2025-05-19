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

        public async Task<IEnumerable<Passenger>> GetAllPassengersAsync(string? nameFilter = null, string? passportNumber = null)
        {
            // Бүх зорчигчдыг авах
            var passengers = await _passengerRepository.GetAllAsync();
            
            // Нэр эсвэл паспортын дугаараар шүүлт хийх
            if (!string.IsNullOrWhiteSpace(nameFilter))
            {
                nameFilter = nameFilter.ToLower();
                passengers = passengers.Where(p =>
                    p.FirstName.ToLower().Contains(nameFilter) ||
                    p.LastName.ToLower().Contains(nameFilter));
            }
            
            if (!string.IsNullOrWhiteSpace(passportNumber))
            {
                passengers = passengers.Where(p => 
                    p.PassportNumber.Contains(passportNumber));
            }
            
            return passengers;
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

        public async Task DeletePassengerAsync(int id)
        {
            var passenger = await _passengerRepository.GetByIdAsync(id);
            if (passenger == null)
                throw new KeyNotFoundException($"Зорчигч ID {id} олдсонгүй.");

            var flightPassengers = await _flightPassengerRepository.FindAsync(fp => fp.PassengerId == id);
            if (flightPassengers.Any())
                throw new InvalidOperationException("Энэ зорчигч нислэгүүдэд бүртгэлтэй тул устгах боломжгүй.");

            await _passengerRepository.DeleteAsync(passenger);
            await _passengerRepository.SaveChangesAsync();
        }

        public async Task<bool> PassengerExistsAsync(int passengerId)
        {
            var passenger = await _passengerRepository.GetByIdAsync(passengerId);
            return passenger != null;
        }
    }
} 