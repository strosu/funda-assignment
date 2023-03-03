# Funda Crawler

Sample solution for the Funda interview process. Implemented as console app for simplicity.

Contents
========

 * [Problem](#problem)
 * [Assumptions](#assumptions)
 * [Structure](#structure)
 * [Observations](#observations)
 * [General approach](#general-approach)

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

- Based on the kind of data (real estate listings), we are assuming the data itself will not vary greatly over the course of the execution. That is, if the initial request sais there are 120 pages of results, there won't be significantly more (or less) before we finish executing (e.g. within the next minute or 2). 


### Structure
---

The `Funda.Crawler` project contains all the logic, while `Funda.Crawler.Tests` cotains some testing examples. The tests are not exhaustive by any measure, they are just intended to hightlight the decoupling and testability of the components.


### Observations
---

The API we're allowed to use does take an argument for page size, which seems to be hard capped at 25. Passing in any larger value does not increase the number of results, but it returns wrong results for the number of pages. 

Since we're trying to get the entire set, asking for smaller page size does not make sense. Thus, we are limited to retrieving 25 results per page. 

At the time of writing, the first problem (getting all the listing in Amsterdam) returns slightly over 2500 results, which means there is a good chance we will get throttled when getting the set for the first part of the problem. 

The second part returns < 30 pages, so throttling will not be an issue (if run in isloation, e.g. not right after the first part). 

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

- will take significantly longer to get all the results when not getting throttled at all (for part of the problem for example)


#### Approach 2: Sending the requests in parallel
---


