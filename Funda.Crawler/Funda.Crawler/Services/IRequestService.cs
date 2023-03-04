using System;
using Newtonsoft.Json;

namespace Funda.Crawler.Services
{
    public class RequestService<T> : IRequestService<T>
    {
        private static readonly int MaxRetryCount = 10;

        private readonly HttpClient _httpClient;
        private readonly IWaitingService _waitingService;

        public RequestService(HttpClient httpClient, IWaitingService waitingService)
        {
            _httpClient = httpClient;
            _waitingService = waitingService;
        }

        public async Task<T> GetPageResult(string pageUrl)
        {
            Console.WriteLine($"Processing page {pageUrl}");

            _waitingService.Reset();

            while (_waitingService.CanRetryFurther())
            {
                var result = await _httpClient.GetAsync(pageUrl);

                if (result != null && result.IsSuccessStatusCode)
                {
                    var json = await result.Content.ReadAsStringAsync();

                    Console.WriteLine($"Finished getting page {pageUrl}");

                    return JsonConvert.DeserializeObject<T>(json);
                }

                await _waitingService.Wait();
            }

            throw new TimeoutException("Could not get the request in time, giving up.");
        }
    }

    public interface IRequestService<T>
    {
        Task<T> GetPageResult(string pageUrl);
    }
}

