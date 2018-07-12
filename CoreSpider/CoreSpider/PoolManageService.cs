using System;

namespace CoreSpider
{
    public class PoolManageService
    {
        /// <summary>
        /// 从代理池随机获取一条代理
        /// </summary>
        /// <returns></returns>
        public static string GetProxy()
        {
            string result = string.Empty;

            //try
            {
                result = RedisManageService.GetRandomItemFromSet("proxypool");
                if (result != null)
                {
                    if (
                        !HttpHelper.IsAvailable(result.Split(new[] { ':' })[0],
                            int.Parse(result.Split(new[] { ':' })[1])))
                    {
                        DeleteProxy(result);
                        return GetProxy();
                    }
                }
            }
            //catch (Exception e)
            {
                //LogUtils.ErrorLog(new Exception("从代理池获取代理数据出错", e));
            }

            return result;
        }

        /// <summary>
        /// 从代理池删除一条代理
        /// </summary>
        /// <param name="value"></param>
        public static void DeleteProxy(string value)
        {
            //try
            {
                RedisManageService.RemoveItemFromSet("proxypool", value);
            }
            //catch (Exception e)
            {
                //LogUtils.ErrorLog(new Exception("从代理池删除代理数据出错", e));
            }
        }

        /// <summary>
        /// 添加一条代理到代理池
        /// </summary>
        /// <param name="proxy"></param>
        public static void Add(IpProxy proxy)
        {
            //try
            {
                if (HttpHelper.IsAvailable(proxy.Address, proxy.Port))
                {
                    RedisManageService.AddItemToSet("proxypool", proxy.Address + ":" + proxy.Port.ToString());
                }
            }
            //catch (Exception e)
            {
                //LogUtils.ErrorLog(new Exception("添加一条代理数据到代理池出错", e));
            }
        }
    }
}