﻿using System;
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
    public class AdministratorsController : ApiController
    {
        private WebAPIEntities db = new WebAPIEntities();

        /// <summary>
        /// 返回reviewver数据
        /// </summary>
        public class ReturnReviewers
        {
            public List<Dictionary<string,string>> reviewers{ get; set; }
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
            public long Id { get; set; }
            public string newName { get; set; }
            public string newPasswd{ get; set; }
        }

        /// <summary>
        /// 查询reviewer对象
        /// </summary>
        /// <param>无 </param>
        /// <returns>success:{"ReviewerID":"12333","ReviewerName":"zhaojiemin"}。error:{"Message","forbidden"}</returns>
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
                        mid.Add("ReviewerName", result.ReviewName);
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
        /// eg:{"ReviewName":"user1","Password":"123456"}
        /// </param>
        /// <returns>success:{"Message", "success"}。failed:{"Message", "failed"}。error:{"Message","forbidden"}</returns>
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
                Reviewer find = db.Reviewer.FirstOrDefault(Reviewer => Reviewer.ReviewName == reviewer.ReviewName);
                //在reviewer表中插入name passwd参数
                if (find == null)//name不在表中
                {
                    try
                    {
                        Reviewer reviewer1 = new Reviewer
                        {
                            ReviewName = reviewer.ReviewName,
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
        /// name 把api文档中传的id改成了审核者名字
        /// eg:{"ReviewName":"zhao"}
        /// </param>
        /// <returns>success:{"Message", "success"}。failed:{"Message", "failed"}。error:{"Message","forbidden"}</returns>
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
                Reviewer find = db.Reviewer.FirstOrDefault(Reviewer => Reviewer.ReviewName == reviewer.ReviewName);
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
        /// 更新reviewer对象（BUG等待改库😭）
        /// </summary>
        /// <param name="rm"> 
        /// eg:{"id":"102303","newName":"afadf","newPasswd":"123345"}
        /// </param>
        /// <returns>success:{"Message", "success"}。failed:{"Message", "failed"}。error:{"Message","forbidden"}</returns>
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
                Reviewer find = db.Reviewer.FirstOrDefault(Reviewer => Reviewer.ReviewerID == rm.Id);
                if (find != null)//id在表中
                {
                    try
                    {
                        find.ReviewName = rm.newName;
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
        /// <param>无 </param>
        /// <returns>success:{"CommentID":"12333","Time":"2018-06-11 11:19:55.367","Content":"very good"}。failed:{"Message", "failed"}。error:{"Message","forbidden"}</returns>
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
                    }
                }
                catch
                {
                    res.Add("Message", "failed");
                    return Json.Serialize(res);
                }
                return Json.Serialize(returncomments);
            }
        }
        /// <summary>
        /// 删除comment
        /// </summary>
        /// <param>
        /// eg:{"CommentID":"12333"}
        /// </param>
        /// <returns>success:{"Message", "success"}。failed:{"Message", "failed"}。error:{"Message","forbidden"}</returns>
        [HttpPost, Route("Administrator/DeleteReviewer")]
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

        // GET: api/Administrators
        public IQueryable<Administrator> GetAdministrator()
        {
            return db.Administrator;
        }

        // GET: api/Administrators/5
        [ResponseType(typeof(Administrator))]
        public IHttpActionResult GetAdministrator(long id)
        {
            Administrator administrator = db.Administrator.Find(id);
            if (administrator == null)
            {
                return NotFound();
            }

            return Ok(administrator);
        }

        // PUT: api/Administrators/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutAdministrator(long id, Administrator administrator)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != administrator.AdministratorID)
            {
                return BadRequest();
            }

            db.Entry(administrator).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdministratorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Administrators
        [ResponseType(typeof(Administrator))]
        public IHttpActionResult PostAdministrator(Administrator administrator)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Administrator.Add(administrator);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (AdministratorExists(administrator.AdministratorID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = administrator.AdministratorID }, administrator);
        }

        // DELETE: api/Administrators/5
        [ResponseType(typeof(Administrator))]
        public IHttpActionResult DeleteAdministrator(long id)
        {
            Administrator administrator = db.Administrator.Find(id);
            if (administrator == null)
            {
                return NotFound();
            }

            db.Administrator.Remove(administrator);
            db.SaveChanges();

            return Ok(administrator);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AdministratorExists(long id)
        {
            return db.Administrator.Count(e => e.AdministratorID == id) > 0;
        }
    }
}