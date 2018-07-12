using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using NLog.Fluent;

namespace CoreSpider
{
    /// <summary>
    /// IP池 抓取蜘蛛
    /// </summary>
    public class IpPoolSpider
    {
        public void Initial()
        {
            //Downloadproxy360();
            // DownloadproxyBiGe();
            Downloadproxy66();
            //Task.Run(()=> { Downloadproxy360(); });
            //Task.Run(() => { DownloadproxyBiGe(); });
            //Task.Run(() => { Downloadproxy66(); });
            //Task.Run(() => { Downloadxicidaili(); });
        }

        // 下载西刺代理的html页面
        public void Downloadxicidaili()
        {
            try
            {
                List<string> list = new List<string>()
                {
                    "http://www.xicidaili.com/nt/",
                    "http://www.xicidaili.com/nn/",
                    "http://www.xicidaili.com/wn/",
                    "http://www.xicidaili.com/wt/"

                };
                foreach (var utlitem in list)
                {
                    for (int i = 1; i < 5; i++)
                    {
                        string url = utlitem + i;
                        var ipProxy = PoolManageService.GetProxy();
                        if (string.IsNullOrEmpty(ipProxy))
                        {
                            Log.Error().Message("Ip代理池暂无可用代理IP").Write();
                            return;
                        }
                        var ip = ipProxy;
                        WebProxy webproxy;
                        if (ipProxy.Contains(":"))
                        {
                            ip = ipProxy.Split(new[] { ':' })[0];
                            var port = int.Parse(ipProxy.Split(new[] { ':' })[1]);
                            webproxy = new WebProxy(ip, port);
                        }
                        else
                        {
                            webproxy = new WebProxy(ip);
                        }

                        string html = HttpHelper.DownloadHtml(url, webproxy);
                        if (string.IsNullOrEmpty(html))
                        {
                            Log.Error().Message("代理地址：" + url + " 访问失败").Write();
                            continue;
                        }

                        var doc = new HtmlDocument();
                        doc.LoadHtml(html);
                        HtmlNode node = doc.DocumentNode;
                        string xpathstring = "//tr[@class='odd']";
                        HtmlNodeCollection collection = node.SelectNodes(xpathstring);
                        foreach (var item in collection)
                        {
                            var proxy = new IpProxy();
                            string xpath = "td[2]";
                            proxy.Address = item.SelectSingleNode(xpath).InnerHtml;
                            xpath = "td[3]";
                            proxy.Port = int.Parse(item.SelectSingleNode(xpath).InnerHtml);
                            Task.Run(() =>
                            {
                                PoolManageService.Add(proxy);
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //LogUtils.ErrorLog(new Exception("下载西刺代理IP池出现故障", e));
            }
        }

        // 下载快代理
        public void Downkuaidaili()
        {
            try
            {
                string url = "https://www.kuaidaili.com/free/inha/";
                for (int i = 1; i < 4; i++)
                {
                    string html = HttpHelper.DownloadHtml(url + i + "/", null);
                    string xpath = "//tbody/tr";
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(html);
                    HtmlNode node = doc.DocumentNode;
                    HtmlNodeCollection collection = node.SelectNodes(xpath);
                    foreach (var item in collection)
                    {
                        var proxy = new IpProxy();
                        proxy.Address = item.FirstChild.InnerHtml;
                        xpath = "td[2]";
                        proxy.Port = int.Parse(item.SelectSingleNode(xpath).InnerHtml);
                        Task.Run(() =>
                        {
                            PoolManageService.Add(proxy);
                        });
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error().Message("下载快代理IP池出现故障", e).Write();
            }
        }

        // 下载proxy360
        public void Downloadproxy360()
        {
            try
            {
                string url = "http://www.proxy360.cn/default.aspx";
                string html = HttpHelper.DownloadHtml(url, null);
                if (string.IsNullOrEmpty(html))
                {
                    Log.Error().Message("代理地址：" + url + " 访问失败").Write();
                    return;
                }

                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                string xpathstring = "//div[@class='proxylistitem']";
                HtmlNode node = doc.DocumentNode;
                HtmlNodeCollection collection = node.SelectNodes(xpathstring);

                foreach (var item in collection)
                {
                    var proxy = new IpProxy();
                    var childnode = item.ChildNodes[1];
                    xpathstring = "span[1]";
                    proxy.Address = childnode.SelectSingleNode(xpathstring).InnerHtml.Trim();
                    xpathstring = "span[2]";
                    proxy.Port = int.Parse(childnode.SelectSingleNode(xpathstring).InnerHtml);
                    Task.Run(() =>
                    {
                        PoolManageService.Add(proxy);
                    });
                }
            }
            catch (Exception e)
            {
                Log.Error().Message("下载proxy360IP池出现故障", e).Write();
            }
        }

        // 下载逼格代理
        public void DownloadproxyBiGe()
        {
            try
            {
                List<string> list = new List<string>
                {
                    "http://www.bigdaili.com/dailiip/1/{0}.html",
                    "http://www.bigdaili.com/dailiip/2/{0}.html",
                    "http://www.bigdaili.com/dailiip/3/{0}.html",
                    "http://www.bigdaili.com/dailiip/4/{0}.html"
                };
                foreach (var utlitem in list)
                {
                    for (int i = 1; i < 5; i++)
                    {
                        string url = String.Format(utlitem, i);
                        string html = HttpHelper.DownloadHtml(url, null);
                        if (string.IsNullOrEmpty(html))
                        {
                            Log.Error().Message("代理地址：" + url + " 访问失败").Write();
                            continue;
                        }

                        var doc = new HtmlDocument();
                        doc.LoadHtml(html);
                        HtmlNode node = doc.DocumentNode;
                        string xpathstring = "//tbody/tr";
                        HtmlNodeCollection collection = node.SelectNodes(xpathstring);
                        foreach (var item in collection)
                        {
                            var proxy = new IpProxy();
                            var xpath = "td[1]";
                            proxy.Address = item.SelectSingleNode(xpath).InnerHtml;
                            xpath = "td[2]";
                            proxy.Port = int.Parse(item.SelectSingleNode(xpath).InnerHtml);
                            Task.Run(() =>
                            {
                                PoolManageService.Add(proxy);
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error().Message("下载逼格代理IP池出现故障", e).Write();
            }
        }

        // 下载66免费代理
        public void Downloadproxy66()
        {
            try
            {
                List<string> list = new List<string>
                {
                    "http://www.66ip.cn/areaindex_35/index.html",
                    "http://www.66ip.cn/areaindex_35/2.html",
                    "http://www.66ip.cn/areaindex_35/3.html"
                };

                foreach (var utlitem in list)
                {
                    string url = utlitem;
                    string html = HttpHelper.DownloadHtml(url, null);
                    if (string.IsNullOrEmpty(html))
                    {
                        Log.Error().Message("代理地址：" + url + " 访问失败").Write();
                        break;
                    }

                    var doc = new HtmlDocument();
                    doc.LoadHtml(html);
                    HtmlNode node = doc.DocumentNode;
                    string xpathstring = "//table[@bordercolor='#6699ff']/tr";
                    HtmlNodeCollection collection = node.SelectNodes(xpathstring);
                    foreach (var item in collection)
                    {
                        var proxy = new IpProxy();
                        var xpath = "td[1]";
                        proxy.Address = item.SelectSingleNode(xpath).InnerHtml;
                        if (proxy.Address.Contains("ip"))
                        {
                            continue;
                        }
                        xpath = "td[2]";
                        proxy.Port = int.Parse(item.SelectSingleNode(xpath).InnerHtml);
                        Task.Run(() =>
                        {
                            PoolManageService.Add(proxy);
                        });
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error().Message("下载66免费代理IP池出现故障"+e.Message).Write();
            }
        }
    }
}
