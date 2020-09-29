using System;
using System.Collections.Generic;

namespace SeckillExamples.EData
{
    public partial class TSeckillTimeModel
    {
        public string Id { get; set; }
        public string SeckillTime { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? Status { get; set; }
        public DateTime? AddTime { get; set; }
        public int? DelFlag { get; set; }
    }
}
