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
            Random rd = new Random();
            int r = rd.Next(1000);
            long userID = (long)(DateTime.Now.ToUniversalTime() - new DateTime(2018, 3, 24)).TotalMilliseconds + r;
            Users tryFind = db.Users.Find(userID);
            while (tryFind != null)
            {
                userID = GenUserID();
                tryFind = db.Users.Find(userID);
            }
            return userID;
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
            ReturnPatent patent = new ReturnPatent();
            patent.access = "success";
            patent.data = db.Patent.Find(id);
            return ConvertToJson(patent);
        }

    }
}