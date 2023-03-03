using System;
using Funda.Crawler.Models;

namespace Funda.Crawler.Extensions
{
    public static class ResultListExtensions
    {
        public static bool IsLastPage(this ResultList result)
        {
            return string.IsNullOrEmpty(result?.Paging?.NextPageUrl);
        }
    }
}

