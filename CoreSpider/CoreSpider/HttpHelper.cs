using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Text;

namespace CoreSpider
{
    internal class HttpHelper
    {
        /// <summary>
        /// 检验某代理IP当前是否可用
        /// 以百度为参考
        /// </summary>
        /// <param name="ipAddress">IP地址</param>
        /// <param name="port">端口</param>
        /// <returns></returns>
        public static bool IsAvailable(string ipAddress, int port)
        {
            bool result = false;
            try
            {
                WebProxy webproxy = new WebProxy(ipAddress, port);
                string html = DownloadHtml("https://www.baidu.com/", webproxy);
                if (html.Contains("百度一下，你就知道"))
                {
                    result = true;
                }
            }
            catch
            {

            }
            return result;
        }

        /// <summary>
        /// 下载html
        /// </summary>
        /// <param name="url">访问地址</param>
        /// <param name="proxy">代理地址</param>
        /// <returns></returns>
        public static string DownloadHtml(string url, WebProxy proxy)
        {
            var httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(5),
            };

            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.66 Safari/537.36");
            var response = AsyncHelper.RunSync(() =>httpClient.GetAsync(url));

            response.EnsureSuccessStatusCode();
            using (var responseStream = AsyncHelper.RunSync(() => response.Content.ReadAsStreamAsync()))
            using (var decompressedStream = new GZipStream(responseStream, CompressionMode.Decompress))
            using (var streamReader = new StreamReader(decompressedStream))
            {
                var source = AsyncHelper.RunSync(() => streamReader.ReadToEndAsync());
                return source;
            }
        }
    }
}