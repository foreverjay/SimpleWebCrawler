namespace SimpleWebCrawler.Engine.Entities
{
    public struct ErrorInfo
    {
        public string Url { get; set; }
        public string FriendlyMessage { get; set; }
        public string ErrorMessage { get; set; }
    }
}
