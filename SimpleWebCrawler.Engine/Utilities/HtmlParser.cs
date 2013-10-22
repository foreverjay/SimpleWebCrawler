using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using SimpleWebCrawler.Engine.Entities;

namespace SimpleWebCrawler.Engine.Utilities
{
    internal class HtmlParser
    {        
        private HtmlParser() {}

        private static HtmlParser _instance = new HtmlParser();

        public static HtmlParser Instance { get { return _instance; } }

        public IEnumerable<string> Parse(string html)
        {
            var urls = new List<string>();
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
                    throw new ParsedUrlException(new ErrorInfo {FriendlyMessage = "Unable to parse html"}, ex);
                }
            }
            return urls;
        }        
    }
}
