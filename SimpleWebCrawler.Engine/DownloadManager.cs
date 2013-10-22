using System;
using System.IO;
using System.Net;
using System.Text;
using SimpleWebCrawler.Engine.Entities;
using SimpleWebCrawler.Engine.Utilities;

namespace SimpleWebCrawler.Engine
{
    internal class DownloadManager
    {
        private readonly string _url;        

        public DownloadManager(string url)
        {
            _url = url;            
        }

        public string DownloadHtml()
        {            
            try
            {
                var webClient = new WebDownloader(120000);
                Byte[] pageData = webClient.DownloadData(_url);
                return Encoding.ASCII.GetString(pageData);
            }
            catch (WebException exception)
            {
                var response = GetResponse(exception);
                
                throw new ParsedUrlException(
                    new ErrorInfo {Url = _url, FriendlyMessage = exception.Message, ErrorMessage = response},
                    exception);   
             
                
            }            
        }        

        private string GetResponse(WebException exception)
        {            
            var logMessage = new StringBuilder();
            logMessage.AppendLine("********************************************************");
            logMessage.AppendLine("Error loading url: " + _url);
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
