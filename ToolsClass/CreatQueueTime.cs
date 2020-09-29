using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SeckillExamples.EData;
using SeckillExamples.Models;

namespace SeckillExamples.ToolsClass
{
    public class CreatQueueTimeService : BackgroundService
    {

        //构造函数
        public ESHOPContext _context;
        public CreatQueueTimeService(ESHOPContext context)
        {
            _context = context;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            //启动自动创建--因为现在商品不一定固定id所有不使用改方法
            Timer timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(3));

        }

        //执行的任务
        private void DoWork(object state)
        {

        }

    }
}
