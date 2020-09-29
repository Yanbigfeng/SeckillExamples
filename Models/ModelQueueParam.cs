using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace SeckillExamples.Models
{
    /// <summary>
    /// 下单参数帮助
    /// </summary>
    public class ModelQueueParam
    {
        /// <summary>
        /// 下单用户编号
        /// </summary>
        public string userId { get; set; }
        /// <summary>
        /// 下单商品秒杀编号
        /// </summary>
        public string seckillArticleId { get; set; }

        /// <summary>
        /// 下单商品数量
        /// </summary>
        public int goodQuantity { get; set; }

    }
}
