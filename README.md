# Funda Crawler

Sample solution for the Funda interview process. Implemented as a console app for brevity.

Contents
========

 * [Problem](#problem)
 * [Assumptions](#assumptions)
 * [Structure](#structure)
 * [Observations](#observations)
 * [General approach](#general-approach)
 * [Abstractions](#abstractions)
 * [Next Steps](#next-steps)

### Problem
---

The goal is to retrieve 2 sets of listings, based on the following criteria:
- all the listings in Amsterdam
- all the listings in Amsterdam with a garden (tuin).

Relevant information:
- might get throttled by the backend if doing more than 100 requests per minute (doesn't always happen, seems like a soft-ish limit).

Processing the information in the required format is trivial once we have all the data (i.e. grouping by the posting agency and sorting).


### Assumptions
---

- The implementation should balance execution speed (a business constraint normally) with code simplicity

- Based on the kind of data (real estate listings), we are assuming the data itself will not vary greatly over the course of the execution. That is, if the initial request says there are 120 pages of results, there won't be significantly more (or less) before we finish executing (e.g. within the next minute or two). 


### Structure
---

The `Funda.Crawler` project contains all the logic, while `Funda.Crawler.Tests` cotains some testing examples. The tests are not exhaustive by any measure, they are just intended to hightlight the decoupling and testability of the components.


### Observations
---

The API we're allowed to use does take an argument for page size, which seems to be hard capped at 25. Passing in any larger value does not increase the number of results, but it returns a wrong results for the number of pages. 
If the page size would have allowed greater values, the best solution would have been to get everything in one go.

Since we're trying to get the entire set, asking for smaller page size does not make sense. Thus, we are limited to retrieving 25 results per page. 

At the time of writing, the first problem (getting all the listing in Amsterdam) returns slightly over 2500 results, which means there is a good chance we will get throttled when getting the set for the first part of the problem. 

The second part returns < 30 pages, so throttling will not be an issue (if run in isloation, e.g. not right after the first part). 

Regarding the 2nd part of the problem, my initial suggestion would have been to simply process the relevant subset of listings that were retrieved in part 1. However, I could not find the relevant field to filter on in memory (no idea if any of the API fields correspond to the Tuin attribute). 

### General approach
---

Most of the time will be spent waiting for the http calls to return, so our application is I/O bound. Deserializing and processing a few thousand results will not pose any issues to a modern CPU. 

#### Approach 1: Sending the requests serially
---

*Rough execution time*: 20s for getting all the listings in Amsterdam, over 115 requests (when not throttled).

If the requests would be throttled after 100 with a 60s cooling period, most of the time would be spent waiting to be let in again. We'd probably spend approx 15s sending the first 100, 45s waiting, and another 5s to finish, so an estimate of 65s

**Pros**

- simpler to implement and reason about
- no need for an additional layer that distributes work and  for concurrent data structures / merging the results
- almost as fast as a parallel approach when getting throttled, at least for the current pattern (thigs would difer if requests would take longer etc).


**Cons**

- will take significantly longer to get all the results when not getting throttled at all (for part 2 of the problem for example)


#### Approach 2: Sending the requests in parallel
---

Using 5 as the degree of parallelism:

*Rough execution time*: 
- 5s when not getting throttled, significantly faster than the serial algorithm. Roughly 200ms per request, similar to the initial approach.
- 75s when throttled: the pattern is similar to the serial approach - we burst through our available quota faster, but still need to wait. As the number of pages is just slightly higher than the throttling limit, the remainder are done faster, but it has a small impact on the overall time.

**Pros**

- faster in some scenarios

**Cons**

- needs an additional layer to distribute the work and aggregate the results


### Abstractions
---

These should be self evident from the code itself, but adding them here to speed up the understanding process:

- Program.cs - does the wiring for the DI container and executes the agent twice, one for each of the URLs we are crawling
- AgentFinder.cs
	- connects the different services together
	- gets the results (while timing the operation), and instructs the formatter to display them
- CrawlerScheduler.cs
	- determines the number of total pages to be queried
	- creates a variable numeber of crawlers, up to the degree of parallelism configured
	- distributes the urls evenly between the crawlers
	- aggregates the results
- Crawler.cs
	- takes a list of URLs and queries them sequentially
	- returns a list of unique listings
- RequestService.cs
	- handles the http call aspects and interpreting the response (deserializing, checking for success etc.)
	- uses an IWatingService for backoffs
- IWaitingService
	- a strategy for waiting between requests;
	- the proposed one is an exponential backoff, but there are other numerous approaches (e.g. wait a fixed amount of seconds between each request).
	- other strategies might be more optimal for our problem, but they can be easily swapped in if benchmarking shows they are better


### Next steps
---

Further improvement suggestions:
- right now, each crawler has its own backoff when a request fails. An improvement would be to have a centralized value that would tell us if we're being throttled. This should be similar to a task (perhaps build from a TaskCompletionSource) that could be awaited before doing a request. 
	- If we're not throttled at the moment, the task would simply return immediately, allowing the request to go through
	- otherwise, the crawler that detects we are throttled would change this task until it can successfully get a request through
- tests should obviously cover all components, much more rigurously than they do right now
	
API improvement suggestions:
- If the specified page size is not supported |(i.e. > 25), the API should either:
	- return a 400 Bad Request, with a relevant message
	- return the same number of max results as it does today, but it should return correct information related to the number of pages (e.g. if we have 1000 records and the pageSize argument sent is 100, it should tell the caller that the current page size is 25, and that there are 40 pages instead of 10). 
- when throttling a user, it would be useful to also return the next time when a request would be successful. This would remove the need for guessing from the client side, and would reduce the load on the server.