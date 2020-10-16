using System;
using System.Collections.Generic;
using System.Linq;
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

        /********************** 接口*****************************/

    }
}