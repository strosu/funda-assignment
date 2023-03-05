using Funda.Crawler.Models;
using Moq;

namespace Funda.Crawler.Tests
{
    /// <summary>
    /// Hides some ugly stuff that we only need to implement once
    /// </summary>
    internal static class TimedOperationsMockExtensions
    {
        /// <summary>
        /// Configures the timed operation mock to be a passthrough, i.e. to just execute the function that it takes as an argument
        /// </summary>
        /// <param name="mock"></param>
        public static void ConfigureAsPassthrough(this Mock<ITimedOperation> mock)
        {
            mock.Setup(x => x.TimeOperationAsync(
                It.IsAny<Func<Task<IEnumerable<Listing>>>>()))
                .Returns<Func<Task<IEnumerable<Listing>>>>(x =>
             {
                 return x();
             });
        }
    }
}
