using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SimpleWebCrawler.Engine.Entities;

namespace SimpleWebCrawler.Engine
{
    internal class ParallelProcessInvoker
    {       
        public IEnumerable<ParsedUrl> Process(
            IEnumerable<string> urlsToParse, 
            Func<object, object, ParsedUrl> processMethod,
            CancellationToken ct,            
            Action<ErrorInfo> onErrorCallback)
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
            //apply one by one task handling
            while (tasks.Count > 0)
            {
                int index = 0;
                try
                {                     
                    index = Task.WaitAny(tasks.ToArray());

                    var completedTask = tasks[index];                    
                    result.Add(completedTask.Result);                    
                }                
                catch (AggregateException ae)
                {
                    var ex = ae.Flatten();
                    foreach (var innerException in ex.InnerExceptions)
                    {                        
                        if (innerException is TaskCanceledException)
                        {
                            onErrorCallback(new ErrorInfo {FriendlyMessage = "Task cancelled", ErrorMessage = innerException.Message});
                        }
                        else if (innerException is ParsedUrlException)
                        {
                            var parseUrlException = (ParsedUrlException) innerException;
                            var errorInfo = parseUrlException.ErrorInfo;
                            if (!string.IsNullOrEmpty(errorInfo.Url))
                            {
                                result.Add(new ParsedUrl
                                               {
                                                   Url = errorInfo.Url,
                                                   Error = new ErrorData {Message = errorInfo.FriendlyMessage}
                                               });
                            }
                            onErrorCallback(errorInfo);
                        }
                        else
                        {
                            var errorMessage = innerException.Message;                            
                            onErrorCallback(new ErrorInfo {FriendlyMessage = errorMessage, ErrorMessage = errorMessage});
                        }                        
                    }
                }
                catch (Exception ex)
                {
                    var errorMessage = ex.Message;                            
                    onErrorCallback(new ErrorInfo { FriendlyMessage = errorMessage, ErrorMessage = errorMessage });
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
