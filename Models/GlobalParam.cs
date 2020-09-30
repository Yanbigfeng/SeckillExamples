using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        public static ConcurrentQueue<int> queueNum = new ConcurrentQueue<int>();//进行记录初始化秒杀库存，记录模式
        public static int queueStockrNum = 0; //进行记录初始化秒杀库存-总值模式



       //后台秒杀中启用库存的时候才会使用
        //public static Thread _worker;
        //public static EventWaitHandle _wh = new AutoResetEvent(false);

    }
}
