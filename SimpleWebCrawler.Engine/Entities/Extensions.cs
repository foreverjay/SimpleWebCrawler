using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleWebCrawler.Engine.Entities
{
    public static class Extensions
    {
        public static IEnumerable<string> GetValidOnSiteUrls(this IEnumerable<string> urls, string domain)
        {
            return urls.Select(url => url.GetValidOnSiteUrl(domain)).Where(url => !string.IsNullOrEmpty(url));
        }

        public static string GetValidOnSiteUrl(this string url, string domain)
        {
            Uri absoluteUri = ConvertToAbsoluteUri(url, domain);
            if (null != absoluteUri && IsOnSiteLink(absoluteUri, domain))
            {
                return absoluteUri.AbsoluteUri;
            }
            return string.Empty;
        }

        private static bool IsOnSiteLink(Uri uri, string domain)
        {
            if (null != uri && uri.IsAbsoluteUri)
            {
                return uri.Host.Equals(domain, StringComparison.InvariantCultureIgnoreCase);
            }
            return false;
        }

        private static Uri ConvertToAbsoluteUri(string urlValue, string siteDomain)
        {
            Uri uri;
            if (Uri.TryCreate(urlValue, UriKind.Absolute, out uri) && uri.IsAbsoluteUri)
            {
                return uri;
            }

            if (urlValue.StartsWith("/"))
            {
                return new Uri(new Uri(string.Concat("http://", siteDomain)), urlValue);
            }

            return null;
        }
    }
}
