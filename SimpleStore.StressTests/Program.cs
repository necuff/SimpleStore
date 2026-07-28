using NBomber.CSharp;

using SimpleStore.StressTests;

var scenario = Scenario.Create("simple_test", async context =>
{
    SimpleTcpClient simpleTcpClient = new SimpleTcpClient();
    await simpleTcpClient.ConnectAsync();

    var step1 = await Step.Run("step1", context, async () =>
    {
        Random rnd = new Random();
        var bytes = new byte[16];
        rnd.NextBytes(bytes);
        var key = Convert.ToBase64String(bytes);
        rnd.NextBytes(bytes);
        var value = bytes;

        var result = await simpleTcpClient.SetAsync(key, value);
        if (result == "OK")
            return Response.Ok();

        return Response.Fail();

    });

    simpleTcpClient.Disconnect();
    return Response.Ok();
})
    .WithLoadSimulations(
        Simulation.Inject(10, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10)),
        Simulation.Inject(100, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30))
    );
    
NBomberRunner.RegisterScenarios(scenario).Run();

Console.ReadLine();