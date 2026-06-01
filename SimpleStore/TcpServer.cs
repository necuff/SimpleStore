using Microsoft.Diagnostics.Utilities;
using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Zaykov.SimpleStore
{
    public interface ITcpServer
    {
        Task StartAsync(CancellationToken cancellationToken);
    }
    public class TcpServer : ITcpServer
    {
        private Socket _listenerSocket;
        public async Task StartAsync(CancellationToken cancellationToken)
        {            
            var port = 8080;
            var address = "127.0.0.1";

            using (_listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                var localEndpoint = new IPEndPoint(IPAddress.Parse(address), port);
                _listenerSocket.Bind(localEndpoint);
                _listenerSocket.Listen(10);

                Console.WriteLine($"Сервер запущен по адресу {address}:{port}");

                while (!cancellationToken.IsCancellationRequested)
                {
                    var clientSocket = await _listenerSocket.AcceptAsync();
                    var clientName = $"Клиент {clientSocket.RemoteEndPoint?.ToString()}";
                    Console.WriteLine($"{clientName} подключился: ");
                    
                    _ = ProcessClientAsync(clientSocket, clientName, cancellationToken);                    
                }
            }                
        }

        private async Task ProcessClientAsync(Socket clientSocket, string clientName, CancellationToken cancellationToken)
        {
            using (clientSocket)
            {
                var buffer = ArrayPool<byte>.Shared.Rent(1024);                

                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        int bytesRead = await clientSocket.ReceiveAsync(buffer);
                        if (bytesRead == 0) break;

                        ReadOnlyMemory<byte> receiveData = new ReadOnlyMemory<byte>(buffer).Slice(0, bytesRead);

                        ReadOnlyMemory<char> span = Encoding.UTF8.GetString(receiveData.Span).AsMemory();

                        WriteToConsole(CommandParser.Parse(span.Span));                        
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

        private void WriteToConsole(Command command)
        {
            Console.WriteLine($"{command.command}, {command.key}, {command.value}");
        }
    }
}
