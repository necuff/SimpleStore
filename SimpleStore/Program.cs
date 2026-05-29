using SimpleStore;

/*
SimpleStore simpleStore = new SimpleStore();

var getCommand = CommandParser.Parse("GET key1");
var setCommand = CommandParser.Parse("Set key2 value1");
var deleteCommand = CommandParser.Parse("Delete key1");

*/

TcpServer tcpServer = new TcpServer();
using var cts = new CancellationTokenSource();

AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
{
    cts.Cancel();
};

await tcpServer.StartAsync(cts.Token);

Console.ReadLine();
