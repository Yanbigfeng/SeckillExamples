using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeckillExamples.ToolsClass
{
    public class JsonHelp
    {
        #region 01-将JSON转换成JSON字符串
        /// <summary>
        ///将JSON转换成JSON字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJsonString<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        #endregion

        #region 02-将字符串转换成JSON对象
        /// <summary>
        /// 将字符串转换成JSON对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <returns></returns>
        public static T ToObject<T>(string content)
        {
            return JsonConvert.DeserializeObject<T>(content);
        }
        #endregion
    }
}
