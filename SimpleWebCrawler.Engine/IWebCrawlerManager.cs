using System;
using System.Collections.Generic;
using System.Threading;
using SimpleWebCrawler.Engine.Entities;

namespace SimpleWebCrawler.Engine
{
    /// <summary>
    /// interface for the crawling manager
    /// </summary>
    public interface IWebCrawlerManager
    {
        #region Configuration properties
        /// <summary>
        /// max iteration depth to process the urls
        /// </summary>
        int MaxProcessingDepth { get; set; }

        /// <summary>
        /// max number of urls to process
        /// </summary>
        int MaxUrlsToProcess { get; set; }

        /// <summary>
        /// determine whether to process offsite links
        /// </summary>
        bool ProcessOffsiteLinks { get; set; }

        CancellationToken CancellationToken { get; set; }
        #endregion

        /// <summary>
        /// run the actual crawling
        /// </summary>
        /// <param name="initialUrls"></param>
        /// <returns></returns>
        DistinctList<ParsedUrl> Run(IEnumerable<string> initialUrls);

        #region events
        /// <summary>
        /// notifies when a new iteration (processing depth) started
        /// </summary>
        event EventHandler<NewLoopStartedEventArgs> NewLoopStarted;

        /// <summary>
        /// notifies when a url has been processed
        /// </summary>
        event EventHandler<UrlProcessedEventArgs> UrlProcessed;

        /// <summary>
        /// notifies when an error has occured
        /// </summary>
        event EventHandler<UrlProcessingErrorOccuredEventArgs> UrlProcessingErrorOccured;
        #endregion
    }
}
