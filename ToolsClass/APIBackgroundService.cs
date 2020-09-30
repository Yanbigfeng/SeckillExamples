using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SeckillExamples.EData;
using SeckillExamples.Models;
using Microsoft.EntityFrameworkCore;
using SeckillExamples.Models.Order;

namespace SeckillExamples.ToolsClass
{
    /// <summary>
    /// API中使用的启动后台任务
    /// </summary>
    public class APIBackgroundService : BackgroundService
    {

        /// <summary>
        /// 后台启动任务主要函数
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            if (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    //需要执行的任务
                    System.Diagnostics.Debug.WriteLine("后台任务运行中");

                    //resetStock();


                }
                catch (Exception ex)
                {


                }
            }

        }

        #region 秒杀库存重置-暂时不用
        /// <summary>
        /// 直接启动后重置秒杀库存
        /// 暂时不用
        /// </summary>
        private void resetStock()
        {
            //OrderMethod method = new OrderMethod();
            //GlobalParam._worker = new Thread(method.DeEnqueue);
            //GlobalParam._worker.Start();
            //string stq = "Server=47.92.198.126;Database=ESHOP;user id=eshop;password=eshop123456;MultipleActiveResultSets=true";
            //var optionsBuilder = new DbContextOptionsBuilder<ESHOPContext>();
            //optionsBuilder.UseSqlServer(stq);
            //using (var db = new ESHOPContext(optionsBuilder.Options))
            //{
            //    //查询当前是否还存在库存
            //    var articleInfo = db.TSeckillArticle.FirstOrDefault(u => u.Id == "y01");
            //    int articleStockNum = articleInfo?.ArticleStockNum ?? 0;
            //    if (articleStockNum == 0)
            //    {

            //    }
            //    else
            //    {
            //        //首先要清除原剩余库存
            //        GlobalParam.queueNum.Clear();
            //        //重新设置初始库存
            //        for (int i = 0; i < articleStockNum; i++)
            //        {
            //            GlobalParam.queueNum.Enqueue(i);
            //        }
            //    }
            //    System.Diagnostics.Debug.WriteLine($"打印当前库存:{articleStockNum}");
            //}

        }

        #endregion
    }
}
