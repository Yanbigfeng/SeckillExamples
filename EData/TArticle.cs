using System;
using System.Collections.Generic;

namespace SeckillExamples.EData
{
    public partial class TArticle
    {
        public string Id { get; set; }
        public string ArticleTitle { get; set; }
        public string ArticleDesc { get; set; }
        public decimal? ArticlePrice { get; set; }
        public int ArticleStockNum { get; set; }
        public int ArticleStatus { get; set; }
        public DateTime? AddTime { get; set; }
        public int? DelFlag { get; set; }
    }
}
