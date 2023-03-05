using System;
using Newtonsoft.Json;

namespace Funda.Crawler.Services
{
    public class RequestService<T> : IRequestService<T>
    {
        private static readonly int MaxRetryCount = 10;

        private readonly HttpClient _httpClient;
        private readonly IWaitingService _waitingService;
        private readonly ILogger _logger;

        public RequestService(HttpClient httpClient, IWaitingService waitingService, ILogger logger)
        {
            _httpClient = httpClient;
            _waitingService = waitingService;
            _logger = logger;
        }

        public async Task<T> GetPageResult(string pageUrl)
        {
            _logger.Log($"Processing page {pageUrl}");

            _waitingService.Reset();

            while (_waitingService.CanRetryFurther())
            {
                var result = await _httpClient.GetAsync(pageUrl);

                if (result != null && result.IsSuccessStatusCode)
                {
                    var json = await result.Content.ReadAsStringAsync();

                    _logger.Log($"Finished getting page {pageUrl}");

                    return JsonConvert.DeserializeObject<T>(json);
                }

                await _waitingService.Wait();
            }

            var errorMessage = "Could not get the request in time, giving up.";
            _logger.LogError(errorMessage);
            throw new TimeoutException(errorMessage);
        }
    }

    public interface IRequestService<T>
    {
        Task<T> GetPageResult(string pageUrl);
    }
}

