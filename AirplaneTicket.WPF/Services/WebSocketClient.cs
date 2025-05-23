using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace AirplaneTicket.WPF.Services
{
    public class WebSocketClient
    {
        private Socket _clientSocket;
        private string _serverAddress = "localhost";
        private int _serverPort = 9009;
        private bool _isConnected = false;
        private CancellationTokenSource _cancellationTokenSource;
        private Thread _receiveThread;
        
        // Ивэнтүүд
        public event EventHandler<SeatAssignedEventArgs> SeatAssigned;
        public event EventHandler Connected;
        public event EventHandler Disconnected;
        public event EventHandler<string> ErrorOccurred;
        
        // Холбогдсон эсэх
        public bool IsConnected => _isConnected;
        
        public WebSocketClient(string serverAddress = "localhost", int port = 9009)
        {
            _serverAddress = serverAddress;
            _serverPort = port;
            _cancellationTokenSource = new CancellationTokenSource();
        }
        
        /// <summary>
        /// Серверт холбогдох
        /// </summary>
        public async Task ConnectAsync()
        {
            try
            {
                if (_isConnected)
                {
                    // Хэрэв холбогдсон байвал эхлээд салгана
                    await DisconnectAsync();
                }
                
                // Сокет үүсгэх
                _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                
                // Серверт холбогдох
                await Task.Run(() => 
                {
                    _clientSocket.Connect(_serverAddress, _serverPort);
                });
                
                _isConnected = true;
                LogMessage("WebSocket серверт амжилттай холбогдлоо");
                
                // Мессеж хүлээн авах thread эхлүүлэх
                _cancellationTokenSource = new CancellationTokenSource();
                _receiveThread = new Thread(ReceiveMessages);
                _receiveThread.IsBackground = true;
                _receiveThread.Start();
                
                // Connected ивэнт дуудах
                OnConnected();
            }
            catch (Exception ex)
            {
                _isConnected = false;
                LogError($"Серверт холбогдоход алдаа гарлаа: {ex.Message}");
                OnErrorOccurred($"Серверт холбогдоход алдаа гарлаа: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Серверээс салгах
        /// </summary>
        public async Task DisconnectAsync()
        {
            try
            {
                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Cancel();
                }
                
                if (_clientSocket != null && _clientSocket.Connected)
                {
                    _clientSocket.Shutdown(SocketShutdown.Both);
                    _clientSocket.Close();
                    _clientSocket.Dispose();
                }
                
                _isConnected = false;
                LogMessage("WebSocket серверээс салгагдлаа");
                
                // Disconnected ивэнт дуудах
                OnDisconnected();
            }
            catch (Exception ex)
            {
                LogError($"Серверээс салгахад алдаа гарлаа: {ex.Message}");
                OnErrorOccurred($"Серверээс салгахад алдаа гарлаа: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Серверээс мессеж хүлээн авах
        /// </summary>
        private void ReceiveMessages()
        {
            byte[] buffer = new byte[4096];
            
            try
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested && _clientSocket.Connected)
                {
                    try
                    {
                        int bytesRead = _clientSocket.Receive(buffer);
                        
                        if (bytesRead > 0)
                        {
                            string jsonMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            ProcessMessage(jsonMessage);
                        }
                        else
                        {
                            // 0 байт авсан нь холболт салсан гэсэн үг
                            break;
                        }
                    }
                    catch (SocketException)
                    {
                        // Холболт салсан
                        break;
                    }
                    catch (Exception ex)
                    {
                        LogError($"Мессеж хүлээн авахад алдаа гарлаа: {ex.Message}");
                        break;
                    }
                }
                
                // Холболт салсан бол Disconnected ивэнт дуудах
                if (_isConnected)
                {
                    _isConnected = false;
                    Application.Current.Dispatcher.BeginInvoke(new Action(() => OnDisconnected()));
                }
            }
            catch (Exception ex)
            {
                LogError($"ReceiveMessages thread-д алдаа гарлаа: {ex.Message}");
                
                if (_isConnected)
                {
                    _isConnected = false;
                    Application.Current.Dispatcher.BeginInvoke(new Action(() => OnDisconnected()));
                }
            }
        }
        
        /// <summary>
        /// Хүлээн авсан мессежийг боловсруулах
        /// </summary>
        private void ProcessMessage(string jsonMessage)
        {
            try
            {
                LogMessage($"Хүлээн авсан мессеж: {jsonMessage}");
                
                // JSON-г унших
                using (JsonDocument document = JsonDocument.Parse(jsonMessage))
                {
                    JsonElement root = document.RootElement;
                    
                    // Ивэнтийн нэрийг авах
                    if (root.TryGetProperty("Event", out JsonElement eventNameElement))
                    {
                        string eventName = eventNameElement.GetString();
                        
                        // Өгөгдлийг авах
                        if (root.TryGetProperty("Data", out JsonElement dataElement))
                        {
                            // Ивэнтийн төрлөөс хамааран ялгаатай боловсруулах
                            switch (eventName)
                            {
                                case "SeatAssigned":
                                    ProcessSeatAssignedEvent(dataElement);
                                    break;
                                default:
                                    LogMessage($"Дэмжигдээгүй ивэнт: {eventName}");
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError($"JSON мессеж боловсруулахад алдаа гарлаа: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Суудал оноолтын ивэнтийг боловсруулах
        /// </summary>
        private void ProcessSeatAssignedEvent(JsonElement data)
        {
            try
            {
                // Шаардлагатай мэдээллийг JSON-с авах
                int flightId = 0;
                string seatNumber = "";
                int passengerId = 0;
                
                if (data.TryGetProperty("FlightId", out JsonElement flightIdElement))
                {
                    flightId = flightIdElement.GetInt32();
                }
                
                if (data.TryGetProperty("SeatNumber", out JsonElement seatNumberElement))
                {
                    seatNumber = seatNumberElement.GetString();
                }
                
                if (data.TryGetProperty("PassengerId", out JsonElement passengerIdElement))
                {
                    passengerId = passengerIdElement.GetInt32();
                }
                
                LogMessage($"Суудал оноолтын мэдэгдэл хүлээн авлаа: Нислэг {flightId}, Суудал {seatNumber}, Зорчигч {passengerId}");
                
                // UI thread дээр ивэнт дуудах
                Application.Current.Dispatcher.BeginInvoke(new Action(() => 
                {
                    OnSeatAssigned(new SeatAssignedEventArgs
                    {
                        FlightId = flightId,
                        SeatNumber = seatNumber,
                        PassengerId = passengerId
                    });
                }));
            }
            catch (Exception ex)
            {
                LogError($"Суудал оноолтын ивэнт боловсруулахад алдаа гарлаа: {ex.Message}");
            }
        }
        
        #region Ивэнт дуудах арга
        
        protected virtual void OnSeatAssigned(SeatAssignedEventArgs e)
        {
            SeatAssigned?.Invoke(this, e);
        }
        
        protected virtual void OnConnected()
        {
            Connected?.Invoke(this, EventArgs.Empty);
        }
        
        protected virtual void OnDisconnected()
        {
            Disconnected?.Invoke(this, EventArgs.Empty);
        }
        
        protected virtual void OnErrorOccurred(string errorMessage)
        {
            ErrorOccurred?.Invoke(this, errorMessage);
        }
        
        #endregion
        
        #region Туслах аргууд
        
        private void LogMessage(string message)
        {
            Console.WriteLine($"[WebSocketClient] {message}");
        }
        
        private void LogError(string error)
        {
            Console.WriteLine($"[WebSocketClient ERROR] {error}");
        }
        
        #endregion
    }
    
    /// <summary>
    /// Суудал оноолтын ивэнтийн аргументууд
    /// </summary>
    public class SeatAssignedEventArgs : EventArgs
    {
        public int FlightId { get; set; }
        public string SeatNumber { get; set; }
        public int PassengerId { get; set; }
    }
}
