using Funda.Crawler.Services;
using System.Diagnostics;

namespace Funda.Crawler
{
    public interface ITimedOperation
    {
        /// <summary>
        /// Wraps an async operation and outputs how long it took
        /// We're using this for benchmarking, in order to find the fastest strategies (e.g. number of concurrent requests, backoff approaches etc).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toExecute"></param>
        /// <returns></returns>
        Task<T> TimeOperationAsync<T>(Func<Task<T>> toExecute);
    }

    public class TimedOperation : ITimedOperation
    {
        private readonly ILogger _logger;

        public TimedOperation(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<T> TimeOperationAsync<T>(Func<Task<T>> toExecute)
        {
            var watch = new Stopwatch();
            watch.Start();

            var result = await toExecute();

            _logger.Log($"Total elapsed - {watch.Elapsed} milliseconds");

            return result;
        }
    }
}

