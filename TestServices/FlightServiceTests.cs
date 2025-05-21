using DataAccess.Models;
using DataAccess.Repositories;
using BusinessLogic.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TestServices
{
    [TestClass]
    public class FlightServiceTests
    {
        private Mock<IRepository<Flight>> _mockFlightRepository;
        private Mock<IRepository<FlightPassenger>> _mockFlightPassengerRepository;
        private Mock<IRepository<Seat>> _mockSeatRepository;
        private Mock<INotificationService> _mockNotificationService;
        private FlightService _flightService;

        [TestInitialize]
        public void Initialize()
        {
            _mockFlightRepository = new Mock<IRepository<Flight>>();
            _mockFlightPassengerRepository = new Mock<IRepository<FlightPassenger>>();
            _mockSeatRepository = new Mock<IRepository<Seat>>();
            _mockNotificationService = new Mock<INotificationService>();
            
            _flightService = new FlightService(
                _mockFlightRepository.Object,
                _mockFlightPassengerRepository.Object,
                _mockSeatRepository.Object,
                _mockNotificationService.Object);
        }

        #region BuhNislegAvah Tests

        [TestMethod]
        public async Task BuhNislegAvah_ShvvltGvi()
        {
            // Arrange
            var testFlights = new List<Flight>
            {
                new Flight { Id = 1, FlightNumber = "MGL123", DepartureCity = "Улаанбаатар", ArrivalCity = "Москва" },
                new Flight { Id = 2, FlightNumber = "MGL456", DepartureCity = "Пекин", ArrivalCity = "Улаанбаатар" }
            };

            _mockFlightRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(testFlights);

            // Act
            var result = await _flightService.GetAllFlightsAsync();
            var resultList = result.ToList();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, resultList.Count);
            Assert.AreEqual(1, resultList[0].Id);
            Assert.AreEqual("MGL123", resultList[0].FlightNumber);
            Assert.AreEqual(2, resultList[1].Id);
            Assert.AreEqual("MGL456", resultList[1].FlightNumber);
            _mockFlightRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [TestMethod]
        public async Task BuhNislegAvah_HoogdsonList_ReturnsEmptyEnumerable()
        {
            // Arrange
            _mockFlightRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<Flight>());

            // Act
            var result = await _flightService.GetAllFlightsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
            _mockFlightRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        #endregion

        #region NislegiigIdgeerAvah Tests

        [TestMethod]
        public async Task NislegiigIdgeerAvah_BaigaaId()
        {
            // Arrange
            int testId = 1;
            var testFlight = new Flight 
            { 
                Id = testId, 
                FlightNumber = "MGL123", 
                DepartureCity = "Улаанбаатар", 
                ArrivalCity = "Москва",
                DepartureTime = DateTime.Now.AddHours(2),
                ArrivalTime = DateTime.Now.AddHours(6),
                Status = FlightStatus.CheckingIn
            };

            _mockFlightRepository.Setup(repo => repo.GetByIdAsync(testId))
                .ReturnsAsync(testFlight);

            // Act
            var result = await _flightService.GetFlightByIdAsync(testId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(testId, result.Id);
            Assert.AreEqual("MGL123", result.FlightNumber);
            Assert.AreEqual("Улаанбаатар", result.DepartureCity);
            Assert.AreEqual("Москва", result.ArrivalCity);
            Assert.AreEqual(FlightStatus.CheckingIn, result.Status);
            _mockFlightRepository.Verify(repo => repo.GetByIdAsync(testId), Times.Once);
        }

        [TestMethod]
        public async Task NislegiigIdgeerAvah_BaihgviId()
        {
            // Arrange
            int testId = 999;
            _mockFlightRepository.Setup(repo => repo.GetByIdAsync(testId))
                .ReturnsAsync((Flight)null);

            // Act
            var result = await _flightService.GetFlightByIdAsync(testId);

            // Assert
            Assert.IsNull(result);
            _mockFlightRepository.Verify(repo => repo.GetByIdAsync(testId), Times.Once);
        }

        #endregion

        #region NislegiigDugaaraarAvah Tests

        [TestMethod]
        public async Task NislegiigDugaaraarAvah_BaigaaDugaar()
        {
            // Arrange
            string flightNumber = "MGL123";
            var testFlight = new Flight 
            { 
                Id = 1, 
                FlightNumber = flightNumber, 
                DepartureCity = "Улаанбаатар", 
                ArrivalCity = "Москва",
                Status = FlightStatus.CheckingIn
            };

            _mockFlightRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Flight, bool>>>()))
                .ReturnsAsync(new List<Flight> { testFlight });

            // Act
            var result = await _flightService.GetFlightByNumberAsync(flightNumber);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual(flightNumber, result.FlightNumber);
            _mockFlightRepository.Verify(repo => repo.FindAsync(It.IsAny<Expression<Func<Flight, bool>>>()), Times.Once);
        }

        [TestMethod]
        public async Task NislegiigDugaaraarAvah_BaihgviDugaar()
        {
            // Arrange
            string flightNumber = "NONEXIST";
            _mockFlightRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Flight, bool>>>()))
                .ReturnsAsync(new List<Flight>());

            // Act
            var result = await _flightService.GetFlightByNumberAsync(flightNumber);

            // Assert
            Assert.IsNull(result);
            _mockFlightRepository.Verify(repo => repo.FindAsync(It.IsAny<Expression<Func<Flight, bool>>>()), Times.Once);
        }

        #endregion

        #region NislegNemeh Tests

        [TestMethod]
        public async Task NislegNemeh_ZovUtga()
        {
            // Arrange
            var flight = new Flight
            {
                FlightNumber = "MGL789",
                DepartureCity = "Улаанбаатар",
                ArrivalCity = "Токио",
                DepartureTime = DateTime.Now.AddDays(1),
                ArrivalTime = DateTime.Now.AddDays(1).AddHours(4),
                Status = FlightStatus.CheckingIn
            };

            _mockFlightRepository.Setup(repo => repo.AddAsync(It.IsAny<Flight>()))
                .Returns(Task.CompletedTask);
            _mockFlightRepository.Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _flightService.AddFlightAsync(flight);

            // Assert
            _mockFlightRepository.Verify(repo => repo.AddAsync(flight), Times.Once);
            _mockFlightRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task NislegNemeh_KhoosoonUtga_AldaaNv()
        {
            // Act
            await _flightService.AddFlightAsync(null);

            // Assert is handled by ExpectedException attribute
        }

        #endregion

        #region NislegBaigaaEsehShalgah Tests

        [TestMethod]
        public async Task NislegBaigaaEsehShalgah_BaigaaId_Vnen()
        {
            // Arrange
            int flightId = 1;
            var testFlight = new Flight { Id = flightId, FlightNumber = "MGL123" };

            _mockFlightRepository.Setup(repo => repo.GetByIdAsync(flightId))
                .ReturnsAsync(testFlight);

            // Act
            var result = await _flightService.FlightExistsAsync(flightId);

            // Assert
            Assert.IsTrue(result);
            _mockFlightRepository.Verify(repo => repo.GetByIdAsync(flightId), Times.Once);
        }

        [TestMethod]
        public async Task NislegBaigaaEsehShalgah_BaihgviId_Khudal()
        {
            // Arrange
            int flightId = 999;
            _mockFlightRepository.Setup(repo => repo.GetByIdAsync(flightId))
                .ReturnsAsync((Flight)null);

            // Act
            var result = await _flightService.FlightExistsAsync(flightId);

            // Assert
            Assert.IsFalse(result);
            _mockFlightRepository.Verify(repo => repo.GetByIdAsync(flightId), Times.Once);
        }

        #endregion

        #region NislegShinchleh Tests

        [TestMethod]
        public async Task NislegShinchleh_ZovUtga()
        {
            // Arrange
            int flightId = 1;
            var existingFlight = new Flight
            {
                Id = flightId,
                FlightNumber = "MGL123",
                DepartureCity = "Улаанбаатар",
                ArrivalCity = "Москва",
                Status = FlightStatus.CheckingIn
            };

            var updatedFlight = new Flight
            {
                Id = flightId,
                FlightNumber = "MGL123",
                DepartureCity = "Улаанбаатар",
                ArrivalCity = "Москва",
                Status = FlightStatus.Boarding // Status changed
            };

            _mockFlightRepository.Setup(repo => repo.GetByIdAsync(flightId))
                .ReturnsAsync(existingFlight);
            _mockFlightRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Flight>()))
                .Returns(Task.CompletedTask);
            _mockFlightRepository.Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);
            _mockNotificationService.Setup(service => 
                service.NotifyFlightStatusChangedAsync(flightId, FlightStatus.Boarding))
                .Returns(Task.CompletedTask);

            // Act
            await _flightService.UpdateFlightAsync(updatedFlight);

            // Assert
            _mockFlightRepository.Verify(repo => repo.GetByIdAsync(flightId), Times.Once);
            _mockFlightRepository.Verify(repo => repo.UpdateAsync(updatedFlight), Times.Once);
            _mockFlightRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            _mockNotificationService.Verify(service => 
                service.NotifyFlightStatusChangedAsync(flightId, FlightStatus.Boarding), Times.Once);
        }

        [TestMethod]
        public async Task NislegiinTutviigShinchleh_MedegdelIlgeeh()
        {
            // Arrange
            int flightId = 1;
            var existingFlight = new Flight
            {
                Id = flightId,
                FlightNumber = "MGL123",
                DepartureCity = "Улаанбаатар",
                ArrivalCity = "Москва",
                Status = FlightStatus.CheckingIn
            };

            var updatedFlight = new Flight
            {
                Id = flightId,
                FlightNumber = "MGL123",
                DepartureCity = "Дархан", // Changed departure city
                ArrivalCity = "Москва",
                Status = FlightStatus.CheckingIn // Status not changed
            };

            _mockFlightRepository.Setup(repo => repo.GetByIdAsync(flightId))
                .ReturnsAsync(existingFlight);
            _mockFlightRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Flight>()))
                .Returns(Task.CompletedTask);
            _mockFlightRepository.Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _flightService.UpdateFlightAsync(updatedFlight);

            // Assert
            _mockFlightRepository.Verify(repo => repo.GetByIdAsync(flightId), Times.Once);
            _mockFlightRepository.Verify(repo => repo.UpdateAsync(updatedFlight), Times.Once);
            _mockFlightRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            _mockNotificationService.Verify(service => 
                service.NotifyFlightStatusChangedAsync(It.IsAny<int>(), It.IsAny<FlightStatus>()), Times.Never);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task NislegShinchleh_KhoosoonUtga_AldaaNv()
        {
            // Act
            await _flightService.UpdateFlightAsync(null);

            // Assert is handled by ExpectedException attribute
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task NislegShinchleh_BaihgviId_ThrowsKeyNotFoundException()
        {
            // Arrange
            int flightId = 999;
            var flight = new Flight
            {
                Id = flightId,
                FlightNumber = "MGL999",
                DepartureCity = "Unknown",
                ArrivalCity = "Unknown"
            };

            _mockFlightRepository.Setup(repo => repo.GetByIdAsync(flightId))
                .ReturnsAsync((Flight)null);

            // Act
            await _flightService.UpdateFlightAsync(flight);

            // Assert is handled by ExpectedException attribute
        }

        #endregion

        #region NislegUstgah Tests

        [TestMethod]
        public async Task NislegUstgah_ZovId()
        {
            // Arrange
            int flightId = 1;
            var flight = new Flight { Id = flightId, FlightNumber = "MGL123" };

            _mockFlightRepository.Setup(repo => repo.GetByIdAsync(flightId))
                .ReturnsAsync(flight);
            _mockFlightPassengerRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<FlightPassenger, bool>>>()))
                .ReturnsAsync(new List<FlightPassenger>());
            _mockSeatRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Seat, bool>>>()))
                .ReturnsAsync(new List<Seat>());
            _mockFlightRepository.Setup(repo => repo.DeleteAsync(It.IsAny<Flight>()))
                .Returns(Task.CompletedTask);

            // Act
            await _flightService.DeleteFlightAsync(flightId);

            // Assert
            _mockFlightRepository.Verify(repo => repo.GetByIdAsync(flightId), Times.Once);
            _mockFlightPassengerRepository.Verify(repo => repo.FindAsync(It.IsAny<Expression<Func<FlightPassenger, bool>>>()), Times.Once);
            _mockSeatRepository.Verify(repo => repo.FindAsync(It.IsAny<Expression<Func<Seat, bool>>>()), Times.Once);
            _mockFlightRepository.Verify(repo => repo.DeleteAsync(flight), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task NislegUstgah_BaihgviId_AldaaNv()
        {
            // Arrange
            int flightId = 999;
            _mockFlightRepository.Setup(repo => repo.GetByIdAsync(flightId))
                .ReturnsAsync((Flight)null);

            // Act
            await _flightService.DeleteFlightAsync(flightId);

            // Assert is handled by ExpectedException attribute
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task NislegUstgah_ZorchigchtoiNisleg_AldaaNv()
        {
            // Arrange
            int flightId = 1;
            var flight = new Flight { Id = flightId, FlightNumber = "MGL123" };
            var flightPassengers = new List<FlightPassenger>
            {
                new FlightPassenger { FlightId = flightId, PassengerId = 100 }
            };

            _mockFlightRepository.Setup(repo => repo.GetByIdAsync(flightId))
                .ReturnsAsync(flight);
            _mockFlightPassengerRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<FlightPassenger, bool>>>()))
                .ReturnsAsync(flightPassengers);

            // Act
            await _flightService.DeleteFlightAsync(flightId);

            // Assert is handled by ExpectedException attribute
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task NislegUstgah_SuudaltoiNisleg_AldaaNv()
        {
            // Arrange
            int flightId = 1;
            var flight = new Flight { Id = flightId, FlightNumber = "MGL123" };
            var seats = new List<Seat>
            {
                new Seat { Id = 1, FlightId = flightId, SeatNumber = "1A" }
            };

            _mockFlightRepository.Setup(repo => repo.GetByIdAsync(flightId))
                .ReturnsAsync(flight);
            _mockFlightPassengerRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<FlightPassenger, bool>>>()))
                .ReturnsAsync(new List<FlightPassenger>());
            _mockSeatRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Seat, bool>>>()))
                .ReturnsAsync(seats);

            // Act
            await _flightService.DeleteFlightAsync(flightId);

            // Assert is handled by ExpectedException attribute
        }

        #endregion
    }
}
