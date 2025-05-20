using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SocketServerLibrary
{
    /// <summary>
    /// WebSocket сервер - клиент апп-тай харилцах
    /// </summary>
    public class WebSocketServer
    {
        // Бүх холбогдсон хэрэглэгчдийг хадгалах ConcurrentDictionary (thread-safe)
        private readonly ConcurrentDictionary<string, WebSocket> _clients = new ConcurrentDictionary<string, WebSocket>();
        
        // WebSocket listener
        private HttpListener _httpListener;
        
        // Server ажиллаж байгаа эсэх
        private bool _isRunning;
        
        // Хэрэглэгчдийн дугаар
        private int _clientCounter = 0;
        
        /// <summary>
        /// WebSocket сервер эхлүүлэх
        /// </summary>
        /// <param name="port">Ашиглах порт</param>
        public async Task StartAsync(int port = 8085)
        {
            if (_isRunning)
                return;
                
            _isRunning = true;
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add($"http://localhost:{port}/");
            _httpListener.Start();
            
            Console.WriteLine($"WebSocket сервер {port} порт дээр амжилттай эхэллээ");
            
            // Хэрэглэгчийн холболтыг хүлээн авах
            await AcceptClientsAsync();
        }
        
        /// <summary>
        /// WebSocket сервер зогсоох
        /// </summary>
        public async Task StopAsync()
        {
            if (!_isRunning)
                return;
            
            _isRunning = false;
            
            // Бүх хэрэглэгчийн холболтыг хаах
            foreach (var clientSocket in _clients.Values)
            {
                try
                {
                    await clientSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Сервер хаагдаж байна", CancellationToken.None);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Хэрэглэгчийн холболтыг хаахад алдаа гарлаа: {ex.Message}");
                }
            }
            
            _clients.Clear();
            _httpListener.Stop();
            
            Console.WriteLine("WebSocket сервер зогссон");
        }
        
        /// <summary>
        /// Бүх хэрэглэгч рүү мессэж илгээх
        /// </summary>
        /// <param name="eventName">Ивэнтийн нэр</param>
        /// <param name="data">Илгээх өгөгдөл</param>
        public async Task SendMessageToAllAsync(string eventName, object data)
        {
            var message = JsonSerializer.Serialize(new { Event = eventName, Data = data });
            var messageBytes = Encoding.UTF8.GetBytes(message);
            var toRemove = new List<string>();
            
            foreach (var client in _clients)
            {
                try
                {
                    if (client.Value.State == WebSocketState.Open)
                    {
                        await client.Value.SendAsync(
                            new ArraySegment<byte>(messageBytes, 0, messageBytes.Length),
                            WebSocketMessageType.Text,
                            true,
                            CancellationToken.None);
                    }
                    else
                    {
                        toRemove.Add(client.Key);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Хэрэглэгч {client.Key} руу мессэж илгээх үед алдаа гарлаа: {ex.Message}");
                    toRemove.Add(client.Key);
                }
            }
            
            // Холболт тасарсан хэрэглэгчдийг устгах
            foreach (var clientId in toRemove)
            {
                WebSocket socket;
                _clients.TryRemove(clientId, out socket);
                Console.WriteLine($"Хэрэглэгч {clientId} устгагдлаа");
            }
        }
        
        /// <summary>
        /// Нэг тодорхой хэрэглэгч рүү мессэж илгээх
        /// </summary>
        public async Task SendMessageToClientAsync(string clientId, string eventName, object data)
        {
            WebSocket client;
            if (_clients.TryGetValue(clientId, out client) && client.State == WebSocketState.Open)
            {
                var message = JsonSerializer.Serialize(new { Event = eventName, Data = data });
                var messageBytes = Encoding.UTF8.GetBytes(message);
                
                await client.SendAsync(
                    new ArraySegment<byte>(messageBytes, 0, messageBytes.Length),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);
            }
        }
        
        /// <summary>
        /// Хэрэглэгчдийн холболтыг хүлээн авах
        /// </summary>
        private async Task AcceptClientsAsync()
        {
            while (_isRunning)
            {
                try
                {
                    var context = await _httpListener.GetContextAsync();
                    
                    if (context.Request.IsWebSocketRequest)
                    {
                        // WebSocket хүсэлтийг хүлээн авч, шинэ хэрэглэгч бүртгэх
                        var webSocketContext = await context.AcceptWebSocketAsync(null);
                        var socket = webSocketContext.WebSocket;
                        
                        string clientId = $"client_{Interlocked.Increment(ref _clientCounter)}";
                        _clients.TryAdd(clientId, socket);
                        
                        Console.WriteLine($"Шинэ хэрэглэгч {clientId} холбогдлоо");
                        
                        // Хэрэглэгчээс мессэж хүлээн авах процесс эхлүүлэх
                        _ = HandleClientMessagesAsync(clientId, socket);
                    }
                    else
                    {
                        // Хэрэв WebSocket хүсэлт биш бол стандарт HTTP хариу илгээх
                        context.Response.StatusCode = 400;
                        context.Response.Close();
                    }
                }
                catch (Exception ex)
                {
                    if (_isRunning)
                    {
                        Console.WriteLine($"Хэрэглэгчийн холболт хүлээн авахад алдаа гарлаа: {ex.Message}");
                    }
                }
            }
        }
        
        /// <summary>
        /// Хэрэглэгчээс ирэх мессэжийг зохицуулах
        /// </summary>
        private async Task HandleClientMessagesAsync(string clientId, WebSocket socket)
        {
            var buffer = new byte[4096];
            
            try
            {
                while (socket.State == WebSocketState.Open && _isRunning)
                {
                    var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        // Хүлээн авсан мессэжийг боловсруулах
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        Console.WriteLine($"Хэрэглэгч {clientId}-с мессэж хүлээн авлаа: {message}");
                        
                        // Хүлээн авсан мессэжид хариу боловсруулах боломжтой
                        // Жишээлбэл, сонсож байгаа ивэнтийн бүртгэл хийх гэх мэт
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await socket.CloseAsync(
                            WebSocketCloseStatus.NormalClosure,
                            "Хэрэглэгч холболтоо салгасан",
                            CancellationToken.None);
                            
                        WebSocket removedSocket;
                        _clients.TryRemove(clientId, out removedSocket);
                        Console.WriteLine($"Хэрэглэгч {clientId} холболтоо салгасан");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Хэрэглэгч {clientId}-ийн мессэж зохицуулахад алдаа гарлаа: {ex.Message}");
                
                if (socket.State == WebSocketState.Open)
                {
                    try
                    {
                        await socket.CloseAsync(
                            WebSocketCloseStatus.InternalServerError,
                            "Серверт алдаа гарлаа",
                            CancellationToken.None);
                    }
                    catch { } // Ignore exceptions on close
                }
                
                WebSocket removedSocket;
                _clients.TryRemove(clientId, out removedSocket);
            }
        }
    }
}
