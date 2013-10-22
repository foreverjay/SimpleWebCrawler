using System;

namespace SimpleWebCrawler.Engine.Entities
{
    internal class ParsedUrlException : Exception
    {
        public ErrorInfo ErrorInfo { get; private set; }

        public ParsedUrlException(ErrorInfo errorInfo, Exception innerException) 
            : base(errorInfo.FriendlyMessage, innerException)
        {
            ErrorInfo = errorInfo;
        }
    }
}
