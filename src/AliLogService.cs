using AliLogToPostmanJson.AliLog;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AliLogToPostmanJson
{
    public class AliLogService
    {
        public List<RequestLog> GetLogListByAliLog(string logPath)
        {
            var list = new List<RequestLog>();

            FileInfo file = new FileInfo(logPath);
            var starmReader = file.OpenText();

            int excludeCount = 0;
            int uriEmptyCount = 0;
            int uriLoginCount = 0;

            string str = string.Empty;
            do
            {
                str = starmReader.ReadLine();
                if (string.IsNullOrWhiteSpace(str))
                {
                    break;
                }

                LogModel log = JsonConvert.DeserializeObject<LogModel>(str);
                if (log == null)
                    continue;

                var currentUri = log.Request_uri;
                if (string.IsNullOrWhiteSpace(currentUri))
                {
                    uriEmptyCount++;
                    continue;
                }

                if (currentUri.Contains("/login.html?") || (log.Http_referer?.Contains("/login.html") ?? false))
                {
                    uriLoginCount++;
                    continue;
                }

                if (!currentUri.Contains("usermanage") && !currentUri.Contains(".ashx"))
                {
                    Console.WriteLine("exclude:" + currentUri);
                    excludeCount++;
                    continue;
                }

                var model = new RequestLog(log);
                list.Add(model);

                //if (list.Count > 10000)
                //    break;
            }
            while (true);

            starmReader.Close();
            starmReader = null;

            Console.WriteLine($"excludeCount:{excludeCount}");
            Console.WriteLine($"uriEmptyCount:{uriEmptyCount}");
            Console.WriteLine($"uriLoginCount:{uriLoginCount}");

            return list;
        }
    }
}
