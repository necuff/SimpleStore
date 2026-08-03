using NBomber.CSharp;

using SimpleStore.StressTests;

var scenario = Scenario.Create("simple_test", async context =>
{
    SimpleTcpClient simpleTcpClient = new SimpleTcpClient();
    await simpleTcpClient.ConnectAsync(context.ScenarioCancellationToken);

    var step1 = await Step.Run("step1", context, async () =>
    {

        context.ScenarioCancellationToken.ThrowIfCancellationRequested();

        Random rnd = new Random();
        var bytes = new byte[16];
        rnd.NextBytes(bytes);
        var key = Convert.ToBase64String(bytes);
        rnd.NextBytes(bytes);
        var value = bytes;

        var result = await simpleTcpClient.SetAsync(key, value, context.ScenarioCancellationToken);
        if (result == "OK")
            return Response.Ok();

        return Response.Fail();

    });

    simpleTcpClient.Disconnect();
    return Response.Ok();
    })
    .WithWarmUpDuration(TimeSpan.FromSeconds(5))
    .WithLoadSimulations(
        Simulation.Inject(10, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10)),
        Simulation.Inject(100, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30))
    );
    
NBomberRunner.RegisterScenarios(scenario).Run();

Console.ReadLine();