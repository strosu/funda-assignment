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

    // THis would be configurable
    private static readonly int DegreeOfParallelism = 5;

    static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        ConfigureServices(services);

        var serviceProvider = services.BuildServiceProvider();

        var agentFinder = serviceProvider.GetService<AgentFinder>();

        await agentFinder.GetAndDisplayProlificAgentsAsync(FundaApiTemplate, DegreeOfParallelism);

        await agentFinder.GetAndDisplayProlificAgentsAsync(FundaApiGardenTemplate, DegreeOfParallelism);

        Console.ReadLine();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<HttpClient>();

        services.AddSingleton<ILogger, ConsoleLogger>();
        services.AddTransient<IRequestService<ResultPage>, RequestService<ResultPage>>();
        services.AddTransient<IWaitingService, ExponentialBackoffWaitingService>();
        services.AddSingleton<ITimedOperation, TimedOperation>();
        services.AddSingleton<ICrawler, SerialCrawler>();
        services.AddSingleton<CrawlerFactory, CrawlerFactory>();
        services.AddSingleton<ICrawlerScheduler, CrawlerScheduler>();
        services.AddSingleton<IResultFormatter, TopTenFormatter>();
        services.AddSingleton<AgentFinder, AgentFinder>();
    }
}

