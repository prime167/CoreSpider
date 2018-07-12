using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreSpider
{
    public class RedisManageService
    {
        private static readonly string redisAddress = "192.168.22.123";

        /// <summary>
        /// 获取某set集合 随机一条数据
        /// </summary>
        /// <param name="setName"></param>
        /// <returns></returns>
        public static string GetRandomItemFromSet(string setName)
        {
            using (var client = new RedisClient(redisAddress, 6379))
            {
                var result = client.GetRandomItemFromSet(setName.ToString());
                if (result == null)
                {
                    throw new Exception("redis set集合" + setName.ToString() + "已无数据！");
                }

                return result;
            }
        }

        /// <summary>
        /// 从某set集合 删除指定数据
        /// </summary>
        /// <param name="setName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static void RemoveItemFromSet(string setName, string value)
        {
            using (RedisClient client = new RedisClient(redisAddress, 6379))
            {
                client.RemoveItemFromSet(setName.ToString(), value);
            }
        }

        /// <summary>
        /// 添加一条数据到某set集合
        /// </summary>
        /// <param name="setName"></param>
        /// <param name="value"></param>
        public static void AddItemToSet(string setName, string value)
        {
            using (RedisClient client = new RedisClient(redisAddress, 6379))
            {
                client.AddItemToSet(setName.ToString(), value);
            }
        }

        /// <summary>
        /// 添加一个列表到某set集合
        /// </summary>
        /// <param name="setName"></param>
        /// <param name="values"></param>
        public static void AddItemListToSet(string setName, List<string> values)
        {
            using (RedisClient client = new RedisClient(redisAddress, 6379))
            {
                client.AddRangeToSet(setName.ToString(), values);
            }
        }

        /// <summary>
        /// 判断某值是否已存在某set集合中
        /// </summary>
        /// <param name="setName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool JudgeItemInSet(string setName, string value)
        {
            using (RedisClient client = new RedisClient(redisAddress, 6379))
            {
                return client.Sets[setName.ToString()].Any(t => t == value);
            }
        }

        /// <summary>
        /// 获取某set数据总数
        /// </summary>
        /// <param name="setName"></param>
        /// <returns></returns>
        public static long GetSetCount(string setName)
        {
            using (RedisClient client = new RedisClient(redisAddress, 6379))
            {
                return client.GetSetCount(setName.ToString());
            }
        }
    }
}