using AliLogToPostmanJson.AliLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace AliLogToPostmanJson
{
    public class RequestLog
    {
        public RequestLog()
        {
        }

        public RequestLog(LogModel log)
        {
            try
            {
                Referer = string.IsNullOrWhiteSpace(log.Http_referer) || log.Http_referer == "-" ||!log.Http_referer.StartsWith("http") ? null : new Uri(log.Http_referer);
                Method = log.Request_Method;
                Uri = new Uri($"{log.Scheme}://{log.Host}{log.Request_uri}");
            }
            catch
            {
                Console.WriteLine($"{log.Request_Method},{log.Http_referer},{log.Scheme},{log.Host},{log.Request_uri} ");
                throw;
            }
        }

        public RequestLog(string method,string referer,string host,string scheme, string pathAndQuery)
        {
            Referer = string.IsNullOrWhiteSpace(referer)?null:new Uri(referer);
            Method = method;
            Uri = new Uri($"{scheme}://{host}{pathAndQuery}");
        }
        public Uri Referer { set; get; }
        public string DirName {
            get {
                return Referer?.PathAndQuery??string.Empty;
            }
        }
        
        public Uri Uri { set; get; }
        public string Method { set; get; }
    }

    public class RequestLogEqualityComparer : IEqualityComparer<RequestLog>
    {
        public bool Equals(RequestLog x, RequestLog y)
        {
            if (x == null)
                return false;
            if (y == null)
                return false;

            if (x.Method != y.Method)
                return false;

            if (x.Referer == null && null == y.Referer)
            {
                return x.Uri.PathAndQuery == y.Uri.PathAndQuery;
            }
            else
            {
                if(x.Referer.PathAndQuery==y.Referer.PathAndQuery)
                {
                    return x.Uri.PathAndQuery == y.Uri.PathAndQuery;
                }
                return false;
            }
        }

        public int GetHashCode(RequestLog obj)
        {
            if (obj == null)
                return 0;

            int refererHashCode = obj.Referer == null?0:obj.Referer.PathAndQuery.GetHashCode();
            
            int hCode = obj.Method.GetHashCode() ^ obj.Uri.PathAndQuery.GetHashCode() ^ refererHashCode;
            return hCode.GetHashCode();
        }
    }
}
