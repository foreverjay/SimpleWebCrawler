using System;
using System.Collections.Generic;

namespace SimpleWebCrawler.Engine.Entities
{
    public class UrlProcessedEventArgs : EventArgs
    {
        public string Url { get; set; }
        public IEnumerable<string> FoundUrls { get; set; }
        public string HtmlContent { get; set; }
    }
}
