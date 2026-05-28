using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using SimpleStore;
using System.Security.Cryptography;
using Zaykov.SimpleStore;

/*
SimpleStore simpleStore = new SimpleStore();

var getCommand = CommandParser.Parse("GET key1");
var setCommand = CommandParser.Parse("Set key2 value1");
var deleteCommand = CommandParser.Parse("Delete key1");

*/


TcpServer tcpServer = new TcpServer();
await tcpServer.StartAsync();



Console.ReadLine();
