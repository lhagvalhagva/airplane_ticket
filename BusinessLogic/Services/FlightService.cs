using DataAccess.Models;
using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class FlightService : IFlightService
    {
        private readonly IRepository<Flight> _flightRepository;

        public FlightService(IRepository<Flight> flightRepository)
        {
            _flightRepository = flightRepository;
        }

        public async Task<IEnumerable<Flight>> GetAllFlightsAsync()
        {
            return await _flightRepository.GetAllAsync();
        }

        public async Task<Flight?> GetFlightByIdAsync(int id)
        {
            return await _flightRepository.GetByIdAsync(id);
        }

        public async Task<Flight?> GetFlightByNumberAsync(string flightNumber)
        {
            var flights = await _flightRepository.FindAsync(f => f.FlightNumber == flightNumber);
            return flights.FirstOrDefault();
        }

        public async Task AddFlightAsync(Flight flight)
        {
            if (flight == null)
                throw new ArgumentNullException(nameof(flight));

            await _flightRepository.AddAsync(flight);
            await _flightRepository.SaveChangesAsync();
        }

        public async Task UpdateFlightStatusAsync(int flightId, FlightStatus status)
        {
            var flight = await _flightRepository.GetByIdAsync(flightId);
            if (flight == null)
                throw new KeyNotFoundException($"Flight with ID {flightId} not found.");

            flight.Status = status;
            await _flightRepository.UpdateAsync(flight);
            await _flightRepository.SaveChangesAsync();
        }

        public async Task<bool> FlightExistsAsync(int flightId)
        {
            var flight = await _flightRepository.GetByIdAsync(flightId);
            return flight != null;
        }
    }
} 