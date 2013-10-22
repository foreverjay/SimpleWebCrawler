using System;

namespace SimpleWebCrawler.Engine.Entities
{
    public class UrlProcessingErrorOccuredEventArgs : EventArgs
    {
        public UrlProcessingErrorOccuredEventArgs()
        {
            ErrorInfo = new ErrorInfo();
        }

        public ErrorInfo ErrorInfo { get; set; }
    }
}
