using System.Diagnostics;
using Funda.Crawler.Models;
using Funda.Crawler.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Funda.Crawler;
class Program
{
    static void Main(string[] args)
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        var crawler = services
            .AddSingleton<Crawler, Crawler>()
            .BuildServiceProvider()
            .GetService<Crawler>();

        var watch = new Stopwatch();
        watch.Start();

        var listings = crawler.GetListings("http://partnerapi.funda.nl/feeds/Aanbod.svc/json/ac1b0b1572524640a0ecc54de453ea9f/?type=koop&zo=/amsterdam&page={0}&pagesize=25")
            .GetAwaiter().GetResult();

        var mostListings = listings.GroupBy(x => (x.MakelaarId, x.MakelaarNaam)).Select(x => new
        {
            AgentName = x.Key.MakelaarNaam,
            ListingCount = x.Count()
        }
        ).ToList().OrderByDescending(x => x.ListingCount).Take(10);

        Console.WriteLine("Results:");

        foreach (var prolificAgent in mostListings)
        {
            Console.WriteLine($"{prolificAgent.AgentName} - {prolificAgent.ListingCount}");
        }

        Console.WriteLine($"Total elapsed - {watch.Elapsed} milliseconds");

        Console.ReadLine();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<HttpClient>();
        services
            .AddSingleton<IRequestService<ResultList>, RequestService<ResultList>>();
    }
}

