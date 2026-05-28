using Microsoft.Diagnostics.Utilities;
using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Zaykov.SimpleStore;

namespace SimpleStore
{
    public interface ITcpServer
    {
        Task StartAsync();
    }
    public class TcpServer : ITcpServer
    {
        private Socket _listenerSocket;
        public async Task StartAsync()
        {
            var port = 8080;
            var address = "127.0.0.1";

            using (_listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                var localEndpoint = new IPEndPoint(IPAddress.Parse(address), port);
                _listenerSocket.Bind(localEndpoint);
                _listenerSocket.Listen(10);

                Console.WriteLine($"Сервер запущен по адресу {address}:{port}");

                while (true)
                {
                    var clientSocket = await _listenerSocket.AcceptAsync();
                    var clientName = $"Клиент {clientSocket.RemoteEndPoint?.ToString()}";
                    Console.WriteLine($"{clientName} подключился: ");
                    
                    _ = ProcessClientAsync(clientSocket, clientName);                    
                }
            }                
        }

        private async Task ProcessClientAsync(Socket clientSocket, string clientName)
        {
            using (clientSocket)
            {
                var buffer = ArrayPool<byte>.Shared.Rent(1024);                

                try
                {
                    while (true)
                    {
                        int bytesRead = await clientSocket.ReceiveAsync(buffer);
                        if (bytesRead == 0) break;

                        ReadOnlyMemory<byte> receiveData = new ReadOnlyMemory<byte>(buffer).Slice(0, bytesRead);

                        ReadOnlyMemory<char> span = Encoding.UTF8.GetString(receiveData.Span).AsMemory();

                        var resultCommand = CommandParser.Parse(span.Span).command.ToString().AsMemory();
                        var resultKey = CommandParser.Parse(span.Span).key.ToString().AsMemory();
                        var resultValue = CommandParser.Parse(span.Span).value.ToString().AsMemory();

                        Console.WriteLine($"{resultCommand}, {resultKey}, {resultValue}");
                    }                                        
                }
                catch (Exception ex) 
                {                    
                    Console.WriteLine($"{clientName} разорвал соединение");
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return( buffer );                    
                    Console.WriteLine($"{clientName} отключился");
                    clientSocket.Close();
                }
            }
                
        }
    }
}
