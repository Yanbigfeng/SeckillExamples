using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeckillExamples.EData;
using SeckillExamples.Models;
using SeckillExamples.ToolsClass;
using StackExchange.Redis;

namespace SeckillExamples.API
{


    /// <summary>
    /// 升级下单-增加单品限流
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PlaceOrderController : ControllerBase
    {

        //注入上下文
        private ESHOPContext _context;
        private IDatabase _redisdb;
        public PlaceOrderController(ESHOPContext context, RedisHelp redisHelp) //依赖注入得到实例  
        {
            _context = context;
            _redisdb = redisHelp.GetDatabase();
        }


        //参数
        static EventWaitHandle _whRedis = new AutoResetEvent(false);
        readonly static object _lockerRedisEnqueue = new object();
        static Thread _workerRedis;

        /********************** 接口*****************************/

        #region 秒杀
        [HttpGet]
        public string order(string usreId = "y0001", string arcId = "y01", string totalPrice = "")
        {

            try
            {
                string usreIdStr = usreId;
                string seckillArticleIdStr = arcId;
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
        [HttpGet]
        public string resetStock(string seckillArticleId)
        {
            try
            {
                if (_workerRedis == null|| _workerRedis.IsAlive==false)
                {
                    _workerRedis = new Thread(DeEnqueueRedis);
                    _workerRedis.Start();
                }
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
                    //库存记录到redis
                    string articleInfoId = articleInfo.Id;
                    _redisdb.StringSet(articleInfoId, articleStockNum);


                    return $"数据库存已重置成功{articleStockNum}";
                }

            }
            catch (Exception ex)
            {

            }
            return "重置库存失败";
        }


        #endregion

        #region 防止iis宕机使用redis缓存

        private static object lockRedis = new object();

        #region 生产
        /// <summary>
        /// 生产
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="seckillArticleId">秒杀商品id</param>
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
                //使用redis存储库存
                int stockNum = (int)_redisdb.StringGet(seckillArticleId);
                if (stockNum > 0)
                {
                    modelQueue.goodQuantity = stockNum; //此处是为了证明插入顺序和消费顺序一致
                    stockNum--;
                    _redisdb.StringSet(seckillArticleId, stockNum);

                    //进行生产-可以下单
                    #region 使用reids队列进行储存
                    var jsonStr = JsonHelp.ToJsonString(modelQueue);
                    _redisdb.ListLeftPush("stock-y", jsonStr);

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


        private void DeEnqueueRedis()
        {
            try
            {

                //连接字符串
                string stq = "Server=47.92.198.126;Database=ESHOP;user id=eshop;password=eshop123456;MultipleActiveResultSets=true";
                var optionsBuilder = new DbContextOptionsBuilder<ESHOPContext>();
                optionsBuilder.UseSqlServer(stq);

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
                        string redisValue = _redisdb.ListRightPop("stock-y").ToString();
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
                    string userId = "";
                    string seckillArticleId = "";
                    if (data != null)
                    {
                        userId = data.userId;
                        seckillArticleId = data.seckillArticleId;
                        int goodQuantity = data.goodQuantity;
                        //存在库存就进行消费库存
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
                        if (dequeueNum > 0)
                        {
                            var articleInfo = db.TSeckillArticle.FirstOrDefault(u => u.Id == "y01");
                            articleInfo.ArticleStockNum -= dequeueNum;
                            db.TSeckillArticle.Update(articleInfo);
                            db.SaveChanges();
                            saved = true;
                            // articleInfo = null;
                            isdbDispose = true;
                            dequeueNum = 0;
                            //处理并发
                        }
                        //库存没有 等待信号
                        _whRedis.WaitOne();
                    }

                }
            }
            catch (Exception ex)
            {
            }

        }
        #endregion
        #endregion







        #region 修改记录说明
        /*
         * 初始记录
         * 把原先的两个队列记录形式修改为总值记录模式，节约内存空间
         * 消费方法中针对数据库的提交上下文和批量提交的优化
         * 2020-10-16：
         * 修改库存初始化记录位置为redis中。实现多商品的库存模式 key值使用商品的id，value使用库存数
         * 
         * 
         * 
         */

        #endregion
    }
}