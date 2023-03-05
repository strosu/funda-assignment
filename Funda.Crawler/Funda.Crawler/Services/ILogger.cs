using System;
namespace Funda.Crawler.Services
{
    public interface ILogger
    {
        void Log(string message);
        void LogError(string message);
    }

    public class ConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }

        public void LogError(string message)
        {
            Console.WriteLine($"ERROR: {message}");
        }
    }
}

