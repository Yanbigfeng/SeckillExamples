using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SeckillExamples.EData
{
    public partial class TSeckillArticle
    {
        public string Id { get; set; }
        public string SeckillTimeModelId { get; set; }
        public string ArticleId { get; set; }
        public int SeckillType { get; set; }
        public string ArticleName { get; set; }
        public string ArticleUrl { get; set; }
        public decimal ArticlePrice { get; set; }
        [ConcurrencyCheck]
        public int ArticleStockNum { get; set; }
        public decimal ArticlePercent { get; set; }
        public int LimitNum { get; set; }
        public int SeckillStatus { get; set; }
        public DateTime? AddTime { get; set; }
        public int? DelFlag { get; set; }
    }
}
