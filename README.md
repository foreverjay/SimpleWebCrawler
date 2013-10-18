SimpleWebCrawler
================

This tool does 2 things:
1. make http request to get the response in html
2. parse the html and searches for the url links

These 2 steps are repeated for the found url links until it reaches its limits which are defined as:
1. url processing depth - how deep to crawl; e.g. depth == 2 means the initial link(s) is loaded/parsed + the links found 
on the initil link(s) are loaded/parsed
2. max processing url - limit for the number of http requests

The solution is split into 2 projects:
1. the engine that does the actual crawling
2. the console application that serves as an example of how to handle the engine API

Engine features
The engine makes use of the TPL library (.net tasks parallel library) - for each job (load/parse) a new task is created.
The engine exposes the following API:
1. events for:
 - entering another processing depth
 - completion of loading/parsing an url (along with the html, url and found url links)
 - error events (along with url and error message)
2. final result set of the parsed urls and found url links
3. ability to cancel the processing via a cancellation token

Areas of use

