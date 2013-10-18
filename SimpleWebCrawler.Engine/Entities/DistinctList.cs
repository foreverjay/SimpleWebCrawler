using System.Collections.Generic;
using System.Linq;

namespace SimpleWebCrawler.Engine.Entities
{    
    public class DistinctList<T> : List<T> where T : class
    {
        public EqualityComparer<T> EqualityComparer { get; set; } 

        public new void AddRange(IEnumerable<T> urlsToAdd)
        {
            base.AddRange(urlsToAdd.Except(this, EqualityComparer));
        }
    }    
}
