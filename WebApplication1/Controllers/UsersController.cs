using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using WebAPI;

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
    /// 用户信息相关控制器
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
        [Route("User/Login")]
        public string Login(Users user)
        {
            Users find = db.Users.FirstOrDefault(Users => Users.UserName == user.UserName);
            if (find == null)
            {
                return "failed";
            }
            else
            {
                if (user.Password != find.Password)
                {
                    return "failed";
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
                    return "success";
                }
            }
        }
        /// <summary>
        /// 用户登出
        /// </summary>
        /// <param>无，直接读取cookie</param>
        /// <returns>"success":登出成功；"failed":登出失败</returns>
        [Route("User/Logout")]
        public string Logout()
        {
            try
            {
                HttpContext.Current.Response.Cookies.Remove("account");
            }
            catch
            {
                return "failed";
            }

            return "success";
        }
        /// <summary>
        /// 当前用户信息
        /// </summary>
        /// <param>无，直接读取cookie</param>
        /// <returns>用户所有信息,若无cookie则返回一个用户名为null的用户</returns>
        [Route("User/Info")]
        public Users Info()
        {
            var cookie = HttpContext.Current.Request.Cookies["account"];
            if (cookie == null)
            {
                Users nullUser = new Users();
                nullUser.UserName = null;
                return nullUser;
            }
            int userID = int.Parse(cookie["UserID"]);
            Users find = db.Users.Find(userID);
            return find;
        }

        /// <summary>
        /// 更改密码
        /// </summary>
        /// <param name="pw">oldPasswd,newPasswd
        /// eg:{"oldPasswd":"12345","newPasswd":"123456"}
        /// </param>
        /// <returns>"success":更改成功,"wrong password":更改失败,"error":错误（如登录态失效等）</returns>
        [Route("User/ModifyPassword")]
        public string ModifyPassword(PasswordModify pw)
        {
            var cookie = HttpContext.Current.Request.Cookies["account"];
            if (cookie == null)
                return "error";
            int userID =  int.Parse(cookie["UserID"]);
            Users find = db.Users.Find(userID);
            if (find.Password == pw.oldPasswd)
            {
                find.Password = pw.newPasswd;
                db.SaveChanges();
                return "success";
            }
            else if (find.Password != pw.oldPasswd)
            {
                return "wrong password";
            }
            else
            {
                return "error";
            }

        }
        //下面是一个读取cookie的例子
        [Route("User/TestCookie")]
        public string TestCookie()
        {
            return HttpContext.Current.Request.Cookies["account"]["Email"].ToString();
        }
        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="newUser">用户信息，必选字段为用户名、密码，可选字段为电话、邮箱等</param>
        /// <returns>"success":注册成功  "user name exists":用户名存在  "error":错误</returns>
        [Route("User/Register")]
        public string Register(Users newUser)
        {
            string newUserName = newUser.UserName;
            Users tryFind = db.Users.FirstOrDefault(Users => Users.UserName == newUserName);
            if (tryFind != null)
            {
                return "user name exists";
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
                return "error";
            }

            return "success";
        }
    }
}