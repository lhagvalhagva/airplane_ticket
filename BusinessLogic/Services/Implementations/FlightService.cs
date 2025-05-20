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
        private readonly IRepository<FlightPassenger> _flightPassengerRepository;
        private readonly IRepository<Seat> _seatRepository;
        private readonly INotificationService _notificationService;

        public FlightService(
            IRepository<Flight> flightRepository,
            IRepository<FlightPassenger> flightPassengerRepository,
            IRepository<Seat> seatRepository,
            INotificationService notificationService)
        {
            _flightRepository = flightRepository;
            _flightPassengerRepository = flightPassengerRepository;
            _seatRepository = seatRepository;
            _notificationService = notificationService;
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
            
            await _notificationService.NotifyFlightStatusChangedAsync(flightId, status);
        }

        public async Task<bool> FlightExistsAsync(int flightId)
        {
            var flight = await _flightRepository.GetByIdAsync(flightId);
            return flight != null;
        }

        public async Task UpdateFlightAsync(Flight flight)
        {
            if (flight == null)
                throw new ArgumentNullException(nameof(flight));

            var existingFlight = await _flightRepository.GetByIdAsync(flight.Id);
            if (existingFlight == null)
                throw new KeyNotFoundException($"Flight with ID {flight.Id} not found.");

            await _flightRepository.UpdateAsync(flight);
            await _flightRepository.SaveChangesAsync();
        }

        public async Task DeleteFlightAsync(int id)
        {
            var flight = await _flightRepository.GetByIdAsync(id);
            if (flight == null)
                throw new KeyNotFoundException($"Flight with ID {id} not found.");

            // Хамааралтай зорчигчид болон суудлууд байгаа эсэхийг шалгах
            var flightPassengers = await _flightPassengerRepository.FindAsync(fp => fp.FlightId == id);
            if (flightPassengers.Any())
                throw new InvalidOperationException($"Cannot delete flight with ID {id} because it has registered passengers.");

            var seats = await _seatRepository.FindAsync(s => s.FlightId == id);
            if (seats.Any())
                throw new InvalidOperationException($"Cannot delete flight with ID {id} because it has assigned seats.");

            await _flightRepository.DeleteAsync(flight);
            // SaveChangesAsync автоматаар DeleteAsync дотор ажиллаж байгаа тул дахин дуудах шаардлагагүй
        }
    }
}