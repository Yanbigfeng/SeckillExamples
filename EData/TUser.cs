using System;
using System.Collections.Generic;

namespace SeckillExamples.EData
{
    public partial class TUser
    {
        public string Id { get; set; }
        public string UserAccount { get; set; }
        public string UserPwd { get; set; }
        public string UserNickName { get; set; }
        public DateTime? AddTime { get; set; }
        public int? DelFlag { get; set; }
    }
}
