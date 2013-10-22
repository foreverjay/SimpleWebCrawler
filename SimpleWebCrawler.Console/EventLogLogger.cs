using System.Diagnostics;

namespace SimpleWebCrawler.Console
{
    internal sealed class EventLogLogger
    {
        private const string LogEventSourceName = "WebCrawler";

        private EventLogLogger() {}

        private static EventLogLogger _instance = new EventLogLogger();

        public static EventLogLogger Instance
        {
            get { return _instance; }
        }

        public void LogError(string errorMessage)
        {
            if (!EventLog.SourceExists(LogEventSourceName))
                EventLog.CreateEventSource(LogEventSourceName, "Application");

            EventLog.WriteEntry(LogEventSourceName, errorMessage, EventLogEntryType.Error);
        }
    }
}
