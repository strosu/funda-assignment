namespace Funda.Crawler.Services
{
    /// <summary>
    /// Service used as a strategy for waiting between failed requests
    /// Implements an exponential backoff; this works well when there's an outage, might not be the best for when throttled
    /// As the sliding window is 60s and we exhaust it pretty early in the execution, we need to get to the 32-64s interval, at which point the waiting times between requests are rather large
    /// </summary>
    public class ExponentialBackoffRetryService : IRetryService
    {
        private static readonly int BackoffFactor = 2;

        private static readonly int MaxRetryCount = 10;

        /// <summary>
        /// Jitter to reduce the cluttering of requests
        /// </summary>
        private readonly int JitterMilliseconds;

        private int _retryCount;

        /// <summary>
        /// Start with 1 second waiting time
        /// </summary>
        private int _millisecondsToWaitNext;

        private readonly ILogger _logger;

        public ExponentialBackoffRetryService(ILogger logger)
        {
            _logger = logger;
            JitterMilliseconds = new Random().Next(100); // A request takes about 200ms on average, the halfway point seems like a decent pick for noise
            Reset();
        }

        public void Reset()
        {
            _retryCount = 0;
            _millisecondsToWaitNext = 1000;
        }

        public bool CanRetryFurther()
        {
            return _retryCount <= MaxRetryCount;
        }

        public async Task Wait()
        {
            if (_retryCount > MaxRetryCount)
            {
                throw new TimeoutException("Too many retries, giving up. Better luck next time.");
            }

            _retryCount++;

            var waitTime = _millisecondsToWaitNext + JitterMilliseconds;
            _logger.Log($"Waiting for {waitTime} milliseconds");

            await Task.Delay(waitTime);

            _millisecondsToWaitNext *= BackoffFactor;
        }
    }

    public interface IRetryService
    {
        Task Wait();

        void Reset();

        bool CanRetryFurther();
    }
}

