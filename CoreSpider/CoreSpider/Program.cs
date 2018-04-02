using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Parser.Html;

namespace CoreSpider
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var html = await GetResponse("https://www.cnblogs.com/pick/");
            var htmlParser = new HtmlParser();
            
            //加载HTML
            var document = htmlParser.Parse(html);

            var titleItemList = document.All.Where(m => m.ClassName == "titlelnk");
            var diggs = document.All.Where(m => m.ClassName == "diggnum");

            var numbersAndWords = titleItemList.Zip(diggs, (n,d) => new {Title = n, Diggs = d });
            
            int iIndex = 1;
            foreach (var nw in numbersAndWords)
            {
                Console.WriteLine($"{iIndex}:{nw.Title.InnerHtml}-{nw.Diggs.InnerHtml}");
                iIndex++;
            }

            Console.ReadLine();
        }

        private static async Task<string> GetResponse(string url)
        {
            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.66 Safari/537.36");

            var response = await httpClient.GetAsync(new Uri(url)).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            using (var decompressedStream = new GZipStream(responseStream, CompressionMode.Decompress))
            using (var streamReader = new StreamReader(decompressedStream))
            {
                var r = await streamReader.ReadToEndAsync().ConfigureAwait(false);
                return r;
            }
        }
    }
}
