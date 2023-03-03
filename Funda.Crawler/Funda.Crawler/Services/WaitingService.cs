using System;
namespace Funda.Crawler.Services
{
    public class ExponentialBackoffWaitingService : IWaitingService
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

        public ExponentialBackoffWaitingService()
        {
            JitterMilliseconds = new Random().Next(1000);
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
            Console.WriteLine($"Waiting for {waitTime} milliseconds");

            await Task.Delay(waitTime);

            _millisecondsToWaitNext *= BackoffFactor;
        }
    }

    public interface IWaitingService
    {
        Task Wait();

        void Reset();

        bool CanRetryFurther();
    }
}

