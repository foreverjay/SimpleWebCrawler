using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SimpleWebCrawler.Engine;
using SimpleWebCrawler.Engine.Entities;
using SimpleWebCrawler.Engine.Utilities;

namespace SimpleWebCrawler.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var urlsToProcess = new List<string>();

            //check the args 
            if (null != args)
            {
                foreach (var arg in args)
                {
                    Uri uri;
                    if (Uri.TryCreate(arg, UriKind.Absolute, out uri))
                    {
                        urlsToProcess.Add(uri.AbsoluteUri);
                    }
                }
            }

            if (urlsToProcess.Count == 0)
            {
                System.Console.WriteLine(string.Concat(
                    "No valid urls provided.\r\n",
                    "You need to pass in valid comma separated absolute uris as the input arguments.\r\n",
                    "Ending now ..."));
                System.Console.Read();
                return;
            }

            System.Console.WriteLine(string.Concat(
                    "Following urls has been identified for processing:\r\n",
                    string.Join(", ", urlsToProcess),
                    "\r\nPress any key to begin processing...\r\n",
                    "To terminate the program, press 'c' to cancel and exit...\r\n"));
            if (System.Console.ReadKey().KeyChar == 'c')
            {
                return;
            }
            System.Console.WriteLine();

            var timeStarted = DateTime.Now;
            System.Console.WriteLine(string.Format("Starting at: {0}", timeStarted.ToLongTimeString()));

            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            var task = Task<DistinctList<ParsedUrl>>.Factory.StartNew(() =>
            {
                var manager = new WebCrawlerManager
                {
                    MaxProcessingDepth = 4,
                    MaxUrlsToProcess = 1000,
                    ProcessOffsiteLinks = false,
                    CancellationToken = token
                };

                manager.NewLoopStarted += (sender, eventargs) => System.Console.WriteLine(
                        string.Concat("Entering depth: ", eventargs.ProcessingDepth));

                manager.UrlProcessed += (sender, eventargs) => System.Console.WriteLine(
                        string.Concat("Processed url: ", eventargs.Url));

                manager.UrlProcessingErrorOccured += (sender, eventargs) => System.Console.WriteLine(
                        string.Concat("Error ocurred on: ", eventargs.Url));

                return manager.Run(urlsToProcess);
            }, token
                );

            var continueTask = task.ContinueWith(antecedant =>
            {
                var processedUrls = antecedant.Result;
                System.Console.WriteLine("Processing finished.");
                System.Console.WriteLine(string.Format("{0} urls processed.", processedUrls.Count));

                //foreach (var processedUrl in processedUrls.Select(url=>url.Url))
                //{
                //    Console.WriteLine(processedUrl);
                //}

                System.Console.WriteLine(string.Format("{0} errors faced.", processedUrls.Count(url => url.HasError)));
                foreach (var processedUrl in processedUrls.Where(url => url.HasError))
                {
                    System.Console.WriteLine("{0} - {1}", processedUrl.Url, processedUrl.Error.Message);
                }

                //foreach (var item in processedUrls)
                //{
                //    Console.WriteLine("Completed url: " + item.Url + ". Found urls: " + string.Join(",", item.FoundUrls));
                //}      

                var timeFinished = DateTime.Now;
                System.Console.WriteLine(string.Format("Finished at: {0}", timeFinished.ToLongTimeString()));
                System.Console.WriteLine(string.Format("Duration: {0} sec", (timeFinished - timeStarted).Seconds));

                var successEntriesToLog = from processUrl in processedUrls
                                          where null != processUrl.FoundUrls
                                          from foundUrl in processUrl.FoundUrls
                                          select new { Url = processUrl.Url, FoundUrl = foundUrl };
                //FileLogger.Instance.LogInfo(successEntriesToLog.Select(url => string.Format("{0} -> {1}", url.Url, url.FoundUrl)));

                var errorEntriesToLog = from processedUrl in processedUrls
                                        where null != processedUrl.Error
                                        select new { Url = processedUrl.Url, Error = processedUrl.Error.Message };
                //FileLogger.Instance.LogError(errorEntriesToLog.Select(url => string.Format("{0} -> {1}", url.Url, url.Error)));


                System.Console.ReadLine();
            });

            // Request cancellation from the UI thread.  
            if (System.Console.ReadKey().KeyChar == 'c')
            {
                tokenSource.Cancel();
                System.Console.WriteLine("\nTask cancellation requested.");
                System.Console.ReadLine();
            }

        }              
    }
}
