using System;
using Newtonsoft.Json;

namespace Funda.Crawler.Services
{
    public class RequestService<T> : IRequestService<T>
    {
        // 10 reties with backoff should be enough
        private static readonly int MaxRetryCount = 10;

        private readonly HttpClient _httpClient;

        public RequestService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<T> GetPageResult(string pageUrl)
        {
            var retryCount = 1;
            var waitSeconds = 1;

            while (retryCount < MaxRetryCount)
            {
                try
                {
                    var result = await _httpClient.GetAsync(pageUrl);

                    if (result != null && result.IsSuccessStatusCode)
                    {
                        var json = await result.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<T>(json);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                retryCount++;
                Console.WriteLine($"Waiting {waitSeconds * 1000}");

                await Task.Delay(waitSeconds * 1000);
                waitSeconds *= 2;
            }

            throw new TimeoutException("Could not get the request in time");
        }
    }

    public interface IRequestService<T>
    {
        Task<T> GetPageResult(string pageUrl);
    }
}

