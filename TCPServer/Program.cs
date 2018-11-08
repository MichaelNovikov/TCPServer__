using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace TCPServer
{
    class Program
    {
        private static int _enteranceCount;

        private static ConcurrentDictionary<string, TcpClient> _clients = new ConcurrentDictionary<string, TcpClient>();

        static void Main(string[] args)
        {
            var server = new TcpListener(new IPEndPoint(IPAddress.Any, 50));
            server.Start();
            Console.WriteLine("Server started");

            Listen();

            while (true)
            {
                Console.WriteLine(1);
                string clientName = "";

                var client = server.AcceptTcpClient();
                Console.WriteLine(2);
                var stream = client.GetStream();
                var reader = new StreamReader(stream);
                if (stream.DataAvailable)
                {
                    clientName = reader.ReadLine();

                    Console.WriteLine(clientName + " Entered");

                    _clients.TryAdd(clientName, client);

                    SendMessageToAll(clientName);
                    Console.WriteLine($"Client count {++_enteranceCount}");
                }
            }
        }

        private async static void Listen()
        {
            await Task.Run(() =>
            {
                var flag = true;
                string rKey = "";
                TcpClient rValue;

                while (true)
                {
                    while (flag)
                    {
                        foreach (var item in _clients)
                        {
                            var stream = item.Value.GetStream();
                            var reader = new StreamReader(stream);
                            if (stream.DataAvailable)
                            {
                                var msg = reader.ReadLine();

                                if (msg == "close")
                                {
                                    msg = item.Key + " left the chat";
                                    rKey = item.Key;
                                    rValue = item.Value;
                                    SendMessageToAll(msg);
                                    Console.WriteLine(msg);
                                    item.Value.Close();
                                    flag = false;
                                    break;
                                }

                                SendMessageToAll($"{item.Key}: {msg}");
                                Console.WriteLine($"{item.Key}: {msg}");
                            }
                        }
                    }
                    flag = _clients.TryRemove(rKey, out rValue);
                }
            });
        }

        private static void SendMessageToAll(string message)
        {
            foreach (var item in _clients)
            {
                var stream = item.Value.GetStream();
                var writer = new StreamWriter(stream);

                writer.WriteLine(message);
                writer.Flush();
            }
        }
    }
}
