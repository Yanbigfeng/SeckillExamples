using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SeckillExamples.API
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ApiTestController : ControllerBase
    {

        [HttpGet]
        public string Test() {

            return "这是api测试";
        }

    }
}