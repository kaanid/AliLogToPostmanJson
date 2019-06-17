using System;
using System.Collections.Generic;
using System.Text;

namespace AliLogToPostmanJson.AliLog
{
    public class LogModel
    {
        public string Host { set; get; }
        public string Http_referer { set; get; }
        public string Request_Method { set; get; }
        public string Request_uri { set; get; }
        public string Scheme { set; get; }
    }
}
