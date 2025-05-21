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
    public class FlightPassengerServiceTests
    {
        private Mock<IRepository<Flight>> _mockFlightRepository;
        private Mock<IRepository<Passenger>> _mockPassengerRepository;
        private Mock<IRepository<FlightPassenger>> _mockFlightPassengerRepository;
        private Mock<INotificationService> _mockNotificationService;
        private FlightPassengerService _flightPassengerService;

        [TestInitialize]
        public void Initialize()
        {
            _mockFlightRepository = new Mock<IRepository<Flight>>();
            _mockPassengerRepository = new Mock<IRepository<Passenger>>();
            _mockFlightPassengerRepository = new Mock<IRepository<FlightPassenger>>();
            _mockNotificationService = new Mock<INotificationService>();
            
            _flightPassengerService = new FlightPassengerService(
                _mockFlightRepository.Object,
                _mockPassengerRepository.Object,
                _mockFlightPassengerRepository.Object,
                _mockNotificationService.Object);
        }

        #region GetPassengersByFlightIdAsync Tests

        [TestMethod]
        public async Task NislegiinZorchigchdygiAvah_ZovNislegId()
        {
            // Arrange
            int flightId = 1;
            int passengerId1 = 10;
            int passengerId2 = 20;

            var flight = new Flight 
            { 
                Id = flightId, 
                FlightNumber = "MGL123",
                DepartureCity = "Улаанбаатар",
                ArrivalCity = "Москва"
            };

            var passenger1 = new Passenger 
            { 
                Id = passengerId1, 
                FirstName = "Болд", 
                LastName = "Баатар",
                PassportNumber = "AA123456"
            };

            var passenger2 = new Passenger 
            { 
                Id = passengerId2, 
                FirstName = "Хүсэл", 
                LastName = "Бат",
                PassportNumber = "BB123456"
            };

            var flightPassengers = new List<FlightPassenger>
            {
                new FlightPassenger { Id = 1, FlightId = flightId, PassengerId = passengerId1 },
                new FlightPassenger { Id = 2, FlightId = flightId, PassengerId = passengerId2 }
            };

            _mockFlightRepository.Setup(repo => repo.GetByIdAsync(flightId))
                .ReturnsAsync(flight);
            _mockFlightPassengerRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<FlightPassenger, bool>>>()))
                .ReturnsAsync(flightPassengers);
            _mockPassengerRepository.Setup(repo => repo.GetByIdAsync(passengerId1))
                .ReturnsAsync(passenger1);
            _mockPassengerRepository.Setup(repo => repo.GetByIdAsync(passengerId2))
                .ReturnsAsync(passenger2);

            // Act
            var result = await _flightPassengerService.GetPassengersByFlightIdAsync(flightId);
            var passengers = result.ToList();

            // Assert
            Assert.AreEqual(2, passengers.Count);
            Assert.AreEqual(passengerId1, passengers[0].Id);
            Assert.AreEqual("Болд", passengers[0].FirstName);
            Assert.AreEqual(passengerId2, passengers[1].Id);
            Assert.AreEqual("Хүсэл", passengers[1].FirstName);
            
            _mockFlightRepository.Verify(repo => repo.GetByIdAsync(flightId), Times.Once);
            _mockFlightPassengerRepository.Verify(repo => repo.FindAsync(It.IsAny<Expression<Func<FlightPassenger, bool>>>()), Times.Once);
            _mockPassengerRepository.Verify(repo => repo.GetByIdAsync(passengerId1), Times.Once);
            _mockPassengerRepository.Verify(repo => repo.GetByIdAsync(passengerId2), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task NislegiinZorchigchdygiAvah_BuruuNislegId_AldaaNv()
        {
            // Arrange
            int flightId = 999;
            _mockFlightRepository.Setup(repo => repo.GetByIdAsync(flightId))
                .ReturnsAsync((Flight)null);

            // Act
            await _flightPassengerService.GetPassengersByFlightIdAsync(flightId);

            // Assert is handled by ExpectedException attribute
        }

        [TestMethod]
        public async Task NislegiinZorchigchdygiAvah_ZorchigchGviNisleg_HoosonList()
        {
            // Arrange
            int flightId = 1;
            var flight = new Flight 
            { 
                Id = flightId, 
                FlightNumber = "MGL123",
                DepartureCity = "Улаанбаатар",
                ArrivalCity = "Москва"
            };

            _mockFlightRepository.Setup(repo => repo.GetByIdAsync(flightId))
                .ReturnsAsync(flight);
            _mockFlightPassengerRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<FlightPassenger, bool>>>()))
                .ReturnsAsync(new List<FlightPassenger>());

            // Act
            var result = await _flightPassengerService.GetPassengersByFlightIdAsync(flightId);

            // Assert
            Assert.AreEqual(0, result.Count());
            _mockFlightRepository.Verify(repo => repo.GetByIdAsync(flightId), Times.Once);
            _mockFlightPassengerRepository.Verify(repo => repo.FindAsync(It.IsAny<Expression<Func<FlightPassenger, bool>>>()), Times.Once);
        }

        #endregion

        #region GetFlightsByPassengerIdAsync Tests

        [TestMethod]
        public async Task ZorchigchiinNisleguudiigAvah_ZovZorchigchId()
        {
            // Arrange
            int passengerId = 1;
            int flightId1 = 10;
            int flightId2 = 20;

            var passenger = new Passenger 
            { 
                Id = passengerId, 
                FirstName = "Болд", 
                LastName = "Баатар",
                PassportNumber = "AA123456"
            };

            var flight1 = new Flight 
            { 
                Id = flightId1, 
                FlightNumber = "MGL123",
                DepartureCity = "Улаанбаатар",
                ArrivalCity = "Москва"
            };

            var flight2 = new Flight 
            { 
                Id = flightId2, 
                FlightNumber = "MGL456",
                DepartureCity = "Москва",
                ArrivalCity = "Улаанбаатар"
            };

            var flightPassengers = new List<FlightPassenger>
            {
                new FlightPassenger { Id = 1, FlightId = flightId1, PassengerId = passengerId },
                new FlightPassenger { Id = 2, FlightId = flightId2, PassengerId = passengerId }
            };

            _mockPassengerRepository.Setup(repo => repo.GetByIdAsync(passengerId))
                .ReturnsAsync(passenger);
            _mockFlightPassengerRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<FlightPassenger, bool>>>()))
                .ReturnsAsync(flightPassengers);
            _mockFlightRepository.Setup(repo => repo.GetByIdAsync(flightId1))
                .ReturnsAsync(flight1);
            _mockFlightRepository.Setup(repo => repo.GetByIdAsync(flightId2))
                .ReturnsAsync(flight2);

            // Act
            var result = await _flightPassengerService.GetFlightsByPassengerIdAsync(passengerId);
            var flights = result.ToList();

            // Assert
            Assert.AreEqual(2, flights.Count);
            Assert.AreEqual(flightId1, flights[0].Id);
            Assert.AreEqual("MGL123", flights[0].FlightNumber);
            Assert.AreEqual(flightId2, flights[1].Id);
            Assert.AreEqual("MGL456", flights[1].FlightNumber);
            
            _mockPassengerRepository.Verify(repo => repo.GetByIdAsync(passengerId), Times.Once);
            _mockFlightPassengerRepository.Verify(repo => repo.FindAsync(It.IsAny<Expression<Func<FlightPassenger, bool>>>()), Times.Once);
            _mockFlightRepository.Verify(repo => repo.GetByIdAsync(flightId1), Times.Once);
            _mockFlightRepository.Verify(repo => repo.GetByIdAsync(flightId2), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task ZorchigchiinNisleguudiigAvah_BuruuZorchigchId_AldaaNv()
        {
            // Arrange
            int passengerId = 999;
            _mockPassengerRepository.Setup(repo => repo.GetByIdAsync(passengerId))
                .ReturnsAsync((Passenger)null);

            // Act
            await _flightPassengerService.GetFlightsByPassengerIdAsync(passengerId);

            // Assert is handled by ExpectedException attribute
        }

        [TestMethod]
        public async Task ZorchigchiinNisleguudiigAvah_NislegGviZorchigch_HoosonList()
        {
            // Arrange
            int passengerId = 1;
            var passenger = new Passenger 
            { 
                Id = passengerId, 
                FirstName = "Болд", 
                LastName = "Баатар",
                PassportNumber = "AA123456"
            };

            _mockPassengerRepository.Setup(repo => repo.GetByIdAsync(passengerId))
                .ReturnsAsync(passenger);
            _mockFlightPassengerRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<FlightPassenger, bool>>>()))
                .ReturnsAsync(new List<FlightPassenger>());

            // Act
            var result = await _flightPassengerService.GetFlightsByPassengerIdAsync(passengerId);

            // Assert
            Assert.AreEqual(0, result.Count());
            _mockPassengerRepository.Verify(repo => repo.GetByIdAsync(passengerId), Times.Once);
            _mockFlightPassengerRepository.Verify(repo => repo.FindAsync(It.IsAny<Expression<Func<FlightPassenger, bool>>>()), Times.Once);
        }

        #endregion

        #region RegisterPassengerToFlightAsync Tests

        [TestMethod]
        public async Task ZorchigchNislegdBurtgeh_ZovId_BurtgelneBa()
        {
            // Arrange
            int flightId = 1;
            int passengerId = 10;
            DateTime testTime = DateTime.UtcNow;

            var flight = new Flight 
            { 
                Id = flightId, 
                FlightNumber = "MGL123",
                DepartureCity = "Улаанбаатар",
                ArrivalCity = "Москва"
            };

            var passenger = new Passenger 
            { 
                Id = passengerId, 
                FirstName = "Болд", 
                LastName = "Баатар",
                PassportNumber = "AA123456"
            };

            _mockFlightRepository.Setup(repo => repo.GetByIdAsync(flightId))
                .ReturnsAsync(flight);
            _mockPassengerRepository.Setup(repo => repo.GetByIdAsync(passengerId))
                .ReturnsAsync(passenger);
            _mockFlightPassengerRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<FlightPassenger, bool>>>()))
                .ReturnsAsync(new List<FlightPassenger>());
            _mockFlightPassengerRepository.Setup(repo => repo.AddAsync(It.IsAny<FlightPassenger>()))
                .Returns(Task.CompletedTask);
            _mockFlightPassengerRepository.Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _flightPassengerService.RegisterPassengerToFlightAsync(flightId, passengerId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(flightId, result.FlightId);
            Assert.AreEqual(passengerId, result.PassengerId);
            
            _mockFlightRepository.Verify(repo => repo.GetByIdAsync(flightId), Times.Once);
            _mockPassengerRepository.Verify(repo => repo.GetByIdAsync(passengerId), Times.Once);
            _mockFlightPassengerRepository.Verify(repo => repo.FindAsync(It.IsAny<Expression<Func<FlightPassenger, bool>>>()), Times.Once);
            _mockFlightPassengerRepository.Verify(repo => repo.AddAsync(It.IsAny<FlightPassenger>()), Times.Once);
            _mockFlightPassengerRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task ZorchigchNislegdBurtgeh_BuruuNislegId_AldaaNv()
        {
            // Arrange
            int flightId = 999;
            int passengerId = 10;

            _mockFlightRepository.Setup(repo => repo.GetByIdAsync(flightId))
                .ReturnsAsync((Flight)null);

            // Act
            await _flightPassengerService.RegisterPassengerToFlightAsync(flightId, passengerId);

            // Assert is handled by ExpectedException attribute
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task ZorchigchNislegdBurtgeh_BuruuZorchigchId_AldaaNv()
        {
            // Arrange
            int flightId = 1;
            int passengerId = 999;

            var flight = new Flight 
            { 
                Id = flightId, 
                FlightNumber = "MGL123",
                DepartureCity = "Улаанбаатар",
                ArrivalCity = "Москва"
            };

            _mockFlightRepository.Setup(repo => repo.GetByIdAsync(flightId))
                .ReturnsAsync(flight);
            _mockPassengerRepository.Setup(repo => repo.GetByIdAsync(passengerId))
                .ReturnsAsync((Passenger)null);

            // Act
            await _flightPassengerService.RegisterPassengerToFlightAsync(flightId, passengerId);

            // Assert is handled by ExpectedException attribute
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task ZorchigchNislegdBurtgeh_BurtgegdsenNisleg_AldaaNv()
        {
            // Arrange
            int flightId = 1;
            int passengerId = 10;

            var flight = new Flight 
            { 
                Id = flightId, 
                FlightNumber = "MGL123",
                DepartureCity = "Улаанбаатар",
                ArrivalCity = "Москва"
            };

            var passenger = new Passenger 
            { 
                Id = passengerId, 
                FirstName = "Болд", 
                LastName = "Баатар",
                PassportNumber = "AA123456"
            };

            var flightPassengers = new List<FlightPassenger>
            {
                new FlightPassenger { Id = 1, FlightId = flightId, PassengerId = passengerId }
            };

            _mockFlightRepository.Setup(repo => repo.GetByIdAsync(flightId))
                .ReturnsAsync(flight);
            _mockPassengerRepository.Setup(repo => repo.GetByIdAsync(passengerId))
                .ReturnsAsync(passenger);
            _mockFlightPassengerRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<FlightPassenger, bool>>>()))
                .ReturnsAsync(flightPassengers);

            // Act
            await _flightPassengerService.RegisterPassengerToFlightAsync(flightId, passengerId);

            // Assert is handled by ExpectedException attribute
        }

        #endregion

        #region RemovePassengerFromFlightAsync Tests (by ID)

        [TestMethod]
        public async Task ZorchigchNislegees_Hasah_IdgeerZov_Hasna()
        {
            // Arrange
            int id = 1;
            var flightPassenger = new FlightPassenger 
            { 
                Id = id, 
                FlightId = 10, 
                PassengerId = 20
            };

            _mockFlightPassengerRepository.Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(flightPassenger);
            _mockFlightPassengerRepository.Setup(repo => repo.DeleteAsync(It.IsAny<FlightPassenger>()))
                .Returns(Task.CompletedTask);

            // Act
            await _flightPassengerService.RemovePassengerFromFlightAsync(id);

            // Assert
            _mockFlightPassengerRepository.Verify(repo => repo.GetByIdAsync(id), Times.Once);
            _mockFlightPassengerRepository.Verify(repo => repo.DeleteAsync(flightPassenger), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task ZorchigchNislegees_Hasah_IdgeerBuruu_AldaaNv()
        {
            // Arrange
            int id = 999;
            _mockFlightPassengerRepository.Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync((FlightPassenger)null);

            // Act
            await _flightPassengerService.RemovePassengerFromFlightAsync(id);

            // Assert is handled by ExpectedException attribute
        }

        #endregion

        #region RemovePassengerFromFlightAsync Tests (by FlightId and PassengerId)

        [TestMethod]
        public async Task ZorchigchNislegees_Hasah_FNIdgeerZov_Hasna()
        {
            // Arrange
            int flightId = 1;
            int passengerId = 10;
            var flightPassenger = new FlightPassenger 
            { 
                Id = 1, 
                FlightId = flightId, 
                PassengerId = passengerId
            };

            var flightPassengers = new List<FlightPassenger> { flightPassenger };

            _mockFlightPassengerRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<FlightPassenger, bool>>>()))
                .ReturnsAsync(flightPassengers);
            _mockFlightPassengerRepository.Setup(repo => repo.DeleteAsync(It.IsAny<FlightPassenger>()))
                .Returns(Task.CompletedTask);

            // Act
            await _flightPassengerService.RemovePassengerFromFlightAsync(flightId, passengerId);

            // Assert
            _mockFlightPassengerRepository.Verify(repo => repo.FindAsync(It.IsAny<Expression<Func<FlightPassenger, bool>>>()), Times.Once);
            _mockFlightPassengerRepository.Verify(repo => repo.DeleteAsync(flightPassenger), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task ZorchigchNislegees_Hasah_FNIdgeerBuruu_AldaaNv()
        {
            // Arrange
            int flightId = 1;
            int passengerId = 10;

            _mockFlightPassengerRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<FlightPassenger, bool>>>()))
                .ReturnsAsync(new List<FlightPassenger>());

            // Act
            await _flightPassengerService.RemovePassengerFromFlightAsync(flightId, passengerId);

            // Assert is handled by ExpectedException attribute
        }

        #endregion

        #region FlightPassengerExistsAsync Tests

        [TestMethod]
        public async Task NislegZorchigchBaigaaEseh_BaigaaId_Vnen()
        {
            // Arrange
            int id = 1;
            var flightPassenger = new FlightPassenger { Id = id, FlightId = 10, PassengerId = 20 };

            _mockFlightPassengerRepository.Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(flightPassenger);

            // Act
            var result = await _flightPassengerService.FlightPassengerExistsAsync(id);

            // Assert
            Assert.IsTrue(result);
            _mockFlightPassengerRepository.Verify(repo => repo.GetByIdAsync(id), Times.Once);
        }

        [TestMethod]
        public async Task NislegZorchigchBaigaaEseh_BaihgviId_Khudal()
        {
            // Arrange
            int id = 999;
            _mockFlightPassengerRepository.Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync((FlightPassenger)null);

            // Act
            var result = await _flightPassengerService.FlightPassengerExistsAsync(id);

            // Assert
            Assert.IsFalse(result);
            _mockFlightPassengerRepository.Verify(repo => repo.GetByIdAsync(id), Times.Once);
        }

        #endregion

        #region PassengerIsRegisteredToFlightAsync Tests

        [TestMethod]
        public async Task ZorchigchNislegdBurtgelteiBaigaaEseh_BaigaaBol_Vnen()
        {
            // Arrange
            int flightId = 1;
            int passengerId = 10;
            var flightPassengers = new List<FlightPassenger>
            {
                new FlightPassenger { Id = 1, FlightId = flightId, PassengerId = passengerId }
            };

            _mockFlightPassengerRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<FlightPassenger, bool>>>()))
                .ReturnsAsync(flightPassengers);

            // Act
            var result = await _flightPassengerService.PassengerIsRegisteredToFlightAsync(flightId, passengerId);

            // Assert
            Assert.IsTrue(result);
            _mockFlightPassengerRepository.Verify(repo => repo.FindAsync(It.IsAny<Expression<Func<FlightPassenger, bool>>>()), Times.Once);
        }

        [TestMethod]
        public async Task ZorchigchNislegdBurtgelteiBaigaaEseh_BaihguiBol_Khudal()
        {
            // Arrange
            int flightId = 1;
            int passengerId = 10;

            _mockFlightPassengerRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<FlightPassenger, bool>>>()))
                .ReturnsAsync(new List<FlightPassenger>());

            // Act
            var result = await _flightPassengerService.PassengerIsRegisteredToFlightAsync(flightId, passengerId);

            // Assert
            Assert.IsFalse(result);
            _mockFlightPassengerRepository.Verify(repo => repo.FindAsync(It.IsAny<Expression<Func<FlightPassenger, bool>>>()), Times.Once);
        }

        #endregion
    }
}
