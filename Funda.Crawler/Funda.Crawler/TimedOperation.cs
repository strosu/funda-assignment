using System;
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
        public async Task<T> TimeOperationAsync<T>(Func<Task<T>> toExecute)
        {
            var watch = new Stopwatch();
            watch.Start();

            var result = await toExecute();

            Console.WriteLine($"Total elapsed - {watch.Elapsed} milliseconds");

            return result;
        }
    }
}

