using Zaykov.SimpleStore;

SimpleStore simpleStore = new SimpleStore();

var getCommand = CommandParser.Parse("GET key1");
var setCommand = CommandParser.Parse("Set key2 value1");
var deleteCommand = CommandParser.Parse("Delete key1");

Console.ReadLine();
