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
    public class SeatServiceTests
    {
        private Mock<IRepository<Seat>> _mockSeatRepository;
        private Mock<IRepository<Flight>> _mockFlightRepository;
        private Mock<IRepository<Passenger>> _mockPassengerRepository;
        private Mock<IRepository<FlightPassenger>> _mockFlightPassengerRepository;
        private Mock<INotificationService> _mockNotificationService;
        private SeatService _seatService;

        [TestInitialize]
        public void Initialize()
        {
            _mockSeatRepository = new Mock<IRepository<Seat>>();
            _mockFlightRepository = new Mock<IRepository<Flight>>();
            _mockPassengerRepository = new Mock<IRepository<Passenger>>();
            _mockFlightPassengerRepository = new Mock<IRepository<FlightPassenger>>();
            _mockNotificationService = new Mock<INotificationService>();
            
            _seatService = new SeatService(
                _mockSeatRepository.Object,
                _mockFlightRepository.Object,
                _mockPassengerRepository.Object,
                _mockFlightPassengerRepository.Object,
                _mockNotificationService.Object);
        }

        #region GetSeatsByFlightIdAsync Tests

        [TestMethod]
        public async Task NislegiinIdgeerBuhSuudliigAvah()
        {
            // Arrange
            int flightId = 1;
            var flight = new Flight { Id = flightId, FlightNumber = "MGL123" };
            
            var seats = new List<Seat>
            {
                new Seat { Id = 1, FlightId = flightId, SeatNumber = "1A", IsOccupied = false },
                new Seat { Id = 2, FlightId = flightId, SeatNumber = "1B", IsOccupied = true, PassengerId = 10 },
                new Seat { Id = 3, FlightId = flightId, SeatNumber = "1C", IsOccupied = false }
            };

            _mockFlightRepository.Setup(repo => repo.GetByIdAsync(flightId))
                .ReturnsAsync(flight);
            _mockSeatRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Seat, bool>>>()))
                .ReturnsAsync(seats);

            // Act
            var result = await _seatService.GetSeatsByFlightIdAsync(flightId);
            var resultList = result.ToList();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, resultList.Count);
            Assert.AreEqual("1A", resultList[0].SeatNumber);
            Assert.AreEqual("1B", resultList[1].SeatNumber);
            Assert.AreEqual("1C", resultList[2].SeatNumber);
            _mockFlightRepository.Verify(repo => repo.GetByIdAsync(flightId), Times.Once);
            _mockSeatRepository.Verify(repo => repo.FindAsync(It.IsAny<Expression<Func<Seat, bool>>>()), Times.Once);
        }

        [TestMethod]
        public async Task NislegiinEzelsenSuudliigAvah()
        {
            // Arrange
            int flightId = 1;
            var flight = new Flight { Id = flightId, FlightNumber = "MGL123" };
            
            var allSeats = new List<Seat>
            {
                new Seat { Id = 1, FlightId = flightId, SeatNumber = "1A", IsOccupied = false },
                new Seat { Id = 2, FlightId = flightId, SeatNumber = "1B", IsOccupied = true, PassengerId = 10 },
                new Seat { Id = 3, FlightId = flightId, SeatNumber = "1C", IsOccupied = false }
            };

            var occupiedSeats = allSeats.Where(s => s.IsOccupied).ToList();

            _mockFlightRepository.Setup(repo => repo.GetByIdAsync(flightId))
                .ReturnsAsync(flight);
            _mockSeatRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Seat, bool>>>()))
                .ReturnsAsync(allSeats);

            // Act
            var result = await _seatService.GetSeatsByFlightIdAsync(flightId, isOccupied: true);
            var resultList = result.ToList();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, resultList.Count);
            Assert.AreEqual("1B", resultList[0].SeatNumber);
            Assert.IsTrue(resultList[0].IsOccupied);
            _mockFlightRepository.Verify(repo => repo.GetByIdAsync(flightId), Times.Once);
            _mockSeatRepository.Verify(repo => repo.FindAsync(It.IsAny<Expression<Func<Seat, bool>>>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task BuruuNislegiinIdgeerHashildneBaina()
        {
            // Arrange
            int flightId = 999;
            _mockFlightRepository.Setup(repo => repo.GetByIdAsync(flightId))
                .ReturnsAsync((Flight)null);

            // Act
            await _seatService.GetSeatsByFlightIdAsync(flightId);

            
        }

        #endregion

        #region GetSeatByIdAsync Tests

        [TestMethod]
        public async Task SuudliinIdgeerSuudliigAvah()
        {
            // Arrange
            int seatId = 1;
            var seat = new Seat 
            { 
                Id = seatId, 
                FlightId = 10, 
                SeatNumber = "1A",
                IsOccupied = false
            };

            _mockSeatRepository.Setup(repo => repo.GetByIdAsync(seatId))
                .ReturnsAsync(seat);

            // Act
            var result = await _seatService.GetSeatByIdAsync(seatId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(seatId, result.Id);
            Assert.AreEqual("1A", result.SeatNumber);
            Assert.AreEqual(10, result.FlightId);
            _mockSeatRepository.Verify(repo => repo.GetByIdAsync(seatId), Times.Once);
        }

        [TestMethod]
        public async Task BuruuSuudliinIdgeerNullBuitsaana()
        {
            // Arrange
            int seatId = 999;
            _mockSeatRepository.Setup(repo => repo.GetByIdAsync(seatId))
                .ReturnsAsync((Seat)null);

            // Act
            var result = await _seatService.GetSeatByIdAsync(seatId);

            // Assert
            Assert.IsNull(result);
            _mockSeatRepository.Verify(repo => repo.GetByIdAsync(seatId), Times.Once);
        }

        #endregion
        
        #region AssignSeatAsync Tests

        [TestMethod]
        public async Task ZorchigchdSuudalOnooh()
        {
            // Arrange
            int flightId = 1;
            int passengerId = 10;
            int seatId = 100;
            
            var flight = new Flight { Id = flightId, FlightNumber = "MGL123" };
            var passenger = new Passenger { Id = passengerId, FirstName = "Болд", LastName = "Баатар" };
            var seat = new Seat { Id = seatId, FlightId = flightId, SeatNumber = "1A", IsOccupied = false };
            var flightPassenger = new FlightPassenger { Id = 50, FlightId = flightId, PassengerId = passengerId };

            _mockFlightRepository.Setup(repo => repo.GetByIdAsync(flightId))
                .ReturnsAsync(flight);
            _mockPassengerRepository.Setup(repo => repo.GetByIdAsync(passengerId))
                .ReturnsAsync(passenger);
            _mockSeatRepository.Setup(repo => repo.GetByIdAsync(seatId))
                .ReturnsAsync(seat);
            _mockFlightPassengerRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<FlightPassenger, bool>>>()))
                .ReturnsAsync(new List<FlightPassenger> { flightPassenger });
            _mockSeatRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Seat, bool>>>()))
                .ReturnsAsync(new List<Seat>());
            _mockSeatRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Seat>()))
                .Returns(Task.CompletedTask);
            _mockSeatRepository.Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);
            _mockNotificationService.Setup(service => service.NotifySeatAssignedAsync(flightId, seat.SeatNumber, passengerId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _seatService.AssignSeatAsync(flightId, passengerId, seatId);

            // Assert
            Assert.IsTrue(result);
            _mockFlightRepository.Verify(repo => repo.GetByIdAsync(flightId), Times.Once);
            _mockPassengerRepository.Verify(repo => repo.GetByIdAsync(passengerId), Times.Once);
            _mockSeatRepository.Verify(repo => repo.GetByIdAsync(seatId), Times.Once);
            _mockFlightPassengerRepository.Verify(repo => repo.FindAsync(It.IsAny<Expression<Func<FlightPassenger, bool>>>()), Times.Once);
            _mockSeatRepository.Verify(repo => repo.FindAsync(It.IsAny<Expression<Func<Seat, bool>>>()), Times.Once);
            _mockSeatRepository.Verify(repo => repo.UpdateAsync(It.Is<Seat>(s => 
                s.Id == seatId && 
                s.IsOccupied == true && 
                s.PassengerId == passengerId)), Times.Once);
            _mockSeatRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            _mockNotificationService.Verify(service => service.NotifySeatAssignedAsync(flightId, seat.SeatNumber, passengerId), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task BuruuNislegiinIdgeerSuudalOnoohod_HashildneBaina()
        {
            // Arrange
            int flightId = 999;
            int passengerId = 10;
            int seatId = 100;
            
            _mockFlightRepository.Setup(repo => repo.GetByIdAsync(flightId))
                .ReturnsAsync((Flight)null);

            // Act
            await _seatService.AssignSeatAsync(flightId, passengerId, seatId);

            
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task BuruuZorchigchiinIdgeerSuudalOnoohod_HashildneBaina()
        {
            // Arrange
            int flightId = 1;
            int passengerId = 999;
            int seatId = 100;
            
            var flight = new Flight { Id = flightId, FlightNumber = "MGL123" };
            
            _mockFlightRepository.Setup(repo => repo.GetByIdAsync(flightId))
                .ReturnsAsync(flight);
            _mockPassengerRepository.Setup(repo => repo.GetByIdAsync(passengerId))
                .ReturnsAsync((Passenger)null);

            // Act
            await _seatService.AssignSeatAsync(flightId, passengerId, seatId);

            
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task NislegtBurtgeeguiZorchigchdSuudalOnoohod_AldaaNu()
        {
            // Arrange
            int flightId = 1;
            int passengerId = 10;
            int seatId = 100;
            
            var flight = new Flight { Id = flightId, FlightNumber = "MGL123" };
            var passenger = new Passenger { Id = passengerId, FirstName = "Болд", LastName = "Баатар" };
            
            _mockFlightRepository.Setup(repo => repo.GetByIdAsync(flightId))
                .ReturnsAsync(flight);
            _mockPassengerRepository.Setup(repo => repo.GetByIdAsync(passengerId))
                .ReturnsAsync(passenger);
            _mockFlightPassengerRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<FlightPassenger, bool>>>()))
                .ReturnsAsync(new List<FlightPassenger>()); // Зорчигч нислэгт бүртгэлгүй

            // Act
            await _seatService.AssignSeatAsync(flightId, passengerId, seatId);

            
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task BuruuSuudliinIdgeerSuudalOnoohod_HashildneBaina()
        {
            // Arrange
            int flightId = 1;
            int passengerId = 10;
            int seatId = 999;
            
            var flight = new Flight { Id = flightId, FlightNumber = "MGL123" };
            var passenger = new Passenger { Id = passengerId, FirstName = "Болд", LastName = "Баатар" };
            var flightPassenger = new FlightPassenger { Id = 50, FlightId = flightId, PassengerId = passengerId };
            
            _mockFlightRepository.Setup(repo => repo.GetByIdAsync(flightId))
                .ReturnsAsync(flight);
            _mockPassengerRepository.Setup(repo => repo.GetByIdAsync(passengerId))
                .ReturnsAsync(passenger);
            _mockFlightPassengerRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<FlightPassenger, bool>>>()))
                .ReturnsAsync(new List<FlightPassenger> { flightPassenger });
            _mockSeatRepository.Setup(repo => repo.GetByIdAsync(seatId))
                .ReturnsAsync((Seat)null);

            // Act
            await _seatService.AssignSeatAsync(flightId, passengerId, seatId);

            
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task NislegtHamaaraguitSuudliigOnoohod_AldaaNu()
        {
            // Arrange
            int flightId = 1;
            int passengerId = 10;
            int seatId = 100;
            int differentFlightId = 2; // Өөр нислэгийн ID
            
            var flight = new Flight { Id = flightId, FlightNumber = "MGL123" };
            var passenger = new Passenger { Id = passengerId, FirstName = "Болд", LastName = "Баатар" };
            var flightPassenger = new FlightPassenger { Id = 50, FlightId = flightId, PassengerId = passengerId };
            var seat = new Seat { Id = seatId, FlightId = differentFlightId, SeatNumber = "1A", IsOccupied = false };
            
            _mockFlightRepository.Setup(repo => repo.GetByIdAsync(flightId))
                .ReturnsAsync(flight);
            _mockPassengerRepository.Setup(repo => repo.GetByIdAsync(passengerId))
                .ReturnsAsync(passenger);
            _mockFlightPassengerRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<FlightPassenger, bool>>>()))
                .ReturnsAsync(new List<FlightPassenger> { flightPassenger });
            _mockSeatRepository.Setup(repo => repo.GetByIdAsync(seatId))
                .ReturnsAsync(seat);

            // Act
            await _seatService.AssignSeatAsync(flightId, passengerId, seatId);

            
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task EzelsenSuudliigDahinOnoohod_AldaaNu()
        {
            // Arrange
            int flightId = 1;
            int passengerId = 10;
            int seatId = 100;
            int otherPassengerId = 20;
            
            var flight = new Flight { Id = flightId, FlightNumber = "MGL123" };
            var passenger = new Passenger { Id = passengerId, FirstName = "Болд", LastName = "Баатар" };
            var flightPassenger = new FlightPassenger { Id = 50, FlightId = flightId, PassengerId = passengerId };
            var seat = new Seat { Id = seatId, FlightId = flightId, SeatNumber = "1A", IsOccupied = true, PassengerId = otherPassengerId };
            
            _mockFlightRepository.Setup(repo => repo.GetByIdAsync(flightId))
                .ReturnsAsync(flight);
            _mockPassengerRepository.Setup(repo => repo.GetByIdAsync(passengerId))
                .ReturnsAsync(passenger);
            _mockFlightPassengerRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<FlightPassenger, bool>>>()))
                .ReturnsAsync(new List<FlightPassenger> { flightPassenger });
            _mockSeatRepository.Setup(repo => repo.GetByIdAsync(seatId))
                .ReturnsAsync(seat);

            // Act
            await _seatService.AssignSeatAsync(flightId, passengerId, seatId);

            
        }

        #endregion

        #region ReleaseSeatAsync Tests

        [TestMethod]
        public async Task SuudliigChoolooloh()
        {
            // Arrange
            int flightId = 1;
            int seatId = 100;
            int passengerId = 10;
            
            var flight = new Flight { Id = flightId, FlightNumber = "MGL123" };
            var seat = new Seat { Id = seatId, FlightId = flightId, SeatNumber = "1A", IsOccupied = true, PassengerId = passengerId };

            _mockFlightRepository.Setup(repo => repo.GetByIdAsync(flightId))
                .ReturnsAsync(flight);
            _mockSeatRepository.Setup(repo => repo.GetByIdAsync(seatId))
                .ReturnsAsync(seat);
            _mockSeatRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Seat>()))
                .Returns(Task.CompletedTask);
            _mockSeatRepository.Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _seatService.ReleaseSeatAsync(flightId, seatId);

            // Assert
            Assert.IsTrue(result);
            _mockFlightRepository.Verify(repo => repo.GetByIdAsync(flightId), Times.Once);
            _mockSeatRepository.Verify(repo => repo.GetByIdAsync(seatId), Times.Once);
            _mockSeatRepository.Verify(repo => repo.UpdateAsync(It.Is<Seat>(s => 
                s.Id == seatId && 
                s.IsOccupied == false && 
                s.PassengerId == null && 
                s.CheckInTime == null)), Times.Once);
            _mockSeatRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public async Task EzleeguiSuudliigChoolooloh()
        {
            // Arrange
            int flightId = 1;
            int seatId = 100;
            
            var flight = new Flight { Id = flightId, FlightNumber = "MGL123" };
            var seat = new Seat { Id = seatId, FlightId = flightId, SeatNumber = "1A", IsOccupied = false, PassengerId = null };

            _mockFlightRepository.Setup(repo => repo.GetByIdAsync(flightId))
                .ReturnsAsync(flight);
            _mockSeatRepository.Setup(repo => repo.GetByIdAsync(seatId))
                .ReturnsAsync(seat);

            // Act
            var result = await _seatService.ReleaseSeatAsync(flightId, seatId);

            // Assert
            Assert.IsTrue(result);
            _mockFlightRepository.Verify(repo => repo.GetByIdAsync(flightId), Times.Once);
            _mockSeatRepository.Verify(repo => repo.GetByIdAsync(seatId), Times.Once);
            // Сайжруулж уг суудал нь хоосон учир Update дуудагдаж байх ёсгүй
            _mockSeatRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Seat>()), Times.Never);
            _mockSeatRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task BuruuNislegiinIdgeerSuudalChooloolohod_HashildneBaina()
        {
            // Arrange
            int flightId = 999;
            int seatId = 100;
            
            _mockFlightRepository.Setup(repo => repo.GetByIdAsync(flightId))
                .ReturnsAsync((Flight)null);

            // Act
            await _seatService.ReleaseSeatAsync(flightId, seatId);

            
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task BuruuSuudliinIdgeerSuudalChooloolohod_HashildneBaina()
        {
            // Arrange
            int flightId = 1;
            int seatId = 999;
            
            var flight = new Flight { Id = flightId, FlightNumber = "MGL123" };
            
            _mockFlightRepository.Setup(repo => repo.GetByIdAsync(flightId))
                .ReturnsAsync(flight);
            _mockSeatRepository.Setup(repo => repo.GetByIdAsync(seatId))
                .ReturnsAsync((Seat)null);

            // Act
            await _seatService.ReleaseSeatAsync(flightId, seatId);

            
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task NislegtHamaaraguitSuudliigChooloolohod_AldaaNu()
        {
            // Arrange
            int flightId = 1;
            int seatId = 100;
            int differentFlightId = 2; // Өөр нислэгийн ID
            
            var flight = new Flight { Id = flightId, FlightNumber = "MGL123" };
            var seat = new Seat { Id = seatId, FlightId = differentFlightId, SeatNumber = "1A", IsOccupied = true };
            
            _mockFlightRepository.Setup(repo => repo.GetByIdAsync(flightId))
                .ReturnsAsync(flight);
            _mockSeatRepository.Setup(repo => repo.GetByIdAsync(seatId))
                .ReturnsAsync(seat);

            // Act
            await _seatService.ReleaseSeatAsync(flightId, seatId);

            
        }

        #endregion

        #region SeatExistsAsync Tests

        [TestMethod]
        public async Task SuudalBaigaEsehShalgah_BaigaaVed()
        {
            // Arrange
            int seatId = 1;
            var seat = new Seat { Id = seatId, FlightId = 10, SeatNumber = "1A" };

            _mockSeatRepository.Setup(repo => repo.GetByIdAsync(seatId))
                .ReturnsAsync(seat);

            // Act
            var result = await _seatService.SeatExistsAsync(seatId);

            // Assert
            Assert.IsTrue(result);
            _mockSeatRepository.Verify(repo => repo.GetByIdAsync(seatId), Times.Once);
        }

        [TestMethod]
        public async Task SuudalBaigaEsehShalgah_BaihguiVed()
        {
            // Arrange
            int seatId = 999;
            _mockSeatRepository.Setup(repo => repo.GetByIdAsync(seatId))
                .ReturnsAsync((Seat)null);

            // Act
            var result = await _seatService.SeatExistsAsync(seatId);

            // Assert
            Assert.IsFalse(result);
            _mockSeatRepository.Verify(repo => repo.GetByIdAsync(seatId), Times.Once);
        }

        #endregion

        #region IsSeatAvailableAsync Tests

        [TestMethod]
        public async Task SuudalBolotshoitEsehShalgah_BolotshoitoiVed()
        {
            // Arrange
            int seatId = 1;
            var seat = new Seat { Id = seatId, FlightId = 10, SeatNumber = "1A", IsOccupied = false };

            _mockSeatRepository.Setup(repo => repo.GetByIdAsync(seatId))
                .ReturnsAsync(seat);

            // Act
            var result = await _seatService.IsSeatAvailableAsync(seatId);

            // Assert
            Assert.IsTrue(result);
            _mockSeatRepository.Verify(repo => repo.GetByIdAsync(seatId), Times.Once);
        }

        [TestMethod]
        public async Task SuudalBolotshoitEsehShalgah_BolotshoigviVed()
        {
            // Arrange
            int seatId = 1;
            var seat = new Seat { Id = seatId, FlightId = 10, SeatNumber = "1A", IsOccupied = true, PassengerId = 5 };

            _mockSeatRepository.Setup(repo => repo.GetByIdAsync(seatId))
                .ReturnsAsync(seat);

            // Act
            var result = await _seatService.IsSeatAvailableAsync(seatId);

            // Assert
            Assert.IsFalse(result);
            _mockSeatRepository.Verify(repo => repo.GetByIdAsync(seatId), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task BuruuSuudliinIdgeerShalgahad_HashildneBaina()
        {
            // Arrange
            int seatId = 999;
            _mockSeatRepository.Setup(repo => repo.GetByIdAsync(seatId))
                .ReturnsAsync((Seat)null);

            // Act
            await _seatService.IsSeatAvailableAsync(seatId);

            
        }

        #endregion

        #region GetPassengerSeatAsync Tests

        [TestMethod]
        public async Task ZorchigchiinSuudliigAvah_SuudaltaiVed()
        {
            // Arrange
            int flightId = 1;
            int passengerId = 10;
            
            var flight = new Flight { Id = flightId, FlightNumber = "MGL123" };
            var passenger = new Passenger { Id = passengerId, FirstName = "Болд", LastName = "Баатар" };
            var flightPassenger = new FlightPassenger { Id = 50, FlightId = flightId, PassengerId = passengerId };
            var seat = new Seat { Id = 100, FlightId = flightId, SeatNumber = "1A", IsOccupied = true, PassengerId = passengerId };

            _mockFlightRepository.Setup(repo => repo.GetByIdAsync(flightId))
                .ReturnsAsync(flight);
            _mockPassengerRepository.Setup(repo => repo.GetByIdAsync(passengerId))
                .ReturnsAsync(passenger);
            _mockFlightPassengerRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<FlightPassenger, bool>>>()))
                .ReturnsAsync(new List<FlightPassenger> { flightPassenger });
            _mockSeatRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Seat, bool>>>()))
                .ReturnsAsync(new List<Seat> { seat });

            // Act
            var result = await _seatService.GetPassengerSeatAsync(flightId, passengerId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(seat.Id, result.Id);
            Assert.AreEqual(seat.SeatNumber, result.SeatNumber);
            Assert.AreEqual(passengerId, result.PassengerId);
            Assert.IsTrue(result.IsOccupied);
            _mockFlightRepository.Verify(repo => repo.GetByIdAsync(flightId), Times.Once);
            _mockPassengerRepository.Verify(repo => repo.GetByIdAsync(passengerId), Times.Once);
            _mockFlightPassengerRepository.Verify(repo => repo.FindAsync(It.IsAny<Expression<Func<FlightPassenger, bool>>>()), Times.Once);
            _mockSeatRepository.Verify(repo => repo.FindAsync(It.IsAny<Expression<Func<Seat, bool>>>()), Times.Once);
        }

        [TestMethod]
        public async Task ZorchigchiinSuudliigAvah_SuudalgviVed()
        {
            // Arrange
            int flightId = 1;
            int passengerId = 10;
            
            var flight = new Flight { Id = flightId, FlightNumber = "MGL123" };
            var passenger = new Passenger { Id = passengerId, FirstName = "Болд", LastName = "Баатар" };
            var flightPassenger = new FlightPassenger { Id = 50, FlightId = flightId, PassengerId = passengerId };

            _mockFlightRepository.Setup(repo => repo.GetByIdAsync(flightId))
                .ReturnsAsync(flight);
            _mockPassengerRepository.Setup(repo => repo.GetByIdAsync(passengerId))
                .ReturnsAsync(passenger);
            _mockFlightPassengerRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<FlightPassenger, bool>>>()))
                .ReturnsAsync(new List<FlightPassenger> { flightPassenger });
            _mockSeatRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Seat, bool>>>()))
                .ReturnsAsync(new List<Seat>()); // Зорчигчид суудал оноогдоогүй

            // Act
            var result = await _seatService.GetPassengerSeatAsync(flightId, passengerId);

            // Assert
            Assert.IsNull(result);
            _mockFlightRepository.Verify(repo => repo.GetByIdAsync(flightId), Times.Once);
            _mockPassengerRepository.Verify(repo => repo.GetByIdAsync(passengerId), Times.Once);
            _mockFlightPassengerRepository.Verify(repo => repo.FindAsync(It.IsAny<Expression<Func<FlightPassenger, bool>>>()), Times.Once);
            _mockSeatRepository.Verify(repo => repo.FindAsync(It.IsAny<Expression<Func<Seat, bool>>>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task BuruuNislegiinIdgeerZorchigchiinSuudliigAvahad_HashildneBaina()
        {
            // Arrange
            int flightId = 999;
            int passengerId = 10;
            
            _mockFlightRepository.Setup(repo => repo.GetByIdAsync(flightId))
                .ReturnsAsync((Flight)null);

            // Act
            await _seatService.GetPassengerSeatAsync(flightId, passengerId);

            
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task BuruuZorchigchiinIdgeerZorchigchiinSuudliigAvahad_HashildneBaina()
        {
            // Arrange
            int flightId = 1;
            int passengerId = 999;
            
            var flight = new Flight { Id = flightId, FlightNumber = "MGL123" };
            
            _mockFlightRepository.Setup(repo => repo.GetByIdAsync(flightId))
                .ReturnsAsync(flight);
            _mockPassengerRepository.Setup(repo => repo.GetByIdAsync(passengerId))
                .ReturnsAsync((Passenger)null);

            // Act
            await _seatService.GetPassengerSeatAsync(flightId, passengerId);

            
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task NislegtBurtgeeguiZorchigchiinSuudliigAvahad_AldaaNu()
        {
            // Arrange
            int flightId = 1;
            int passengerId = 10;
            
            var flight = new Flight { Id = flightId, FlightNumber = "MGL123" };
            var passenger = new Passenger { Id = passengerId, FirstName = "Болд", LastName = "Баатар" };
            
            _mockFlightRepository.Setup(repo => repo.GetByIdAsync(flightId))
                .ReturnsAsync(flight);
            _mockPassengerRepository.Setup(repo => repo.GetByIdAsync(passengerId))
                .ReturnsAsync(passenger);
            _mockFlightPassengerRepository.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<FlightPassenger, bool>>>()))
                .ReturnsAsync(new List<FlightPassenger>()); // Зорчигч нислэгт бүртгэлгүй

            // Act
            await _seatService.GetPassengerSeatAsync(flightId, passengerId);

            
        }

        #endregion
    }
}
