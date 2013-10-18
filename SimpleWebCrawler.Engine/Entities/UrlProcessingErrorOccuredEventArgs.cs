using System;

namespace SimpleWebCrawler.Engine.Entities
{
    public class UrlProcessingErrorOccuredEventArgs : EventArgs
    {
        public string Url { get; set; }
        public string ErrorMessage { get; set; }
        
    }
}
