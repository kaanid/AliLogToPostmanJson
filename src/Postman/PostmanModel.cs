using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AliLogToPostmanJson
{
    public class PostmanModel
    {
        public Info Info { set; get; }
        public List<Item> Item { set; get; }
        public List<Event> Event { set; get; }
    }

    public class Info
    {
        [JsonProperty("_postman_id")]
        public Guid PostmanId { set; get; }
        public string Name { set; get; }
        public string Schema => "https://schema.getpostman.com/json/collection/v2.1.0/collection.json";
    }

    public class Item
    {
        public string Name { set; get; }
        public List<Event> Event { set; get; }
    }

    public class ItemDir:Item
    {
        public List<Item> Item { set; get; } = new List<Item>();
    }

    public class ItemRequest : Item
    {
        public Request Request { set; get; }
        public object[] Response { set; get; }

    }

    public class Event
    {
        public string Listen { set; get; }
        public Script Script { set; get; }
    }

    public class Script
    {
        public Guid Id { set; get; }
        public List<string> Exec { set; get; }
        public string Type { set; get; }
    }

    public class UrlModel
    {
        public string Raw { set; get; }
        public List<string> Host { set; get; }
        public string[] Path { set; get; }
        public List<KeyValuePair<string, string>> Query { set; get; }
    }

    public class Request
    {
        public string Protocol { set; get; } = "http";
        public string Port { set; get; }
        public string Method { set; get; }
        public List<Header> Header { set; get; }
        public UrlModel Url { set; get; }
    }

    public class RequestGet : Request
    {
        public RequestGet()
        {
            Method = "GET";
        }
    }

    public class RequestPost : Request
    {
        public RequestPost()
        {
            Method = "POST";
            Header = new List<Header> {
                new Header
                {
                    Key="Content-Type",
                    Name="Content-Type",
                    Value="application/x-www-form-urlencoded",
                    Type="text"
                }
            };
        }
        public Body Body { set; get; }
    }

    public class Header
    {
        public string Key { set; get; }
        public string Name { set; get; }
        public string Value { set; get; }
        public string Type { set; get; }
    }

    public class Body
    {
        // urlencoded，raw
        public string Mode { set; get; }
    }
    public class BodyByUrlencoded : Body
    {
        public BodyByUrlencoded()
        {
            Mode = "urlencoded";
        }
        public List<BodyByUrlencodedItem> Urlencoded { set; get; }

    }

    public class BodyByUrlencodedItem
    {
        public string Key { set; get; }
        public string Value { set; get; }
        public string Type { set; get; }
    }
}
