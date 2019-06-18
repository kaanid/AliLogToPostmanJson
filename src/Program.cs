using AliLogToPostmanJson.AliLog;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AliLogToPostmanJson
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            string aliLogPath = "C:\\downloaded_data.txt";

            var aliLogService = new AliLogService();
            var list = aliLogService.GetLogListByAliLog(aliLogPath);

            Console.WriteLine($"list count:{list.Count}");
            if (list.Count == 0)
                return;

            var listDistinct = list.Distinct(new RequestLogEqualityComparer()).ToList();
            Console.WriteLine($"listDistinct count:{listDistinct.Count}");

            var service = new PostManJson();
            var postmanJasonModel = service.Create("dp2");
            service.ToModel(postmanJasonModel, listDistinct);
            var filePath = service.SaveFile(postmanJasonModel);


            Console.WriteLine("completed");
            Console.Read();
        }

        
    }
}
