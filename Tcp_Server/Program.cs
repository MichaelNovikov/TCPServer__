using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Tcp_Client
{
    class Program
    {
        private TcpClient _client;
        private StreamReader _reader;
        private StreamWriter _writer;

        private async Task Listen()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    if (_client.Connected && _client.GetStream().DataAvailable)
                    {
                        var text = _reader.ReadLine();
                        Console.WriteLine(text);
                    }
                }
            });
        }

        private void StartClient()
        {
            Random random = new Random();

            _client = new TcpClient();
            _client.Connect("127.0.0.1", 50);
            var stream = _client.GetStream();
            _reader = new StreamReader(stream);
            _writer = new StreamWriter(stream);


            var name = "Alex" + random.Next(1000).ToString();
            _writer.WriteLine(name);
            _writer.Flush();


            Listen();

            while (true)
            {
                var str = Console.ReadLine();
                _writer.WriteLine(str);
                _writer.Flush();
            }
        }

        static void Main(string[] args)
        {
            new Program().StartClient();
        }
    }
}
