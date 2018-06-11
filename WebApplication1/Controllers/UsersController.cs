using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using static WebAPI.Controllers.Utils;

namespace WebAPI.Controllers
{
    
    /// <summary>
    /// 更改密码
    /// </summary>
    public class PasswordModify
    {
        public string oldPasswd { get; set; }
        public string newPasswd { get; set; }
    }
    /// <summary>
    /// 用户信息相关控制器 by 邢智涣
    /// </summary>
    public class UsersController : ApiController
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

        //controller前加Route,可以直接设置该函数的URL,比如下面的这个URL就是http://localhost:xxxx/Users/Login
        //调试web api可以用postman 
        //body的类型选"raw" 并用json格式进行输入
        //下面是一个读取数据库并添加cookie的例子

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="user">UserName,Password
        /// eg:{"UserName":"user1","Password":"123456"}
        /// </param>
        /// <returns>"failed":登录失败 "success":登录成功
        /// </returns>
        [HttpPost,Route("User/Login")]
        public HttpResponseMessage Login(Users user)
        {
            Dictionary<string, string> res = new Dictionary<string, string>();
            Users find = db.Users.FirstOrDefault(Users => Users.UserName == user.UserName);
            if (find == null)
            {
                res.Add("Message", "failed");
                return ConvertToJson(res);
            }
            else
            {
                if (user.Password != find.Password)
                {
                    res.Add("Message", "failed");
                    return ConvertToJson(res);
                }
                else
                {
                    HttpCookie cookie = new HttpCookie("account");
                    cookie["role"] = "user";
                    cookie["name"] = find.UserName;
                    cookie["isExpert"] = find.IsExpert.ToString();
                    cookie["Email"] = find.Email;
                    cookie["Phone"] = find.Phone;
                    cookie["point"] = find.integral.ToString();
                    cookie["UserID"] = find.UserID.ToString();
                    cookie.Expires = DateTime.Now.AddMinutes(120);
                    HttpContext.Current.Response.Cookies.Add(cookie);
                    res.Add("Message", "success");
                    return ConvertToJson(res);
                }
            }
        }

        /// <summary>
        /// 用户登出
        /// </summary>
        /// <param>无，直接读取cookie</param>
        /// <returns>"success":登出成功；"failed":登出失败</returns>
        [Route("User/Logout")]
        public HttpResponseMessage Logout()
        {
            Dictionary<string, string> res = new Dictionary<string, string>();
            try
            {
                var cookie = HttpContext.Current.Request.Cookies["account"];
                cookie.Expires = DateTime.Now.AddDays(-7);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
            catch
            {
                res.Add("Message", "failed");
                return ConvertToJson(res);
            }
            res.Add("Message", "success");
            return ConvertToJson(res);
        }

        /// <summary>
        /// 用以测试cookie是否被正确操作
        /// </summary>
        /// <returns></returns>
        [Route("User/TestCookie")]
        public string TestCookie()
        {
            var cookie = HttpContext.Current.Request.Cookies["account"];
            if (cookie == null)
            {
                return "no cookie";
            }
            else
            {
                return "cookie";
            }
        }

        /// <summary>
        /// 当前用户信息
        /// </summary>
        /// <param>无，直接读取cookie</param>
        /// <returns>用户所有信息,若无cookie则返回Message: failed</returns>
        [Route("User/Info")]
        public HttpResponseMessage Info()
        {
            var cookie = HttpContext.Current.Request.Cookies["account"];
            Dictionary<string, string> res = new Dictionary<string, string>();
            if (cookie == null)
            {
                res.Add("Message", "failed");
            }
            int userID = int.Parse(cookie["UserID"]);
            Users find = db.Users.Find(userID);
            res.Add("UserName", find.UserName);
            res.Add("UserID", find.UserID.ToString());
            res.Add("IsExpert", find.IsExpert.ToString());
            res.Add("Email", find.Email);
            res.Add("integral", find.integral.ToString());
            return ConvertToJson(res);
        }
        /// <summary>
        /// 更改密码
        /// </summary>
        /// <param name="pw">oldPasswd,newPasswd
        /// eg:{"oldPasswd":"12345","newPasswd":"123456"}
        /// </param>
        /// <returns>"success":更改成功,"wrong password":更改失败,"error":错误（如登录态失效等）</returns>
        [Route("User/ModifyPassword")]
        public HttpResponseMessage ModifyPassword(PasswordModify pw)
        {
            Dictionary<string, string> res = new Dictionary<string, string>();
            var cookie = HttpContext.Current.Request.Cookies["account"];
            if (cookie == null)
            {
                res.Add("Message","Cookie不存在");
                return ConvertToJson(res);
            }
            int userID =  int.Parse(cookie["UserID"]);
            Users find = db.Users.Find(userID);
            if (find.Password == pw.oldPasswd)
            {
                find.Password = pw.newPasswd;
                db.SaveChanges();
                res.Add("Message", "success");
                return ConvertToJson(res);
            }
            else if (find.Password != pw.oldPasswd)
            {
                res.Add("Message", "密码错误");
                return ConvertToJson(res);
            }
            else
            {
                res.Add("Message", "未知错误");
                return ConvertToJson(res);
            }

        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="newUser">用户信息，必选字段为用户名、密码，可选字段为电话、邮箱等</param>
        /// <returns>"success":注册成功  "user name exists":用户名存在  "error":错误</returns>
        [Route("User/Register")]
        public HttpResponseMessage Register(Users newUser)
        {
            string newUserName = newUser.UserName;
            Users tryFind = db.Users.FirstOrDefault(Users => Users.UserName == newUserName);
            Dictionary<string, string> res = new Dictionary<string, string>();
            if (tryFind != null)
            {
                res.Add("Message", "user name exists");
                return ConvertToJson(res);
            }
            newUser.UserID = GenUserID();
            newUser.integral = 100;//用户初始积分
            try
            {
                db.Users.Add(newUser);
                db.SaveChanges();
            }
            catch
            {
                res.Add("Message", "error");
                return ConvertToJson(res);
            }
            res.Add("Message", "success");
            return ConvertToJson(res);
        }
    }
}