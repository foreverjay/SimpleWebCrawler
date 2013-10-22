using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using SimpleWebCrawler.Engine.Entities;
using SimpleWebCrawler.Engine.Utilities;

namespace SimpleWebCrawler.Engine
{
    internal class ParsingManager
    {
        private Action<string, IEnumerable<string>, string> _onUrlProcessed;

        public ParsingManager(Action<string, IEnumerable<string>, string> onUrlProcessed)
        {
            _onUrlProcessed = onUrlProcessed;
        }

        public ParsedUrl LoadAndParse(string url, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
            {
                ct.ThrowIfCancellationRequested();
            }
            
            try
            {
                var pageContent = DownloadHtml(url);

                var parsedUrls = HtmlParser.Parse(pageContent).ToList();

                _onUrlProcessed(url, parsedUrls, pageContent);

                return new ParsedUrl { Url = url, FoundUrls = parsedUrls };                            
            }
            catch (WebException exception)
            {
                var response = GetResponse(url, exception);

                throw new ParsedUrlException(
                    new ErrorInfo { Url = url, FriendlyMessage = exception.Message, ErrorMessage = response },
                    exception);

            }
            catch(Exception ex)
            {
                throw new ParsedUrlException(
                    new ErrorInfo {Url = url, FriendlyMessage = "Error parsing url", ErrorMessage = ex.Message}, ex);
            }                        
        }

        private string DownloadHtml(string url)
        {            
            var webClient = new WebDownloader(120000);
            Byte[] pageData = webClient.DownloadData(url);
            return Encoding.ASCII.GetString(pageData);                        
        }        

        private string GetResponse(string url, WebException exception)
        {            
            var logMessage = new StringBuilder();
            logMessage.AppendLine("********************************************************");
            logMessage.AppendLine("Error loading url: " + url);
            logMessage.AppendLine(exception.Message);
            if (exception.Response != null)
            {
                try
                {
                    var responseStream = exception.Response.GetResponseStream();

                    if (responseStream != null)
                    {
                        using (var reader = new StreamReader(responseStream))
                        {
                            string responseText = reader.ReadToEnd();
                            logMessage.AppendLine(responseText);
                        }
                    }
                }
                catch (Exception) {}
                                
            }

            return logMessage.ToString();
        }
    }
}
