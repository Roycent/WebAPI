using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Script.Serialization;
using WebAPI;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 返回reviewver数据
    /// </summary>
    public class ReturnReviewers
    {
        public List<Dictionary<string, string>> reviewers { get; set; }
    }
    /// <summary>
    /// 返回comment数据
    /// </summary>
    public class ReturnComments
    {
        public List<Dictionary<string, string>> comments { get; set; }
    }
    /// <summary>
    /// 更改reviewer信息
    /// </summary>
    public class ReviewerdModify
    {
        public string oldName { get; set; }
        public string newName { get; set; }
        public string newPasswd { get; set; }
    }

    public class AdministratorsController : ApiController
    {
        private WebAPIEntities db = new WebAPIEntities();

        /// <summary>
        /// 查询reviewer对象
        /// </summary>
        /// <returns>成功:{"Message":"success","Data":{"CommentID":"12333","Time":"2018-06-11 11:19:55.367","Content":"very good"}};
        /// 失败:{"Message": "failed"};
        /// 权限不足:{"Message":"forbidden"}</returns>
        [Route("Administrator/GetReviewer")]
        public string GetReviewer()
        {
            
            //当前用户身份检验
            string role = HttpContext.Current.Request.Cookies["account"]["role"];
            if (role != "admin")
            {
                Dictionary<string, string> res = new Dictionary<string, string>();
                JavaScriptSerializer Json = new JavaScriptSerializer();
                res.Add("Message", "forbidden");
                return Json.Serialize(res);
            }
            else
            {
                // Users find = db.Users.FirstOrDefault(Users => Users.UserName == user.UserName);
                // 查找数据库reviewer对象，返回json格式
                //返回json格式,find
                JavaScriptSerializer Json = new JavaScriptSerializer();
                Dictionary<string, string> res = new Dictionary<string, string>();
                ReturnReviewers returnreviewers = new ReturnReviewers();
                returnreviewers.reviewers = new List<Dictionary<string, string>>();
                var results =
                    from Reviewer in db.Reviewer
                    select Reviewer;
                try
                { 
                foreach(var result in results)
                    {
                        Dictionary<string, string> mid = new Dictionary<string, string>();
                        mid.Add("ReviewerID", result.ReviewerID.ToString());
                        mid.Add("Name", result.Name);
                        returnreviewers.reviewers.Add(mid);
                    }
                }
                catch
                {
                    res.Add("Message", "failed");
                    return Json.Serialize(res);
                }
                return Json.Serialize(returnreviewers);
            }
        }

        /// <summary>
        /// 创建reviewer对象
        /// </summary>
        /// <param name="reviewer"> 
        /// eg:{"Name":"user1","Password":"123456"}
        /// </param>
        /// <returns>成功:{"Message": "success"};失败:{"Message": "failed"};权限不足:{"Message":"forbidden"}</returns>
        [HttpPost,Route("Administrator/CreateReviewer")]
        public string CreateReviewer(Reviewer reviewer)
        {
            Dictionary<string, string> res = new Dictionary<string, string>();
            JavaScriptSerializer Json = new JavaScriptSerializer();
            //当前用户身份检验
            string role = HttpContext.Current.Request.Cookies["account"]["role"];
            if (role != "admin")
            {
                res.Add("Message", "forbidden");
                return Json.Serialize(res);
            }
            else
            {
                Reviewer find = db.Reviewer.FirstOrDefault(Reviewer => Reviewer.Name == reviewer.Name);
                //在reviewer表中插入name passwd参数
                if (find == null)//name不在表中
                {
                    try
                    {
                        Reviewer reviewer1 = new Reviewer
                        {
                            Name = reviewer.Name,
                            Password = reviewer.Password
                        };
                        db.Reviewer.Add(reviewer1);
                        db.SaveChanges();
                    }
                    catch
                    {
                        res.Add("Message", "failed");
                        return Json.Serialize(res);
                    }
                    res.Add("Message", "success");
                    return Json.Serialize(res);
                }
                else
                {
                    res.Add("Message", "failed");
                    return Json.Serialize(res);
                }
            }
        }

        /// <summary>
        /// 删除Reviewer对象
        /// </summary>
        /// <param name="reviewer">
        /// eg:{"Name":"zhao"}
        /// </param>
        /// <returns>成功:{"Message": "success"};失败:{"Message": "failed"};权限不足:{"Message":"forbidden"}</returns>
        [HttpPost, Route("Administrator/DeleteReviewer")]
        public string DeleteReviewer(Reviewer reviewer)
        {
            Dictionary<string, string> res = new Dictionary<string, string>();
            JavaScriptSerializer Json = new JavaScriptSerializer();
            //当前用户身份检验
            string role = HttpContext.Current.Request.Cookies["account"]["role"];
            if (role != "admin")
            {
                res.Add("Message", "forbidden");
                return Json.Serialize(res);
            }
            else
            {
                //在reviewer表中匹配对应id，删除表记录
                Reviewer find = db.Reviewer.FirstOrDefault(Reviewer => Reviewer.Name == reviewer.Name);
                if (find != null)//name在表中
                {
                    try
                    {
                        db.Reviewer.Remove(find);
                        db.SaveChanges();
                    }
                    catch
                    {
                        res.Add("Message", "failed");
                        return Json.Serialize(res);
                    }
                    res.Add("Message", "success");
                    return Json.Serialize(res);
                }    
                else
                {
                    res.Add("Message", "failed");
                    return Json.Serialize(res);//没有该审核者
                }
            }
        }

        /// <summary>
        /// 更新reviewer对象
        /// </summary>
        /// <param name="rm"> 
        /// eg:{"oldName":"102303","newName":"afadf","newPasswd":"123345"}
        /// </param>
        /// <returns>成功:{"Message":"success"};失败:{"Message":"failed"};权限不足:{"Message":"forbidden"}</returns>
        [HttpPost, Route("Administrator/UpdateReviewer")]
        public string UpdateReviewer( ReviewerdModify rm )
        {
            Dictionary<string, string> res = new Dictionary<string, string>();
            JavaScriptSerializer Json = new JavaScriptSerializer();
            //当前用户身份检验
            string role = HttpContext.Current.Request.Cookies["account"]["role"];
            if (role != "admin")
            {
                return "forbidden";
            }
            else
            {
                //在reviewer表中匹配对应name，更新name，password
                Reviewer find = db.Reviewer.FirstOrDefault(Reviewer => Reviewer.Name == rm.oldName);
                if (find != null)//id在表中
                {
                    try
                    {
                        find.Name = rm.newName;
                        find.Password = rm.newPasswd;
                        db.SaveChanges();
                    }
                    catch
                    {
                        res.Add("Message", "failed");
                        return Json.Serialize(res);
                    }
                    res.Add("Message", "success");
                    return Json.Serialize(res);
                }
                else
                {
                    res.Add("Message", "failed");
                    return Json.Serialize(res);//无此人
                }
                
            }
        }


        /// <summary>
        /// 查询comment
        /// </summary>
        /// <returns>
        /// 成功:{"Message":"success","Data":{"CommentID":"12333","Time":"2018-06-11 11:19:55.367","Content":"very good"}};
        /// 失败:{"Message":"failed"};
        /// 权限不足:{"Message":"forbidden"}</returns>
        [Route("Administrator/GetComment")]
        public string GetComment()
        {
            Dictionary<string, string> res = new Dictionary<string, string>();
            JavaScriptSerializer Json = new JavaScriptSerializer();
            //当前用户身份检验
            string role = HttpContext.Current.Request.Cookies["account"]["role"];
            if (role != "admin")
            {
                res.Add("Message", "forbidden");
                return Json.Serialize(res);
            }
            else
            {
                ReturnComments returncomments = new ReturnComments();
                returncomments.comments = new List<Dictionary<string, string>>();
                var results =
                    from Comment in db.Comment
                    select Comment;
                try
                {
                    foreach (var result in results)
                    {
                        Dictionary<string, string> mid = new Dictionary<string, string>();
                        mid.Add("CommentID", result.CommentID.ToString());
                        mid.Add("Time", result.Time.ToString());
                        mid.Add("Type", result.Content.ToString());
                        returncomments.comments.Add(mid);
                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        res.Add("Message", "success");
                        res.Add("Data", serializer.Serialize(returncomments));
                    }
                }
                catch
                {
                    res.Add("Message", "failed");
                    return Json.Serialize(res);
                }
                return Json.Serialize(res);
            }
        }
        /// <summary>
        /// 删除comment
        /// </summary>
        /// <param name="comment">
        /// eg:{"CommentID":"12333"}
        /// </param>
        /// <returns>成功:{"Message":"success"};失败:{"Message":"failed"};权限不足:{"Message":"forbidden"}</returns>
        [HttpPost, Route("Administrator/DeleteComment")]
        public string DeleteComment(Comment comment)
        {
            Dictionary<string, string> res = new Dictionary<string, string>();
            JavaScriptSerializer Json = new JavaScriptSerializer();
            //当前用户身份检验
            string role = HttpContext.Current.Request.Cookies["account"]["role"];
            if (role != "admin")
            {
                res.Add("Message", "forbidden");
                return Json.Serialize(res);
            }
            else
            {
                //在reviewer表中匹配对应id，删除表记录
                Comment find = db.Comment.FirstOrDefault(Comment => Comment.CommentID == comment.CommentID);
                if (find != null)//name在表中
                {
                    try
                    {
                        db.Comment.Remove(find);
                        db.SaveChanges();
                    }
                    catch
                    {
                        res.Add("Message", "failed");
                        return Json.Serialize(res);
                    }
                    res.Add("Message", "success");
                    return Json.Serialize(res);
                }
                else
                {
                    res.Add("Message", "failed");
                    return Json.Serialize(res);//没有该审核者
                }
            }
        }

        /// <summary>
        /// 管理员登录
        /// </summary>
        /// <param name="administrator">
        /// eg:{"AdminName":"Admin1","Password":"123456"}
        /// </param>
        /// <returns>成功:{"Message":"success"};失败:{"Message":"failed"}
        /// </returns>
        [HttpPost, Route("Administrator/Login")]
        public string AdministratorLogin(Administrator administrator)
        {
            Dictionary<string, string> res = new Dictionary<string, string>();
            Administrator find = db.Administrator.FirstOrDefault(Administrator => Administrator.AdminName == administrator.AdminName);
            JavaScriptSerializer Json = new JavaScriptSerializer();
            if (find == null)
            {
                res.Add("Message", "failed");
                return Json.Serialize(res);
            }
            else
            {
                if (administrator.Password != find.Password)
                {
                    res.Add("Message", "Password error!");
                    return Json.Serialize(res);
                }
                else
                {
                    HttpCookie cookie = new HttpCookie("account");
                    cookie["role"] = "admin";
                    cookie["name"] = find.AdminName;
                    cookie["AdminID"] = find.AdministratorID.ToString();
                    cookie.Expires = DateTime.Now.AddMinutes(120);
                    HttpContext.Current.Response.Cookies.Add(cookie);
                    res.Add("Message", "success");
                    return Json.Serialize(res);
                }
            }
        }
        /// <summary>
        /// 管理员登出
        /// </summary>
        /// <returns>成功:{"Message":"success"};失败:{"Message":"failed"}
        /// </returns>
        [Route("Administrator/Logout")]
        public string Logout()
        {
            JavaScriptSerializer Json = new JavaScriptSerializer();
            Dictionary<string, string> res = new Dictionary<string, string>();
            try
            {
                var cookie = HttpContext.Current.Request.Cookies["account"];
                cookie.Expires = DateTime.Now.AddDays(-7);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
            catch
            {
                res.Add("Message", "Cookie error!");
                return Json.Serialize(res);
            }
            res.Add("Message", "success");
            return Json.Serialize(res);
        }

        /// <summary>
        /// 管理员更改密码
        /// </summary>
        /// <param name="pw">
        /// eg:{"oldPasswd":"12345","newPasswd":"123456"}
        /// </param>
        /// <returns>
        /// 成功:{"Message":"success"};
        /// cookie不存在:{"Message":"Cookie不存在"};
        /// 原密码输入错误:{"Message":"密码错误"};
        /// 未知错误:{"Message":"未知错误"}
        /// </returns>
        [Route("Administrator/ModifyPassword")]
        public string ModifyPassword(PasswordModify pw)
        {
            JavaScriptSerializer Json = new JavaScriptSerializer();
            Dictionary<string, string> res = new Dictionary<string, string>();
            var cookie = HttpContext.Current.Request.Cookies["account"];
            if (cookie == null)
            {
                res.Add("Message", "Cookie不存在");
                return Json.Serialize(res);
            }
            int AdministratorID = int.Parse(cookie["AdminID"]);
            Administrator find = db.Administrator.Find(AdministratorID);
            if (find.Password == pw.oldPasswd)
            {
                find.Password = pw.newPasswd;
                db.SaveChanges();
                res.Add("Message", "success");
                return Json.Serialize(res);
            }
            else if (find.Password != pw.oldPasswd)
            {
                res.Add("Message", "密码错误");
                return Json.Serialize(res);
            }
            else
            {
                res.Add("Message", "未知错误");
                return Json.Serialize(res);
            }
        }

        /// <summary>
        /// 获取角色
        /// </summary>
        /// <returns>
        /// 身份不明，未知错误: {"Message":"failed"};
        /// 游客: {"Message":"Cookie error"};
        /// 普通用户:{"Message":"success","Data":{"Identity","commonuser"}};
        /// 专家用户:{"Message":"success","Data":{"Identity","expertuser"}};
        /// 管理员用户:{"Message":"success","Data":{"Identity","adminuser"}};
        /// </returns>
        [Route("GetRole")]
        public string Info()
        {
            JavaScriptSerializer Json = new JavaScriptSerializer();
            Dictionary<string, string> res = new Dictionary<string, string>();
            var cookie = HttpContext.Current.Request.Cookies["account"];
            try
            {
                Dictionary<string, string> roles = new Dictionary<string, string>();
                string role = "failed";
                if (cookie["role"].ToString() == "user")
                {
                    long userID = long.Parse(cookie["UserID"]);
                    Users find = db.Users.Find(userID);
                    if (find.IsExpert is true)
                        role = "expertuser";
                    else
                        role = "commonuser";
                }
                else if (cookie["role"].ToString() == "admin")
                {
                        role = "adminuser";
                }
                roles.Add("Identity", role);//返回角色信息
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                res.Add("Message", "success");
                res.Add("Data", serializer.Serialize(roles));
                return Json.Serialize(res);
            }
            catch
            {
                res.Add("Message", "Cookie error");
                return Json.Serialize(res);
            }
        }
    }
}