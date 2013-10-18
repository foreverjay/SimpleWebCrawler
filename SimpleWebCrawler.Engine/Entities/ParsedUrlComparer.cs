using System;
using System.Collections.Generic;

namespace SimpleWebCrawler.Engine.Entities
{    
    public class ParsedUrlComparer : EqualityComparer<ParsedUrl>
    {        
        public override bool Equals(ParsedUrl x, ParsedUrl y)
        {
            return x.Url.Equals(y.Url, StringComparison.CurrentCultureIgnoreCase);
        }

        public override int GetHashCode(ParsedUrl obj)
        {
            int hCode = obj.Url.GetHashCode() ^ obj.Url.GetHashCode();
            return hCode.GetHashCode();
        }
    }
}
