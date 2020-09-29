using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeckillExamples.Models
{
    public static class GlobalParam
    {
        //线程安全的队列
        public static ConcurrentQueue<ModelQueueParam> queue = new ConcurrentQueue<ModelQueueParam>();
        public static ConcurrentQueue<int> queueNum = new ConcurrentQueue<int>();

    }
}
