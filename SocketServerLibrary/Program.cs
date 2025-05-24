using System;
using System.Threading;
using SocketServerLibrary;

namespace SocketServerLibrary
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Socket Server эхлүүлж байна...");
            
            var server = WebSocketServer.Instance;
            server.Start();
            
            Console.WriteLine("Socket Server ажиллаж байна. Зогсоохын тулд 'q' дарна уу.");
            
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.KeyChar == 'q' || key.KeyChar == 'Q')
                {
                    break;
                }
            }
            
            Console.WriteLine("Socket Server зогсож байна...");
            server.Stop();
            Console.WriteLine("Socket Server зогслоо.");
        }
    }
} 