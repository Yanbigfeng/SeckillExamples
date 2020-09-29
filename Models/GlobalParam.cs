using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeckillExamples.Models
{
    /// <summary>
    /// 设置全局的静态变量
    /// </summary>
    public static class GlobalParam
    {


        //下单控制器使用-Order
        //线程安全的队列
        public static ConcurrentQueue<ModelQueueParam> queue = new ConcurrentQueue<ModelQueueParam>();
        public static ConcurrentQueue<int> queueNum = new ConcurrentQueue<int>();

    }
}
