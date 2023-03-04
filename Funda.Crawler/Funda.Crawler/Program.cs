using System.Diagnostics;
using Funda.Crawler.Models;
using Funda.Crawler.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Funda.Crawler;
class Program
{
    private static readonly string FundaApiTemplate =
        "http://partnerapi.funda.nl/feeds/Aanbod.svc/json/ac1b0b1572524640a0ecc54de453ea9f/?type=koop&zo=/amsterdam&page={0}&pagesize=25";

    private static readonly string FundaApiGardenTemplate =
        "http://partnerapi.funda.nl/feeds/Aanbod.svc/json/ac1b0b1572524640a0ecc54de453ea9f/?type=koop&zo=/amsterdam/tuin&page={0}&pagesize=25";

    static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        ConfigureServices(services);

        var serviceProvider = services.BuildServiceProvider();

        var agentFinder = serviceProvider.GetService<AgentFinder>();
        await agentFinder.GetAndDisplayProlificAgentsAsync(FundaApiTemplate);

        Console.ReadLine();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<HttpClient>();
        services.AddTransient<IRequestService<ResultList>, RequestService<ResultList>>();
        services.AddTransient<IWaitingService, ExponentialBackoffWaitingService>();
        services.AddSingleton<ITimedOperation, TimedOperation>();
        services.AddSingleton<ICrawler, Crawler>();
        services.AddSingleton<IResultFormatter, TopTenFormatter>();
        services.AddSingleton<AgentFinder, AgentFinder>();
    }
}

