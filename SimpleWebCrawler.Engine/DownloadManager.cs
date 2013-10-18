using System;
using System.IO;
using System.Net;
using System.Text;
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

        public void DownloadHtml()
        {            
            try
            {
                var webClient = new WebDownloader(120000);
                Byte[] pageData = webClient.DownloadData(_url);
                DownloadedContent = Encoding.ASCII.GetString(pageData);
            }
            catch (WebException exception)
            {
         
                HasError = true;
                ErrorMessage = exception.Message;
                
                LogException(exception);
            }            
        }

        public bool HasError { get; private set; }

        public string ErrorMessage { get; private set; }

        public string DownloadedContent { get; private set; }

        private void LogException(WebException exception)
        {            
            var logMessage = new StringBuilder();
            logMessage.AppendLine("********************************************************");
            logMessage.AppendLine("Error loading url: " + _url);
            logMessage.AppendLine(exception.Message);
            if (exception.Response != null)
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

            FileLogger.Instance.LogError(logMessage.ToString());
        }
    }
}
