﻿using System;
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
using SeckillExamples.ToolsClass;
using StackExchange.Redis;

namespace SeckillExamples.API
{


    /// <summary>
    /// 下单初始版本：
    /// 实现基本的消费者生产者模式
    /// </summary>


    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {

        //注入上下文
        private ESHOPContext _context;
        private IDatabase _redisdb;
        public OrderController(ESHOPContext context, RedisHelp redisHelp) //依赖注入得到实例  
        {
            _context = context;
            _redisdb = redisHelp.GetDatabase();
        }

        /********************** 接口*****************************/


        #region 秒杀
        [HttpGet]
        public string order(string usreId = "y0001", string arcId = "y01", string totalPrice = "")
        {

            try
            {
                string usreIdStr = usreId;
                string seckillArticleIdStr = arcId;


                #region 空接口测压

               // return "抢单成功";

                #endregion

                #region ConcurrentQueue
                //生产线程
                //int resultInt = Push(usreIdStr, seckillArticleIdStr);
                ////消费线程-1
                //_wh.Set();
                //if (resultInt > 0)
                //{
                //    return "抢单成功";
                //}
                //else
                //{
                //    return "抢单失败";
                //}
                #endregion

                #region redis
                int resultIntRedis = PushRedis(usreIdStr, seckillArticleIdStr);
                _whRedis.Set();



                if (resultIntRedis > 0)
                {
                    return "抢单成功";
                }
                else
                {
                    return "抢单失败";
                }
                #endregion





            }
            catch (Exception ex)
            {

            }
            return "error";
        }

        #endregion


        #region 重置库存
        static TSeckillArticle articleInfo; //临时存放
        [HttpGet]
        public string resetStock(string seckillArticleId)
        {
            try
            {
                if (_worker == null)
                {
                    _worker = new Thread(DeEnqueue);
                    _worker.Start();
                }
                if (_workerRedis == null)
                {
                    _workerRedis = new Thread(DeEnqueueRedis);
                    _workerRedis.Start();
                }
                //查询当前是否还存在库存
                articleInfo = _context.TSeckillArticle.FirstOrDefault(u => u.Id == seckillArticleId);
                int articleStockNum = articleInfo?.ArticleStockNum ?? 0;
                if (articleStockNum == 0)
                {
                    //没有库存直接返回
                    return "数据库存已为0";
                }
                else
                {
                    //库存总值记录模式
                    GlobalParam.queueStockrNum = articleStockNum;
                    ////首先要清除原剩余库存
                    //GlobalParam.queueNum.Clear();
                    ////重新设置初始库存
                    //for (int i = 0; i < articleStockNum; i++)
                    //{
                    //    GlobalParam.queueNum.Enqueue(i);
                    //}
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

        #region 使用ConcurrentQueue队列处理

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
            lock (GlobalParam.queue)
            {
                //总值模式
                if (GlobalParam.queueStockrNum > 0)
                {
                    GlobalParam.queueStockrNum--;
                    //进行生产-可以下单
                    GlobalParam.queue.Enqueue(modelQueue);
                    return 1;

                }
                else
                {
                    //库存已使用完成
                    return 0;
                }

                //队列记录模式
                //if (GlobalParam.queueNum.TryDequeue(out i))
                //{
                //    //进行生产
                //    GlobalParam.queue.Enqueue(modelQueue);
                //    return 1;
                //}
                //else
                //{
                //    //没有库存
                //    return 0;
                //}

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
                //连接字符串
                string stq = "Server=47.92.198.126;Database=ESHOP;user id=eshop;password=eshop123456;MultipleActiveResultSets=true";
                var optionsBuilder = new DbContextOptionsBuilder<ESHOPContext>();
                optionsBuilder.UseSqlServer(stq);
                //using (var db = new ESHOPContext(optionsBuilder.Options))
                //{
                var db = new ESHOPContext(optionsBuilder.Options);
                TSeckillArticle articleInfo = null;//存储如果存在修改需要查询库存
                bool isdbDispose = false; //手动记录是否释放db上下文
                int dequeueNum = 0;
                while (true)
                {
                    if (isdbDispose)
                    {
                        db = new ESHOPContext(optionsBuilder.Options);
                        isdbDispose = false;
                    }
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
                        articleInfo = db.TSeckillArticle.Where(u => u.Id == seckillArticleId).AsNoTracking().FirstOrDefault();
                        int articleStockNum = articleInfo?.ArticleStockNum ?? 0;
                        if (articleStockNum == 0)
                        {
                            //库存没有 等待信号
                            _wh.WaitOne();
                        }
                        else
                        {
                            //存在库存就进行消费库存
                            // articleInfo.ArticleStockNum -= 1;
                            dequeueNum++;
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

                        }
                    }
                    else
                    {
                        //是否处理过数据
                        if (articleInfo != null)
                        {
                            articleInfo.ArticleStockNum -= dequeueNum;
                            //db.Entry(articleInfo).State = EntityState.Modified;
                            db.TSeckillArticle.Update(articleInfo);
                            //处理并发
                            while (!saved)
                            {
                                try
                                {
                                    db.SaveChanges();
                                    saved = true;
                                    articleInfo = null;
                                    isdbDispose = true;
                                    dequeueNum = 0;
                                }
                                catch (DbUpdateConcurrencyException ex)
                                {
                                    foreach (var entry in ex.Entries)
                                    {
                                        if (entry.Entity is TSeckillArticle)
                                        {
                                            var proposedValues = entry.CurrentValues;//当前需要写入数据库的值
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
                //}
            }
            catch (Exception ex)
            {
            }
        }


        #endregion

        #endregion


        #region 防止iis宕机使用redis缓存

        private static object lockRedis = new object();

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
        public int PushRedis(String userId, string seckillArticleId)
        {
            ModelQueueParam modelQueue = new ModelQueueParam();
            modelQueue.userId = userId;
            modelQueue.seckillArticleId = seckillArticleId;
            //总值模式
            lock (lockRedis)
            {

                if (GlobalParam.queueStockrNum > 0)
                {
                    modelQueue.goodQuantity = GlobalParam.queueStockrNum;
                    GlobalParam.queueStockrNum--;
                    //进行生产-可以下单
                    #region 使用reids队列进行储存
                    var jsonStr = JsonHelp.ToJsonString(modelQueue);
                    _redisdb.ListLeftPush("order-key", jsonStr);

                    #endregion
                    return 1;

                }
                else
                {
                    //库存已使用完成
                    return 0;
                }
            }
        }
        #endregion

        #region 消费 ---单商品版本
        static EventWaitHandle _whRedis = new AutoResetEvent(false);
        readonly static object _lockerRedisEnqueue = new object();
        static Thread _workerRedis;

        private void DeEnqueueRedis()
        {
            try
            {

                //连接字符串
                string stq = "Server=47.92.198.126;Database=ESHOP;user id=eshop;password=eshop123456;MultipleActiveResultSets=true";
                var optionsBuilder = new DbContextOptionsBuilder<ESHOPContext>();
                optionsBuilder.UseSqlServer(stq);
                //using (var db = new ESHOPContext(optionsBuilder.Options))
                //{
                var db = new ESHOPContext(optionsBuilder.Options);
                bool isdbDispose = false; //手动记录是否释放db上下文
                int dequeueNum = 0;
                while (true)
                {
                    if (isdbDispose)
                    {
                        db = new ESHOPContext(optionsBuilder.Options);
                        isdbDispose = false;
                    }
                    ModelQueueParam data;
                    var saved = false;//处理并发字段
                    lock (_lockerRedisEnqueue)
                    {
                        string redisValue = _redisdb.ListRightPop("order-key").ToString();
                        if (redisValue == null)
                        {
                            data = null;
                        }
                        else
                        {
                            data = JsonHelp.ToObject<ModelQueueParam>(redisValue);
                        }

                    }
                    //存在下单人
                    if (data != null)
                    {
                        string userId = data.userId;
                        string seckillArticleId = data.seckillArticleId;
                        int goodQuantity = data.goodQuantity;

                        //存在库存就进行消费库存
                        articleInfo.ArticleStockNum -= 1;
                        dequeueNum++;
                        //同时添加订单信息
                        string id = Guid.NewGuid().ToString("N");
                        DateTime nowTime = DateTime.Now;
                        TOrder order = new TOrder();
                        order.Id = id;
                        order.UserId = userId;
                        order.ArticleId = seckillArticleId;
                        order.OrderNum = id;
                        order.OrderTotalPrice = goodQuantity;
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


                    }
                    else
                    {
                        //是否处理过数据
                        if (articleInfo != null)
                        {
                            //articleInfo.ArticleStockNum -= dequeueNum;
                            //db.Entry(articleInfo).State = EntityState.Modified;
                            db.TSeckillArticle.Update(articleInfo);
                            //处理并发
                            while (!saved)
                            {
                                try
                                {
                                    db.SaveChanges();
                                    saved = true;
                                    // articleInfo = null;
                                    isdbDispose = true;
                                    dequeueNum = 0;
                                }
                                catch (DbUpdateConcurrencyException ex)
                                {
                                    foreach (var entry in ex.Entries)
                                    {
                                        if (entry.Entity is TSeckillArticle)
                                        {
                                            var proposedValues = entry.CurrentValues;//当前需要写入数据库的值
                                            var databaseValues = entry.GetDatabaseValues();//数据库值
                                            entry.OriginalValues.SetValues(databaseValues);
                                        }
                                    }
                                }

                            }
                        }
                        //库存没有 等待信号
                        _whRedis.WaitOne();
                    }

                }
                //}
            }
            catch (Exception ex)
            {
            }

        }
        #endregion
        #endregion


        /********************** 修改记录说明*****************************/


        #region 修改记录说明
        //1.把原先的两个队列记录形式修改为总值记录模式，节约内存空间
        //2.消费方法中针对数据库的提交上下文和批量提交的优化
        #endregion
    }
}