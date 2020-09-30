using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeckillExamples.ToolsClass
{
    public class RedisHelp
    {
        private string _connectionString; //连接字符串
        private int _defaultDB; //默认数据库
        private readonly ConnectionMultiplexer connectionMultiplexer;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="defaultDB">默认使用Redis的0库</param>
        public RedisHelp(string connectionString, int defaultDB = 0)
        {
            _connectionString = connectionString;
            _defaultDB = defaultDB;
            connectionMultiplexer = ConnectionMultiplexer.Connect(_connectionString);
        }

        /// <summary>
        /// 获取数据库
        /// </summary>
        /// <returns></returns>
        public IDatabase GetDatabase()
        {
            return connectionMultiplexer.GetDatabase(_defaultDB);
        }

    }
}
