using System;
using System.Net;

namespace SimpleWebCrawler.Engine.Utilities
{
    internal class WebDownloader : WebClient
    {
        private readonly CookieContainer _cookieContainer = new CookieContainer();

        //time in milliseconds        
        public int Timeout { get; set; }

        public WebDownloader() : this(60000) { }

        public WebDownloader(int timeout)
        {
            Timeout = timeout;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            var webRequest = request as HttpWebRequest;
            if (webRequest != null)
            {
                webRequest.CookieContainer = _cookieContainer;
                request.Timeout = Timeout;
            }
            
            return webRequest;
        }
    }    
}
