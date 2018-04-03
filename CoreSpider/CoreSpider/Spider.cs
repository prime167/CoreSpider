using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace CoreSpider
{
    public class Spider
    {
        public static readonly HttpClient HttpClient;

        public WebProxy Proxy { get; set; }

        public RequestMethod Method { get; set; }

        public HttpRequestHeader Header { get; set; }

        public HttpContent PostContent { get; set; }
    }
}
