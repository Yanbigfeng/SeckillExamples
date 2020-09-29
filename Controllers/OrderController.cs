using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SeckillExamples.EData;
using SeckillExamples.Models;

namespace SeckillExamples.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {

        //注入上下文
        private ESHOPContext _context;

        public OrderController(ESHOPContext context) //依赖注入得到实例  
        {
            _context = context;
        }

        //线程安全的队列
        ConcurrentQueue<ModelQueueParam> queue = GlobalParam.queue;
        ConcurrentQueue<int> queueNum = GlobalParam.queueNum;

        /********************** 接口*****************************/


        #region 秒杀
        [HttpGet]
        public string order(string usreId = "y0001", string arcId = "y01", string totalPrice = "")
        {

            try
            {
                string usreIdStr = usreId;
                string seckillArticleIdStr = arcId;
                //生产线程
                int resultInt = Push(usreIdStr, seckillArticleIdStr);
                //消费线程-1
                _wh.Set();

                if (resultInt > 0)
                {
                    return "抢单成功";
                }
                else
                {
                    return "抢单失败";
                }
            }
            catch (Exception ex)
            {

            }
            return "error";
        }

        #endregion


        #region 重置库存
        [HttpGet]
        public string resetStock(string seckillArticleId)
        {
            try
            {
                _worker = new Thread(DeEnqueue);
                _worker.Start();
                //查询当前是否还存在库存
                var articleInfo = _context.TSeckillArticle.FirstOrDefault(u => u.Id == seckillArticleId);
                int articleStockNum = articleInfo?.ArticleStockNum ?? 0;
                if (articleStockNum == 0)
                {
                    //没有库存直接返回
                    return "数据库存已为0";
                }
                else
                {
                    //首先要清除原剩余库存
                    queueNum.Clear();
                    //重新设置初始库存
                    for (int i = 0; i < articleStockNum; i++)
                    {
                        queueNum.Enqueue(i);
                    }
                    return $"数据库存已重置成功{articleStockNum}";
                }

            }
            catch (Exception ex)
            {

            }
            return "重置库存失败";
        }


        #endregion

        /********************** 帮助方法*****************************/

        #region 生产

        /// <summary>
        /// 生产
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="seckillArticleId"></param>
        /// <returns>
        /// 1:下单成功、
        /// 0：下单失败
        /// </returns>
        public int Push(String userId, string seckillArticleId)
        {
            ModelQueueParam modelQueue = new ModelQueueParam();
            modelQueue.userId = userId;
            modelQueue.seckillArticleId = seckillArticleId;
            lock (this.queue)
            {
                int i = 0;
                if (queueNum.TryDequeue(out i))
                {
                    //进行生产
                    queue.Enqueue(modelQueue);
                    return 1;
                }
                else
                {
                    //没有库存
                    return 0;
                }

            }

        }

        #endregion


        #region 消费
        static EventWaitHandle _wh = new AutoResetEvent(false);
        readonly static object _locker = new object();
        static Thread _worker;
        public void DeEnqueue()
        {
            try
            {
                string stq = "Server=47.92.198.126;Database=ESHOP;user id=eshop;password=eshop123456;MultipleActiveResultSets=true";
                var optionsBuilder = new DbContextOptionsBuilder<ESHOPContext>();
                optionsBuilder.UseSqlServer(stq);
                using (var db = new ESHOPContext(optionsBuilder.Options))
                {
                    TSeckillArticle articleInfo = null;//存储如果存在修改需要查询库存
                    while (true)
                    {
                        ModelQueueParam data;

                        var saved = false;//处理并发字段
                        lock (_locker)
                        {
                            //根据数据内容进行循环插入数据
                            if (queue.TryDequeue(out data))
                            {

                            }
                        }
                        //存在下单人
                        if (data != null)
                        {
                            string userId = data.userId;
                            string seckillArticleId = data.seckillArticleId;
                            //查询秒杀商品信息
                            articleInfo = db.TSeckillArticle.Where(u => u.Id == seckillArticleId).FirstOrDefault();
                            int articleStockNum = articleInfo?.ArticleStockNum ?? 0;
                            if (articleStockNum == 0)
                            {
                                //库存没有 等待信号
                                _wh.WaitOne();
                            }
                            else
                            {
                                //存在库存就进行消费库存
                                articleInfo.ArticleStockNum -= 1;

                                //同时添加订单信息
                                string id = Guid.NewGuid().ToString("N");
                                DateTime nowTime = DateTime.Now;
                                TOrder order = new TOrder();
                                order.Id = id;
                                order.UserId = userId;
                                order.ArticleId = seckillArticleId;
                                order.OrderNum = id;
                                order.OrderTotalPrice = 50;
                                order.AddTime = nowTime;
                                order.UpdateTime = nowTime;
                                order.PayTime = nowTime;
                                order.DeliverTime = nowTime;
                                order.FinishTime = nowTime;
                                order.OrderStatus = 3;
                                order.OrderPhone = "12451554";
                                order.OrderAddress = "地址地址地址";
                                order.OrderRemark = "订单备注";
                                order.DelFlag = 1;

                                //进行数据库真实操作
                                db.TOrder.Attach(order);
                                db.TOrder.Add(order);
                                //db.Entry(articleInfo).State = EntityState.Modified;
                                #region 并发处理
                                //处理并发
                                #endregion
                            }
                        }
                        else {
                            //是否处理过数据
                            if (articleInfo != null)
                            {
                                // articleInfo.ArticleStockNum -= dequeueNum;
                                db.Entry(articleInfo).State = EntityState.Modified;
                                //处理并发
                                while (!saved)
                                {
                                    try
                                    {
                                        db.SaveChanges();
                                        saved = true;
                                    }
                                    catch (DbUpdateConcurrencyException ex)
                                    {
                                        foreach (var entry in ex.Entries)
                                        {
                                            if (entry.Entity is TSeckillArticle)
                                            {
                                                var proposedValues = entry.CurrentValues;//当前值
                                                var databaseValues = entry.GetDatabaseValues();//数据库值
                                                entry.OriginalValues.SetValues(databaseValues);
                                            }
                                        }
                                    }

                                }
                            }
                            //库存没有 等待信号
                            _wh.WaitOne();

                        }
                    
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        #endregion








        #region 处理并发代码记录
        //while (!saved) {
        //                               try
        //                               {
        //                                   db.SaveChanges();
        //                                   saved = true;
        //                               }
        //                               catch (DbUpdateConcurrencyException ex)
        //                               {
        //                                   foreach (var entry in ex.Entries)
        //                                   {
        //                                       if (entry.Entity is TSeckillArticle)
        //                                       {
        //                                           var proposedValues = entry.CurrentValues;
        //                                           var databaseValues = entry.GetDatabaseValues();
        //                                           foreach (var property in proposedValues.Properties)
        //                                           {
        //                                               var proposedValue = proposedValues[property];
        //                                               var databaseValue = databaseValues[property];


        //                                               // TODO: decide which value should be written to database
        //                                               // proposedValues[property] = <value to be saved>;
        //                                               proposedValues["ArticleStockNum"] = (object)(Convert.ToInt32(databaseValues["ArticleStockNum"]) - 1);
        //                                           }
        //                                           entry.OriginalValues.SetValues(databaseValues);
        //                                       }
        //                                   }
        //                               }

        //                           }
        #endregion

    }
}