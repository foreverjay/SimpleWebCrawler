using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace SimpleWebCrawler.Engine.Utilities
{
    internal class HtmlParser
    {                
        public static IEnumerable<string> Parse(string html)
        {
            var urls = new List<string>();
            if (!string.IsNullOrEmpty(html))
            {
                var doc = new HtmlDocument();
                
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
            return urls;
        }        
    }
}
