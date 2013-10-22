using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SimpleWebCrawler.Engine.Entities;
using SimpleWebCrawler.Engine.Utilities;

namespace SimpleWebCrawler.Engine
{
    public class WebCrawlerManager : IWebCrawlerManager
    {
        #region IWebCrawlerManager implementation

        #region configuration properties
        private int? _maxProcessingDepth;
        /// <summary>
        /// max iteration depth to process the urls
        /// </summary>
        public int MaxProcessingDepth
        {
            get
            {
                if (!_maxProcessingDepth.HasValue)
                {
                    _maxProcessingDepth = 4;
                }
                return _maxProcessingDepth.Value;
            }
            set { _maxProcessingDepth = value; }
        }

        private int? _maxUrlsToProcess;
        /// <summary>
        /// max number of urls to process
        /// </summary>
        public int MaxUrlsToProcess
        {
            get
            {
                if (!_maxUrlsToProcess.HasValue)
                {
                    _maxUrlsToProcess = 1000;
                }
                return _maxUrlsToProcess.Value;
            }
            set { _maxUrlsToProcess = value; }
        }

        private bool? _processOffsiteLinks;
        /// <summary>
        /// determine whether to process offsite links
        /// </summary>
        public bool ProcessOffsiteLinks
        {
            get
            {
                if (!_processOffsiteLinks.HasValue)
                {
                    _processOffsiteLinks = false;
                }
                return _processOffsiteLinks.Value;
            }
            set { _processOffsiteLinks = value; }
        }

        private CancellationToken? _cancellationToken;
        public CancellationToken CancellationToken
        {
            get
            {
                if (!_cancellationToken.HasValue)
                {
                    _cancellationToken = CancellationToken.None;
                }
                return _cancellationToken.Value;
            }
            set { _cancellationToken = value; }
        }

        #endregion

        /// <summary>
        /// starts the processing url
        /// </summary>
        /// <param name="initialUrls"></param>
        /// <returns></returns>
        public DistinctList<ParsedUrl> Run(IEnumerable<string> initialUrls)
        {
            var parsingManager = new ParsingManager(OnUrlProcessed);
            var processor = new ParallelProcessInvoker(parsingManager);            

            var processedUrls = new DistinctList<ParsedUrl> {EqualityComparer = new ParsedUrlComparer()};
            var urlsToProcess = initialUrls;

            var loopCounter = 1;
            while (loopCounter <= MaxProcessingDepth)
            {
                OnNewLoopStarted(loopCounter);

                var result = processor.Process(urlsToProcess,
                                               //(url, ct) => ProcessUrl((string) url, (CancellationToken)ct),
                                               CancellationToken,
                                               OnUrlProcessingErrorOccured);
                processedUrls.AddRange(result);

                //if cancellation has been requested, then stop processing
                if (CancellationToken.IsCancellationRequested)
                {
                    break;
                }

                var itemsToTake = MaxUrlsToProcess - processedUrls.Count;


                urlsToProcess = processedUrls                    
                    .SelectMany(url => url.FoundUrls)
                    .GetValidOnSiteUrls("hiring.monster.com") //TODO - remove the hardcoded value with a dynamic processing based on url
                    .Distinct()
                    .Except(processedUrls.Select(u=>u.Url))                    
                    .Take(itemsToTake);

                loopCounter++;
            }

            return processedUrls;
        }

        public event EventHandler<NewLoopStartedEventArgs> NewLoopStarted;
        public event EventHandler<UrlProcessedEventArgs> UrlProcessed;
        public event EventHandler<UrlProcessingErrorOccuredEventArgs> UrlProcessingErrorOccured;

        #endregion     

        #region firing the events
        private void OnNewLoopStarted(int processingDepth)
        {
            if (null != NewLoopStarted)
            {
                NewLoopStarted(this, new NewLoopStartedEventArgs {ProcessingDepth = processingDepth});
            }
        }

        private void OnUrlProcessed(string url, IEnumerable<string> foundUrls, string html)
        {
            if (null != UrlProcessed)
            {
                UrlProcessed(this, new UrlProcessedEventArgs {Url = url, FoundUrls = foundUrls, HtmlContent = html});
            }
        }

        private void OnUrlProcessingErrorOccured(ErrorInfo errorInfo)
        {
            if (null != UrlProcessingErrorOccured)
            {
                UrlProcessingErrorOccured(this, new UrlProcessingErrorOccuredEventArgs {ErrorInfo = errorInfo});
            }
        }
        #endregion
    }
}
