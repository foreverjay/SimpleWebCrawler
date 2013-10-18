using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace SimpleWebCrawler.Engine.Utilities
{
    internal class HtmlParser
    {        
        private HtmlParser() {}

        private static HtmlParser _instance = new HtmlParser();

        public static HtmlParser Instance { get { return _instance; } }

        public bool Parse(string html, out IEnumerable<string> urls)
        {
            urls = new List<string>();
            bool resultStatus = true;
            if (!string.IsNullOrEmpty(html))
            {
                var doc = new HtmlDocument();                
                try
                {
                    doc.LoadHtml(html);
                    var hrefNodes = doc.DocumentNode.SelectNodes("//a[@href]");
                    if (null != hrefNodes)
                    {
                        urls = (from node in hrefNodes
                                let href = node.Attributes["href"]
                                where null != href
                                      && !string.IsNullOrEmpty(href.Value)
                                      && !href.Value.Trim().StartsWith("#")
                                      && !href.Value.Trim().StartsWith("javascript")
                                select href.Value).ToList();
                    }
                }
                catch (Exception ex)
                {
                    EventLogLogger.Instance.LogError(ex.StackTrace);                    
                    resultStatus = false;
                }                                
            }
            
            return resultStatus;
        }        
    }
}
