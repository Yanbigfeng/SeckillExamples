using Microsoft.EntityFrameworkCore;
using SeckillExamples.EData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SeckillExamples.Models.Order
{

    /// <summary>
    /// 暂时不用-为了以后可以直接从后台任务进行启动使用
    /// </summary>
    public class OrderMethod
    {
        #region 消费
        readonly static object _locker = new object();
        //static EventWaitHandle _wh = new AutoResetEvent(false);
        //static Thread _worker;
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
                            if (GlobalParam.queue.TryDequeue(out data))
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
                               // GlobalParam._wh.WaitOne();
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
                        else
                        {
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
                            //GlobalParam._wh.WaitOne();

                        }

                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        #endregion
    }
}
