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
    public class PassengerServiceTests
    {
        private Mock<IRepository<Passenger>> _mockPassengerRepository;
        private Mock<IRepository<FlightPassenger>> _mockFlightPassengerRepository;
        private PassengerService _passengerService;

        [TestInitialize]
        public void Initialize()
        {
            _mockPassengerRepository = new Mock<IRepository<Passenger>>();
            _mockFlightPassengerRepository = new Mock<IRepository<FlightPassenger>>();
            _passengerService = new PassengerService(_mockPassengerRepository.Object, _mockFlightPassengerRepository.Object);
        }

        #region GetAllPassengersAsync Tests

        [TestMethod]
        public async Task BuhZorchigchdiigAvah_ShvvltGvi()
        {
            // Arrange
            var testPassengers = new List<Passenger>
            {
                new Passenger { Id = 1, FirstName = "Болд", LastName = "Баатар", Email = "bold@example.com", PassportNumber = "AA123456" },
                new Passenger { Id = 2, FirstName = "Хүсэл", LastName = "Бат", Email = "khusel@example.com", PassportNumber = "BB123456" }
            };

            _mockPassengerRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(testPassengers);

            // Act
            var result = await _passengerService.GetAllPassengersAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            _mockPassengerRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [TestMethod]
        public async Task ZorchigchdiigNereerShvvj_Avah()
        {
            // Arrange
            var testPassengers = new List<Passenger>
            {
                new Passenger { Id = 1, FirstName = "Болд", LastName = "Баатар", Email = "bold@example.com", PassportNumber = "AA123456" },
                new Passenger { Id = 2, FirstName = "Хүсэл", LastName = "Бат", Email = "khusel@example.com", PassportNumber = "BB123456" }
            };

            _mockPassengerRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(testPassengers);

            // Act
            var result = await _passengerService.GetAllPassengersAsync(nameFilter: "болд");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("Болд", result.First().FirstName);
            _mockPassengerRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [TestMethod]
        public async Task ZorchigchdiigPassportaarShvvj_Avah()
        {
            // Arrange
            var testPassengers = new List<Passenger>
            {
                new Passenger { Id = 1, FirstName = "Болд", LastName = "Баатар", Email = "bold@example.com", PassportNumber = "AA123456" },
                new Passenger { Id = 2, FirstName = "Хүсэл", LastName = "Бат", Email = "khusel@example.com", PassportNumber = "BB123456" }
            };

            _mockPassengerRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(testPassengers);

            // Act
            var result = await _passengerService.GetAllPassengersAsync(passportNumber: "BB123");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("Хүсэл", result.First().FirstName);
            _mockPassengerRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        #endregion

        #region GetPassengerByIdAsync Tests

        [TestMethod]
        public async Task ZorchigchdiigIdgeerAvah_BaigaaId()
        {
            // Arrange
            int testId = 1;
            var testPassenger = new Passenger
            {
                Id = testId,
                FirstName = "Болд",
                LastName = "Баатар",
                Email = "bold@example.com",
                PassportNumber = "AA123456"
            };

            _mockPassengerRepository.Setup(repo => repo.GetByIdAsync(testId))
                .ReturnsAsync(testPassenger);

            // Act
            var result = await _passengerService.GetPassengerByIdAsync(testId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(testId, result.Id);
            Assert.AreEqual("Болд", result.FirstName);
            _mockPassengerRepository.Verify(repo => repo.GetByIdAsync(testId), Times.Once);
        }

        [TestMethod]
        public async Task ZorchigchdiigIdgeerAvah_BaihgviId()
        {
            // Arrange
            int testId = 999;
            _mockPassengerRepository.Setup(repo => repo.GetByIdAsync(testId))
                .ReturnsAsync((Passenger)null);

            // Act
            var result = await _passengerService.GetPassengerByIdAsync(testId);

            // Assert
            Assert.IsNull(result);
            _mockPassengerRepository.Verify(repo => repo.GetByIdAsync(testId), Times.Once);
        }

        #endregion

        #region GetPassengerByPassportNumberAsync Tests

        [TestMethod]
        public async Task ZorchigchdiigPassportoorAvah_BaigaaPassport()
        {
            // Arrange
            string passportNumber = "AA123456";
            var testPassenger = new Passenger
            {
                Id = 1,
                FirstName = "Болд",
                LastName = "Баатар",
                Email = "bold@example.com",
                PassportNumber = passportNumber
            };

            _mockPassengerRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Passenger, bool>>>()))
                .ReturnsAsync(new List<Passenger> { testPassenger });

            // Act
            var result = await _passengerService.GetPassengerByPassportNumberAsync(passportNumber);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("Болд", result.FirstName);
            Assert.AreEqual(passportNumber, result.PassportNumber);
            _mockPassengerRepository.Verify(repo => repo.FindAsync(It.IsAny<Expression<Func<Passenger, bool>>>()), Times.Once);
        }

        [TestMethod]
        public async Task ZorchigchdiigPassportoorAvah_BaihgviPassport()
        {
            // Arrange
            string passportNumber = "XX999999";
            _mockPassengerRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Passenger, bool>>>()))
                .ReturnsAsync(new List<Passenger>());

            // Act
            var result = await _passengerService.GetPassengerByPassportNumberAsync(passportNumber);

            // Assert
            Assert.IsNull(result);
            _mockPassengerRepository.Verify(repo => repo.FindAsync(It.IsAny<Expression<Func<Passenger, bool>>>()), Times.Once);
        }

        #endregion

        #region AddPassengerAsync Tests

        [TestMethod]
        public async Task ZorchigchNemeh_ZovMedeelel()
        {
            // Arrange
            var passenger = new Passenger
            {
                FirstName = "Хүсэл",
                LastName = "Бат",
                Email = "khusel@example.com",
                PassportNumber = "BB123456"
            };

            _mockPassengerRepository.Setup(repo => repo.AddAsync(It.IsAny<Passenger>()))
                .Returns(Task.CompletedTask);
            _mockPassengerRepository.Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _passengerService.AddPassengerAsync(passenger);

            // Assert
            _mockPassengerRepository.Verify(repo => repo.AddAsync(passenger), Times.Once);
            _mockPassengerRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ZorchigchNemeh_KhoosoonUtga_AldaaNv()
        {
            // Act
            await _passengerService.AddPassengerAsync(null);

            // Assert is handled by ExpectedException attribute
        }

        #endregion

        #region UpdatePassengerAsync Tests

        [TestMethod]
        public async Task ZorchigchShinchleh_ZovMedeelel()
        {
            // Arrange
            var passenger = new Passenger
            {
                Id = 1,
                FirstName = "Болд",
                LastName = "Баатаргүй", // Changed last name
                Email = "bold.new@example.com",
                PassportNumber = "AA123456"
            };

            _mockPassengerRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Passenger>()))
                .Returns(Task.CompletedTask);
            _mockPassengerRepository.Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _passengerService.UpdatePassengerAsync(passenger);

            // Assert
            _mockPassengerRepository.Verify(repo => repo.UpdateAsync(passenger), Times.Once);
            _mockPassengerRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ZorchigchShinchleh_KhoosoonUtga_AldaaNv()
        {
            // Act
            await _passengerService.UpdatePassengerAsync(null);

            // Assert is handled by ExpectedException attribute
        }

        #endregion

        #region DeletePassengerAsync Tests

        [TestMethod]
        public async Task ZorchigchUstgah_ZovIdNisleggvi()
        {
            // Arrange
            int testId = 1;
            var testPassenger = new Passenger
            {
                Id = testId,
                FirstName = "Болд",
                LastName = "Баатар",
                Email = "bold@example.com",
                PassportNumber = "AA123456"
            };

            _mockPassengerRepository.Setup(repo => repo.GetByIdAsync(testId))
                .ReturnsAsync(testPassenger);
            _mockFlightPassengerRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<FlightPassenger, bool>>>()))
                .ReturnsAsync(new List<FlightPassenger>());
            _mockPassengerRepository.Setup(repo => repo.DeleteAsync(It.IsAny<Passenger>()))
                .Returns(Task.CompletedTask);
            _mockPassengerRepository.Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _passengerService.DeletePassengerAsync(testId);

            // Assert
            _mockPassengerRepository.Verify(repo => repo.DeleteAsync(testPassenger), Times.Once);
            _mockPassengerRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task ZorchigchUstgah_BaihgviId_AldaaNv()
        {
            // Arrange
            int testId = 999;
            _mockPassengerRepository.Setup(repo => repo.GetByIdAsync(testId))
                .ReturnsAsync((Passenger)null);

            // Act
            await _passengerService.DeletePassengerAsync(testId);

            // Assert is handled by ExpectedException attribute
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task ZorchigchUstgah_Nisleltei_AldaaNv()
        {
            // Arrange
            int testId = 1;
            var testPassenger = new Passenger
            {
                Id = testId,
                FirstName = "Болд",
                LastName = "Баатар",
                Email = "bold@example.com",
                PassportNumber = "AA123456"
            };

            var flightPassengers = new List<FlightPassenger>
            {
                new FlightPassenger { PassengerId = testId, FlightId = 100 }
            };

            _mockPassengerRepository.Setup(repo => repo.GetByIdAsync(testId))
                .ReturnsAsync(testPassenger);
            _mockFlightPassengerRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<FlightPassenger, bool>>>()))
                .ReturnsAsync(flightPassengers);

            // Act
            await _passengerService.DeletePassengerAsync(testId);

            // Assert is handled by ExpectedException attribute
        }

        #endregion

        #region PassengerExistsAsync Tests

        [TestMethod]
        public async Task ZorchigchBaigaaEsehShalgah_BaigaaId_Vnen()
        {
            // Arrange
            int passengerId = 1;
            var testPassenger = new Passenger
            {
                Id = passengerId,
                FirstName = "Болд",
                LastName = "Баатар",
                Email = "bold@example.com",
                PassportNumber = "AA123456"
            };

            _mockPassengerRepository.Setup(repo => repo.GetByIdAsync(passengerId))
                .ReturnsAsync(testPassenger);

            // Act
            var result = await _passengerService.PassengerExistsAsync(passengerId);

            // Assert
            Assert.IsTrue(result);
            _mockPassengerRepository.Verify(repo => repo.GetByIdAsync(passengerId), Times.Once);
        }

        [TestMethod]
        public async Task ZorchigchBaigaaEsehShalgah_BaihgviId_Khudal()
        {
            // Arrange
            int passengerId = 999;
            _mockPassengerRepository.Setup(repo => repo.GetByIdAsync(passengerId))
                .ReturnsAsync((Passenger)null);

            // Act
            var result = await _passengerService.PassengerExistsAsync(passengerId);

            // Assert
            Assert.IsFalse(result);
            _mockPassengerRepository.Verify(repo => repo.GetByIdAsync(passengerId), Times.Once);
        }

        #endregion
    }
}
