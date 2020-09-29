using System;
using System.Collections.Generic;

namespace SeckillExamples.EData
{
    public partial class TPayRecord
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string OrderId { get; set; }
        public string OrderNum { get; set; }
        public decimal PayPrice { get; set; }
        public int PayStatus { get; set; }
        public int PayStyle { get; set; }
        public DateTime? AddTime { get; set; }
        public int? DelFlag { get; set; }
    }
}
