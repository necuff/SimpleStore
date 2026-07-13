using Zaykov.SimpleStore;


SimpleStore simpleStore = new SimpleStore();

TcpServer tcpServer = new TcpServer();

using var cts = new CancellationTokenSource();

AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
{
    cts.Cancel();
};

await tcpServer.StartAsync(cts.Token, simpleStore);

Console.ReadLine();
