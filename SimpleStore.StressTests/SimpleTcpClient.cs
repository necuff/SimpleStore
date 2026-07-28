using NBomber.Contracts.Cluster;
using System.Net.Sockets;
using System.Text;

namespace SimpleStore.StressTests
{
    public class SimpleTcpClient : IDisposable
    {
        private TcpClient _tcpClient;
        private NetworkStream? _stream;

        public async Task ConnectAsync()
        {
            var port = 8080;
            var address = "127.0.0.1";
            _tcpClient = new TcpClient();            
            await _tcpClient.ConnectAsync(address, port);

            _stream = _tcpClient.GetStream();

            Console.WriteLine("Клиент подключен");
        }

       

        public async Task<string> SetAsync(string key, byte[] value)
        {
            string message = $"set {key} {Encoding.UTF8.GetString(value)}";

            var response = await SendMessageAsync(message);

            Console.WriteLine(response.ToString());
            return response;
        }

        public async Task GetAsync(string key)
        {
            string message = $"get {key}";

            var response = await SendMessageAsync(message);

            Console.WriteLine(response.ToString());
        }

        public async Task DeleteAsync(string key)
        {
            string message = $"delete {key}";

            var response = await SendMessageAsync(message);

            Console.WriteLine(response.ToString());
        }

        public void Disconnect()
        {
            _tcpClient?.Close();
            Console.WriteLine("Клиент отключен");
        }

        public void Dispose()
        {
            Disconnect();
            _tcpClient?.Dispose();
        }

        private async Task<string> ReceiveLineAsync()
        {
            if (_stream == null || !_tcpClient!.Connected)
                throw new InvalidOperationException("Клиент не подключен");

            using var reader = new StreamReader(_stream, Encoding.UTF8, leaveOpen: true);
            return await reader.ReadLineAsync() ?? string.Empty;
        }

        private async Task<string> SendMessageAsync(string message)
        {
            if (_stream == null || !_tcpClient!.Connected)
                throw new InvalidOperationException("Клиент не подключен");

            await _stream.WriteAsync(Encoding.UTF8.GetBytes(message), 0, message.Length);

            var response = await ReceiveLineAsync();
            return response.ToString();
        }
    }
}
