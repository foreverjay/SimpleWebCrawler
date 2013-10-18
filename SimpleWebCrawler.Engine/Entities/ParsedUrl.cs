using System.Collections.Generic;

namespace SimpleWebCrawler.Engine.Entities
{
    public class ParsedUrl
    {        
        public ParsedUrl()
        {
            FoundUrls = new List<string>();            
        }

        public string Url { get; set; }
        public IEnumerable<string> FoundUrls { get; set; }
        public ErrorData Error { get; set; }

        public bool HasError { get { return null != Error; } }        
    }

    public class ErrorData
    {
        public string Message { get; set; }
        public int ExceptionType { get; set; }
    }
}