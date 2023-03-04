using System;
using Funda.Crawler.Models;

namespace Funda.Crawler.Extensions
{
    public static class ResultListExtensions
    {
        public static bool IsLastPage(this ResultList result)
        {
            if (result == null)
            {
                return true;
            }

            return result.Paging.IsLastPage();
        }

        static bool IsLastPage(this PageInformation pageInfo)
        {
            return string.IsNullOrEmpty(pageInfo?.NextPageUrl);
        }
    }
}

