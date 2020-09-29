using System;
using System.Collections.Generic;

namespace SeckillExamples.EData
{
    public partial class TOrder
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string OrderNum { get; set; }
        public string ArticleId { get; set; }
        public decimal OrderTotalPrice { get; set; }
        public DateTime? AddTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public DateTime? PayTime { get; set; }
        public DateTime? DeliverTime { get; set; }
        public DateTime? FinishTime { get; set; }
        public int OrderStatus { get; set; }
        public string OrderPhone { get; set; }
        public string OrderAddress { get; set; }
        public string OrderRemark { get; set; }
        public int? DelFlag { get; set; }
    }
}
