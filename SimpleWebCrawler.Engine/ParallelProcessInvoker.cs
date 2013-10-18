using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SimpleWebCrawler.Engine.Entities;
using SimpleWebCrawler.Engine.Utilities;

namespace SimpleWebCrawler.Engine
{
    internal class ParallelProcessInvoker
    {       
        public IEnumerable<ParsedUrl> Process(
            IEnumerable<string> urlsToParse, 
            Func<object, object, ParsedUrl> processMethod,
            CancellationToken ct)
        {
            if (null == urlsToParse)
                throw new ArgumentException("urlsToParse cannot be null");

            var tasks = new List<Task<ParsedUrl>>();            
            foreach (var urlToParse in urlsToParse)
            {
                string url = urlToParse;
                var task = Task<ParsedUrl>.Factory.StartNew(o => processMethod(url, ct), url, ct);
                tasks.Add(task);
            }


            var result = new List<ParsedUrl>();

            //store the task results in a collection
            while (tasks.Count > 0)
            {
                int index = 0;
                try
                {                     
                    index = Task.WaitAny(tasks.ToArray());

                    var completedTask = tasks[index];
                    result.Add(completedTask.Result);                    
                }
                //TODO - notify about the error
                catch (AggregateException ae)
                {
                    var ex = ae.Flatten();
                    foreach (var innerException in ex.InnerExceptions)
                    {
                        //Console.WriteLine(innerException.Message);
                        if (innerException is TaskCanceledException)
                        {
                            //todo - do we need to do anything here
                        }
                        else
                        {
                            EventLogLogger.Instance.LogError(innerException.Message);
                        }                        
                    }
                }
                catch (Exception ex)
                {
                    EventLogLogger.Instance.LogError(ex.Message);
                    //Console.WriteLine(ex.Message);
                }
                finally
                {
                    tasks.RemoveAt(index);
                }

            }

            return result;
        }
    }
}
