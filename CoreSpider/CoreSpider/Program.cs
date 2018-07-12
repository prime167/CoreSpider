using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using AngleSharp.Parser.Html;
using Newtonsoft.Json;

namespace CoreSpider
{
    class Program
    {
        //static Timer timer = new Timer(Callback, null, 0, Timeout.Infinite);

        static async Task Main(string[] args)
        {
            Console.WriteLine("Get proxies...\r\n");

            var spider = new IpPoolSpider();
            spider.Initial();
            Console.WriteLine("Get proxies end...\r\n");

            Console.WriteLine("request...\r\n");
            var posts = new List<BlogPost>();
            int iIndex = 1;

            for (int pageIndex = 1; pageIndex <=0; pageIndex++)
            {
                var url = "https://www.cnblogs.com/mvc/AggSite/PostList.aspx";

                Console.WriteLine(url);
                var html = await GetResponse(url, pageIndex);
                var htmlParser = new HtmlParser();

                var document = htmlParser.Parse(html);

                var postDivs = document.All.Where(m => m.ClassName == "post_item");

                foreach (var div in postDivs)
                {
                    var post = new BlogPost
                    {
                        Title = div.QuerySelector("a.titlelnk").InnerHtml,
                        Url = div.QuerySelector("a.titlelnk").Attributes["href"].Value,
                        DiggNumber = Convert.ToInt32(div.QuerySelector("span.diggnum").InnerHtml),
                        Author = div.QuerySelector("div.post_item_foot").Children[0].InnerHtml
                    };

                    var foot = div.QuerySelector("div.post_item_foot").TextContent;
                    var items = foot.Split('\n');
                    for (var i = 0; i < items.Length; i++)
                    {
                        post.Author = items[1].Trim();
                        post.PostTime = DateTime.Parse(items[2].Trim().Substring(3));
                    }

                    Console.WriteLine($"{iIndex} {post.Title} {post.Url} {post.DiggNumber} {post.Author}  {post.PostTime}");
                    posts.Add(post);
                    iIndex++;
                }
            }

            Console.WriteLine();
            Console.WriteLine(posts.Count);
            Console.WriteLine("\r\nPress anykey to exit");
            Console.ReadLine();
        }

        private static async Task<string> GetResponse(string url, int pageIndex)
        {
            var proxy = PoolManageService.GetProxy();
            try
            {
                HttpClientHandler httpClientHandler = new HttpClientHandler
                {
                    UseProxy = true,
                    Proxy = new WebProxy(proxy, false),
                    PreAuthenticate = false,
                    UseDefaultCredentials = false,
                };

                var httpClient = new HttpClient(httpClientHandler)
                {
                    Timeout = TimeSpan.FromSeconds(5),
                };

                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.66 Safari/537.36");
                var ct = new { CategoryType = "Picked", ParentCategoryId = 0, CategoryId = -2, PageIndex = pageIndex, TotalPostCount = 4000, ItemListActionName = "PostList" };
                var stringContent = new StringContent(JsonConvert.SerializeObject(ct), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(url, stringContent).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();
                using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                using (var decompressedStream = new GZipStream(responseStream, CompressionMode.Decompress))
                using (var streamReader = new StreamReader(decompressedStream))
                {
                    var r = await streamReader.ReadToEndAsync().ConfigureAwait(false);
                    return r;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }
    }
}
