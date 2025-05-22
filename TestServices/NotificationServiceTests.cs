// using BusinessLogic.Services;
// using DataAccess.Models;
// using Microsoft.AspNetCore.SignalR;
// using Moq;
// using System.Text.Json;

// namespace TestServices
// {
//     [TestClass]
//     public class NotificationServiceTests
//     {
//         private Mock<IHubContext<NotificationHubBase>> _mockHubContext;
//         private Mock<IHubClients> _mockClients;
//         private Mock<IClientProxy> _mockClientProxy;
//         private NotificationService _notificationService;

//         [TestInitialize]
//         public void Setup()
//         {
//             _mockHubContext = new Mock<IHubContext<NotificationHubBase>>();
//             _mockClients = new Mock<IHubClients>();
//             _mockClientProxy = new Mock<IClientProxy>();

//             _mockHubContext.Setup(h => h.Clients).Returns(_mockClients.Object);
//             _mockClients.Setup(c => c.Group(It.IsAny<string>())).Returns(_mockClientProxy.Object);
            
//             _notificationService = new NotificationService(_mockHubContext.Object);
//         }

//         [TestMethod]
//         public async Task NotifyFlightStatusChanged_SendsSignalRNotification()
//         {
//             // Arrange
//             int flightId = 2;
//             FlightStatus newStatus = FlightStatus.Boarding;

//             // Act
//             await _notificationService.NotifyFlightStatusChangedAsync(flightId, newStatus);

//             // Assert
//             _mockClients.Verify(
//                 clients => clients.Group($"flight_{flightId}"),
//                 Times.Once);

//             _mockClientProxy.Verify(
//                 clientProxy => clientProxy.SendAsync(
//                     "ReceiveFlightStatusUpdate",
//                     It.Is<string>(json => ContainsExpectedData(json, flightId, newStatus)),
//                     It.IsAny<CancellationToken>()),
//                 Times.Once);
//         }

//         private bool ContainsExpectedData(string json, int flightId, FlightStatus newStatus)
//         {
//             try
//             {
//                 // Хайж олох түлхүүр утгууд
//                 bool hasFlightId = json.Contains($"\"FlightId\":{flightId}");
//                 bool hasNewStatus = json.Contains($"\"NewStatus\":{(int)newStatus}");
//                 bool hasStatusText = json.Contains($"\"StatusText\":\"{newStatus}\"");
                
//                 Console.WriteLine($"JSON: {json}");
//                 Console.WriteLine($"Шалгалтын үр дүн: HasFlightId={hasFlightId}, HasNewStatus={hasNewStatus}, HasStatusText={hasStatusText}");
                
//                 return hasFlightId && hasNewStatus && hasStatusText;
//             }
//             catch
//             {
//                 return false;
//             }
//         }

//         // [TestMethod]
//         // public async Task NotifySeatAssigned_LogsMessageButDoesNotSendSignalR()
//         // {
//         //     // Arrange
//         //     int flightId = 2;
//         //     string seatNumber = "A1";
//         //     int passengerId = 456;

//         //     // Act
//         //     await _notificationService.NotifySeatAssignedAsync(flightId, seatNumber, passengerId);

//         //     // Assert - Энэ метод одоогоор SignalR ашигладаггүй учир дуудагдахгүй байх ёстой
//         //     _mockClients.Verify(
//         //         clients => clients.Group(It.IsAny<string>()),
//         //         Times.Never);

//         //     _mockClientProxy.Verify(
//         //         clientProxy => clientProxy.SendAsync(
//         //             It.IsAny<string>(),
//         //             It.IsAny<object>(),
//         //             It.IsAny<CancellationToken>()),
//         //         Times.Never);
//         // }
//     }
// }
