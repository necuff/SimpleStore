using NBomber.Contracts.Cluster;
using System.Net.Sockets;
using System.Text;

namespace SimpleStore.StressTests
{
    public class SimpleTcpClient : IDisposable
    {
        private TcpClient _tcpClient;
        private NetworkStream? _stream;
        const int port = 8080;
        const string address = "127.0.0.1";

        public async Task ConnectAsync(CancellationToken cancellationToken)
        {            
            
            _tcpClient = new TcpClient();            
            await _tcpClient.ConnectAsync(address, port, cancellationToken);

            _stream = _tcpClient.GetStream();

            Console.WriteLine("Клиент подключен");
        }

        public async Task<string> SetAsync(string key, byte[] value, CancellationToken cancellationToken)
        {
            if(cancellationToken.IsCancellationRequested)
                return string.Empty;

            if (key.Length == 0 || value.Length == 0)
                return string.Empty;
            string message = $"set {key} {Encoding.UTF8.GetString(value)}";

            var response = await SendMessageAsync(message);

            Console.WriteLine(response.ToString());
            return response;
        }

        public async Task GetAsync(string key, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;
            if (key.Length == 0)
                return;
            string message = $"get {key}";

            var response = await SendMessageAsync(message);            
        }

        public async Task DeleteAsync(string key, CancellationToken cancellationToken)            
        {
            if (cancellationToken.IsCancellationRequested)
                return;
            if (key.Length == 0)
                return;
            string message = $"delete {key}";

            var response = await SendMessageAsync(message);            
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
