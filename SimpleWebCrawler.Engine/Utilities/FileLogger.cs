using System;
using System.Collections.Generic;
using System.IO;

namespace SimpleWebCrawler.Engine.Utilities
{
    internal sealed class FileLogger
    {
        private const string DirPath = @"d:\logs\webcrawler";

        private const string ErrorFileNameRoot = "errors";
        private const string InfoFileNameRoot = "info";

        private readonly string _errorFileName;
        private readonly string _infoFileName;

        private static readonly Object InfoLock = new Object();
        private static readonly Object ErrorLock = new Object();

        private static readonly FileLogger _instance = new FileLogger();        

        private FileLogger()
        {            
            if (!Directory.Exists(DirPath))
            {
                Directory.CreateDirectory(DirPath);
            }

            _errorFileName = Path.Combine(DirPath,
                                          string.Format("{0}_{1}.txt", ErrorFileNameRoot,
                                                        DateTime.Now.ToString("yyy-M-dd-H-mm-ss")));
            _infoFileName = Path.Combine(DirPath,
                                         string.Format("{0}_{1}.txt", InfoFileNameRoot,
                                                        DateTime.Now.ToString("yyy-M-dd-H-mm-ss")));
        }

        public static FileLogger Instance
        {
            get { return _instance; }
        }

        public void LogInfo(string text)
        {
            lock (InfoLock)
            {                
                File.AppendAllText(_infoFileName, string.Concat("\r\n", text));
            }
        }

        public void LogInfo(IEnumerable<string> text)
        {
            lock (InfoLock)
            {
                File.AppendAllLines(_infoFileName, text);
            }
        }        

        public void LogError(string text)
        {
            lock (ErrorLock)
            {
                File.AppendAllText(_errorFileName, string.Concat("\r\n", text));
            }
        }

        public void LogError(IEnumerable<string> text)
        {
            lock (ErrorLock)
            {
                File.AppendAllLines(_errorFileName, text);
            }
        }
    }
}
