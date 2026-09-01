using Zaykov.SimpleStoreProject;
using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Zaykov.SimpleStoreProject
{
    public interface ITcpServer
    {
        Task StartAsync(CancellationToken cancellationToken, ISimpleStore simpleStore);
    }
    public class TcpServer : ITcpServer
    {
        private Socket _listenerSocket;
        private ISimpleStore _simpleStore;
        public async Task StartAsync(CancellationToken cancellationToken, ISimpleStore simpleStore)
        {            
            var port = 8080;
            var address = "127.0.0.1";
            _simpleStore = simpleStore;

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
                        var result =  WriteToStore(CommandParser.Parse(span.Span));
                        await clientSocket.SendAsync(Encoding.UTF8.GetBytes(result));
                        

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

        private string WriteToStore(Command command)
        {
            const string nil_result = "(nil)\r\n";
            const string ok_result = "OK\r\n";
            const string wrong_result = "-ERR Wrong syntax\r\n";
            const string unknown_result = "-ERR Unknown command\r\n";

            switch (command.command)
            {
                case "get":
                    try
                    {
                        var result = _simpleStore.Get(command.key.ToString());
                        return result.Name + "\r\n";
                    }
                    catch
                    {
                        return nil_result;
                    }                    
                case "set":
                    try
                    {

                        var profile = JsonSerializer.Deserialize<UserProfile>(command.value);
                        _simpleStore.Set(command.key.ToString(), profile);
                        return ok_result;
                    }
                    catch
                    {
                        return wrong_result;
                    }                    
                case "delete":
                    try
                    {
                        _simpleStore.Delete(command.key.ToString());
                        return ok_result;
                    }
                    catch
                    {
                        return wrong_result;
                    }                                 
                default:
                    return unknown_result;
            }
        }
    }
}
