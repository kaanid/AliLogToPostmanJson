using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AliLogToPostmanJson
{
    public class PostManJson
    {
        public PostmanModel Create(string name)
        {
            var model = new PostmanModel();

            model.Info = new Info
            {
                Name = name,
                PostmanId = Guid.NewGuid()
            };
            model.Item = new List<Item>();

            return model;
        }
        public string SaveFile(PostmanModel model)
        {
            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Guid.NewGuid().ToString("N") + ".json");
            File.AppendAllText(path, JsonConvert.SerializeObject(model, settings));

            return path;
        }

        public void ToModel(PostmanModel postmanJasonModel,List<RequestLog> listDistinct)
        {
            int getCount = 0;
            int postCount = 0;

            Dictionary<string, string> dictWhatDo = null;
            var @event = new Event
            {
                Listen = "test",
                Script = new Script
                {
                    Id = Guid.NewGuid(),
                    Type = "text/javascript",
                    Exec = new List<string>
                                    {
                                        "pm.test(\"response is ok\", function () {\r",
                                        "    pm.response.to.have.status(200);\r",
                                        "});\r",
                                        "\r",
                                        "pm.test(\"Succeed is true\", function () {\r",
                                        "    var jsonData = pm.response.json();\r",
                                        "    pm.expect(jsonData.succeed).to.eql(true)\r",
                                        "});\r",
                                        "\r",
                                        "pm.test(\"Data is exist\", function () {\r",
                                        "    var jsonData = pm.response.json();\r",
                                        "    pm.expect(jsonData.state).to.eql(0)\r",
                                        "});"
                                    }
                }
            };
            postmanJasonModel.Event = new List<Event> { @event };


            var list2 = listDistinct.GroupBy(m => m.DirName);
            foreach (var m in list2)
            {
                Console.WriteLine($"m Name:{m.Key} list count:{m.Count()}");
                var dir1 = new ItemDir { Name = m.Key };

                dictWhatDo = new Dictionary<string, string>();
                foreach (var m2 in m)
                {
                    var query = GetQuery(m2.Uri);
                    if (query != null)
                    {
                        var kv = query.FirstOrDefault(m6 => m6.Key == "whatDo");
                        if (kv.Key != null)
                        {
                            if (dictWhatDo.ContainsKey(kv.Value))
                                continue;

                            dictWhatDo[kv.Value] = string.Empty;
                        }

                    }

                    ItemRequest item = null;
                    if (m2.Method == "GET")
                    {
                        getCount++;
                        item = new ItemRequest
                        {
                            Name = m2.Uri.PathAndQuery,
                            Request = new RequestGet
                            {
                                Url = new UrlModel
                                {
                                    Raw = "{{DPHOST}}" + m2.Uri.LocalPath,
                                    Host = new List<string> { "{{DPHOST}}" },
                                    Path = m2.Uri.LocalPath.Split('/',StringSplitOptions.RemoveEmptyEntries),
                                    Query = query
                                }
                            },
                            Event = new List<Event> { @event }
                        };
                    }
                    else
                    {
                        postCount++;
                        item = new ItemRequest
                        {
                            Name = m2.Uri.PathAndQuery,
                            Request = new RequestPost
                            {
                                Url = new UrlModel
                                {
                                    Raw = "{{DPHOST}}" + m2.Uri.LocalPath,
                                    Host = new List<string> { "{{DPHOST}}" },
                                    Path = m2.Uri.LocalPath.Split('/', StringSplitOptions.RemoveEmptyEntries),
                                    Query = query
                                },
                                Body = new BodyByUrlencoded
                                {
                                    Urlencoded = new List<BodyByUrlencodedItem>
                                    {

                                    }
                                }
                            },
                        };
                    }

                    @event.Script.Id = Guid.NewGuid();
                    item.Event = new List<Event> { };
                    dir1.Item.Add(item);
                }

                postmanJasonModel.Item.Add(dir1);
            }
        }
        private List<KeyValuePair<string, string>> GetQuery(Uri uri)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(uri.Query))
                {
                    List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
                    var arrQuery = uri.Query.TrimStart('?').Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var str in arrQuery)
                    {
                        var arr = str.Split('=');
                        var k = arr[0];
                        string v = arr.Length > 1 ? arr[1] : string.Empty;

                        list.Add(new KeyValuePair<string, string>(k, v));
                    }
                    return list;
                }
                return null;
            }
            catch
            {
                Console.WriteLine(uri);
                throw;
            }
        }
        
    }
}
