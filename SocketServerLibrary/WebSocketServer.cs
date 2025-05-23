using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SocketServerLibrary
{
    /// <summary>
    /// Энгийн Socket сервер хэрэгжүүлэлт - клиент аппликейшнтай харилцах
    /// </summary>
    public class WebSocketServer
    {
        // Singleton pattern хэрэгжүүлэлт
        private static WebSocketServer _instance;
        private static readonly object _lock = new object();
        
        public static WebSocketServer Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new WebSocketServer(9009);
                        }
                    }
                }
                return _instance;
            }
        }
        
        private ConcurrentDictionary<int, Socket> _connectedSockets;
        private CancellationTokenSource _cancellationTokenSource;
        private List<Thread> _threads;
        private int _port = 9009;
        private Socket _serverSocket;
        public bool HasStarted { get; private set; }
        private int _clientIdCounter = 0;

        public WebSocketServer(int port = 9009)
        {
            _port = port;
            _connectedSockets = new ConcurrentDictionary<int, Socket>();
            _cancellationTokenSource = new CancellationTokenSource();
            _threads = new List<Thread>();
            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            HasStarted = false;
        }

        /// <summary>
        /// Серверийг эхлүүлэх
        /// </summary>
        public void Start()
        {
            if (HasStarted)
                return;

            try
            {
                // Сервер socket үүсгэх
                _serverSocket.Bind(new IPEndPoint(IPAddress.Any, _port));
                _serverSocket.Listen(100); // Хүлээн авах клиентийн тоо
                
                HasStarted = true;
                Console.WriteLine($"WebSocket сервер {_port} порт дээр амжилттай эхэллээ");

                // Холболт хүлээн авах thread эхлүүлэх
                Thread acceptThread = new Thread(AcceptConnections);
                acceptThread.IsBackground = true;
                acceptThread.Start();
                _threads.Add(acceptThread);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Сервер эхлүүлэхэд алдаа гарлаа: {ex.Message}");
            }
        }

        /// <summary>
        /// Клиент холболтыг хүлээн авах
        /// </summary>
        private void AcceptConnections()
        {
            try
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    Socket clientSocket = _serverSocket.Accept();
                    int clientId = Interlocked.Increment(ref _clientIdCounter);
                    
                    _connectedSockets.TryAdd(clientId, clientSocket);
                    Console.WriteLine($"Шинэ клиент холбогдлоо: {clientId}");

                    // Мессеж хүлээн авах thread эхлүүлэх
                    Thread receiveThread = new Thread(() => ReceiveMessages(clientId, clientSocket));
                    receiveThread.IsBackground = true;
                    receiveThread.Start();
                    _threads.Add(receiveThread);
                }
            }
            catch (Exception ex)
            {
                if (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    Console.WriteLine($"Холболт хүлээн авахад алдаа гарлаа: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Клиентээс мессеж хүлээн авах
        /// </summary>
        private void ReceiveMessages(int clientId, Socket clientSocket)
        {
            byte[] buffer = new byte[4096];
            
            try
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested && clientSocket.Connected)
                {
                    int bytesRead = clientSocket.Receive(buffer);
                    
                    if (bytesRead > 0)
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Console.WriteLine($"Хүлээн авсан мессеж (клиент {clientId}): {message}");
                    }
                    else
                    {
                        // Клиент холболтоо салгасан
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Клиент {clientId} холболт салгагдлаа: {ex.Message}");
            }
            finally
            {
                // Холболт салгагдсан үед цэвэрлэх
                CleanupClient(clientId, clientSocket);
            }
        }

        /// <summary>
        /// Бүх клиент рүү мессеж илгээх
        /// </summary>
        public void SendMessageToAll(string eventName, object data)
        {
            byte[] messageBytes = CreateMessageBytes(eventName, data);
            List<int> disconnectedClients = new List<int>();
            
            foreach (var client in _connectedSockets)
            {
                try
                {
                    if (client.Value.Connected)
                    {
                        client.Value.Send(messageBytes);
                    }
                    else
                    {
                        disconnectedClients.Add(client.Key);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Мессеж илгээхэд алдаа гарлаа (клиент {client.Key}): {ex.Message}");
                    disconnectedClients.Add(client.Key);
                }
            }
            
            // Холболт салгагдсан клиентүүдийг цэвэрлэх
            foreach (int clientId in disconnectedClients)
            {
                if (_connectedSockets.TryRemove(clientId, out Socket socket))
                {
                    CloseSocketSafely(socket);
                    Console.WriteLine($"Клиент {clientId} холболт цэвэрлэгдлээ (автомат)");
                }
            }
        }

        /// <summary>
        /// Тодорхой нэг клиент рүү мессеж илгээх
        /// </summary>
        public bool SendMessageToClient(int clientId, string eventName, object data)
        {
            if (!_connectedSockets.TryGetValue(clientId, out Socket clientSocket))
                return false;
                
            try
            {
                byte[] messageBytes = CreateMessageBytes(eventName, data);
                
                if (clientSocket.Connected)
                {
                    clientSocket.Send(messageBytes);
                    return true;
                }
                else
                {
                    CleanupClient(clientId, clientSocket);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Мессеж илгээхэд алдаа гарлаа (клиент {clientId}): {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Серверийг зогсоох
        /// </summary>
        public void Stop()
        {
            if (!HasStarted)
                return;
                
            try
            {
                _cancellationTokenSource.Cancel();
                
                // Бүх клиентийн холболтыг хаах
                foreach (var client in _connectedSockets)
                {
                    CloseSocketSafely(client.Value);
                }
                
                _connectedSockets.Clear();
                
                // Сервер сокетийг хаах
                CloseSocketSafely(_serverSocket);
                
                HasStarted = false;
                Console.WriteLine("WebSocket сервер зогслоо");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Сервер зогсооход алдаа гарлаа: {ex.Message}");
            }
        }

        /// <summary>
        /// Суудал оноох үед мэдэгдэл илгээх
        /// </summary>
        public void NotifySeatAssigned(int flightId, string seatNumber, int passengerId)
        {
            var data = new 
            {
                FlightId = flightId,
                SeatNumber = seatNumber,
                PassengerId = passengerId,
                Timestamp = DateTime.UtcNow
            };
            
            SendMessageToAll("SeatAssigned", data);
            Console.WriteLine($"Суудал оноолтын мэдэгдэл илгээгдлээ: Нислэг {flightId}, Суудал {seatNumber}, Зорчигч {passengerId}");
        }

        #region Helper методууд

        /// <summary>
        /// JSON мессеж үүсгэж byte array болгох
        /// </summary>
        private byte[] CreateMessageBytes(string eventName, object data)
        {
            var messageObj = new { Event = eventName, Data = data, Timestamp = DateTime.UtcNow };
            string jsonMessage = JsonSerializer.Serialize(messageObj);
            return Encoding.UTF8.GetBytes(jsonMessage);
        }

        /// <summary>
        /// Клиентийг аюулгүй цэвэрлэх
        /// </summary>
        private void CleanupClient(int clientId, Socket clientSocket)
        {
            _connectedSockets.TryRemove(clientId, out _);
            CloseSocketSafely(clientSocket);
            Console.WriteLine($"Клиент {clientId} холболт цэвэрлэгдлээ");
        }

        /// <summary>
        /// Socket-г аюулгүй хаах
        /// </summary>
        private void CloseSocketSafely(Socket socket)
        {
            try { socket?.Close(); } catch { }
        }

        #endregion
    }
}
