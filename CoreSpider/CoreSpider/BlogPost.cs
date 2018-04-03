using System;
using System.Collections.Generic;
using System.Text;

namespace CoreSpider
{
    public class BlogPost
    {
        public string Url { get; set; }

        public string Title { get; set; }

        public int DiggNumber { get; set; }

        public string Author { get; set; }

        public DateTime PostTime { get; set; }
    }
}
