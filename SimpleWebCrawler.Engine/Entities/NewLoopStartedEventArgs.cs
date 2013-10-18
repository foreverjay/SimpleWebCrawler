using System;

namespace SimpleWebCrawler.Engine.Entities
{
    public class NewLoopStartedEventArgs : EventArgs
    {
        public int ProcessingDepth { get; set; }
    }
}
