using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using static WebAPI.Controllers.Utils;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 用于压力测试的接口类
    /// </summary>
    public class StressTestingController : ApiController
    {
        private WebAPIEntities db = new WebAPIEntities();

        private long GenUserID()
        {
            byte[] uuid = Guid.NewGuid().ToByteArray();
            return BitConverter.ToInt64(uuid, 0);
        }

        private long GetPatentID()
        {
            var patent = db.Patent.OrderBy(p => Guid.NewGuid()).First();
            return patent.PatentID;
        }

        /// <summary>
        /// 测试注册接口
        /// </summary>
        /// <returns></returns>
        [Route("Testing/Register")]
        public HttpResponseMessage Register()
        {
            Users newUser = new Users
            {
                UserID = GenUserID(),
                integral = 100,//用户初始积分
                UserName = "StressTesting",
                Nickname = "StressTesting",
                Password = "123456"
            };
            Dictionary<string, string> res = new Dictionary<string, string>();
            try
            {
                db.Users.Add(newUser);
                db.SaveChanges();
            }
            catch(Exception e )
            {
                res.Add("Message", e.Message);
                return ConvertToJson(res);
            }
            res.Add("Message", "success");
            return ConvertToJson(res);
        }

        /// <summary>
        /// 测试查询接口
        /// </summary>
        /// <returns></returns>
        [Route("Testing/RequestPatent")]
        public HttpResponseMessage RequestPatent()
        {
            long id = GetPatentID();
            db.Configuration.ProxyCreationEnabled = false;
            JavaScriptSerializer Json = new JavaScriptSerializer();
            ReturnData<Patent> patent = new ReturnData<Patent>();
            patent.message = "success";
            patent.data = db.Patent.Find(id);
            return ConvertToJson(patent);
        }
        /// <summary>
        /// 测试查询固定Patent接口
        /// </summary>
        /// <returns></returns>
        [Route("Testing/StaticPatent")]
        public HttpResponseMessage RequestStaticPatent()
        {
            long id = 7065147417;
            db.Configuration.ProxyCreationEnabled = false;

            JavaScriptSerializer Json = new JavaScriptSerializer();
            ReturnData<Patent> patent = new ReturnData<Patent>();
            patent.message = "success";
            patent.data = db.Patent.Find(id);
            return ConvertToJson(patent);
        }

    }
}