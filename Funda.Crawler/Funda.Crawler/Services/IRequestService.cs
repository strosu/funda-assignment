using Newtonsoft.Json;

namespace Funda.Crawler.Services
{
    public class RequestService<T> : IRequestService<T>
    {
        private readonly HttpClient _httpClient;
        private readonly IRetryService _retryService;
        private readonly ILogger _logger;

        public RequestService(HttpClient httpClient, IRetryService retryService, ILogger logger)
        {
            _httpClient = httpClient;
            _retryService = retryService;
            _logger = logger;
        }

        public async Task<T> GetPageResultAsync(string pageUrl)
        {
            _logger.Log($"Processing page {pageUrl}");

            _retryService.Reset();

            while (_retryService.CanRetryFurther())
            {
                try
                {
                    var result = await _httpClient.GetAsync(pageUrl);

                    if (result != null && result.IsSuccessStatusCode)
                    {
                        var json = await result.Content.ReadAsStringAsync();

                        _logger.Log($"Finished getting page {pageUrl}");

                        return JsonConvert.DeserializeObject<T>(json);
                    }
                }
                catch (Exception ex)
                {
                    // This is meant to catch unhandled exceptions, e.g. server throwing a 500 for whatever reason; In that case, don't just give up
                    _logger.LogError(ex.Message);
                }

                await _retryService.Wait();
            }

            var errorMessage = "Could not get the request in time, giving up.";
            _logger.LogError(errorMessage);
            throw new TimeoutException(errorMessage);
        }
    }

    public interface IRequestService<T>
    {
        Task<T> GetPageResultAsync(string pageUrl);
    }
}

