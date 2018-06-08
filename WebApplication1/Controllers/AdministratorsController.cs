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
    public class AdministratorsController : ApiController
    {
        private WebAPIEntities db = new WebAPIEntities();

        /// <summary>
        /// 返回reviewver数据
        /// </summary>
        public class Returnreviewers
        {
            public List<Dictionary<string,string>> reviewers{ get; set; }
        }

        /// <summary>
        /// 查询reviewer对象
        /// </summary>
        /// </param>无 </param>
        /// <returns>success:以id和name为一单位的reviewer对象，json格式。error:权限不够"Authority Deficiency"</returns>
        [Route("Administrator/GetReviewer")]
        public string GetReviewer()
        {
            ///当前用户身份检验
            string role = HttpContext.Current.Request.Cookies["account"]["role"];
            if (role != "admin")
            {
                return "forbidden";
            }
            else
            {
                /// Users find = db.Users.FirstOrDefault(Users => Users.UserName == user.UserName);
                /// 查找数据库reviewer对象，返回json格式
                ///返回json格式,find
                JavaScriptSerializer Json = new JavaScriptSerializer();
                Returnreviewers returnreviewers = new Returnreviewers();
                ///returnreviewers.reviewers.GetEnumerator();
                returnreviewers.reviewers = new List<Dictionary<string, string>>();
                var results =
                    from Reviewer in db.Reviewer
                    select Reviewer;
                foreach(var result in results)
                {
                    Dictionary<string, string> mid = new Dictionary<string, string>();
                    mid.Add("ReviewerID", result.ReviewerID.ToString());
                    mid.Add("ReviewerName", result.ReviewName);
                    returnreviewers.reviewers.Add(mid);
                }
                return Json.Serialize(returnreviewers);
            }
        }

        /// <summary>
        /// 创建reviewer对象
        /// </summary>
        /// <param name="name"> 
        /// eg:{"name":"user1","passwd":"123456"}
        /// </param>
        /// <param name="passwd"></param>
        /// <returns>success: "success" or "failed"。error:权限不够"Authority Deficiency"</returns>
        [HttpPost,Route("Administrator/CreateReviewer")]
        public string CreateReviewer(string name, string passwd)
        {
            ///当前用户身份检验
            string role = HttpContext.Current.Request.Cookies["account"]["role"];
            if (role != "admin")
            {
                return "forbidden";
            }
            else
            {
                Reviewer find = db.Reviewer.FirstOrDefault(Reviewer => Reviewer.ReviewName == name);
                ///在reviewer表中插入name passwd参数
                if (find == null)///name不在表中
                {
                    try
                    {
                        Reviewer reviewer = new Reviewer
                        {
                            ReviewName = name,
                            Password=passwd                          
                        };
                        db.Reviewer.Add(reviewer);
                        db.SaveChanges();
                    }
                    catch
                    {
                        return "failed";
                    }
                    return "success";
                }
                else
                    return "failed";//重名
            }
        }

        /// <summary>
        /// 删除Reviewer对象
        /// </summary>
        /// <param name="name">
        /// name 把api文档中传的id改成了审核者名字
        /// eg:{"name":"zhao"}
        /// </param>
        /// <returns>success: "success" or "failed"。error:权限不够"Authority Deficiency"</returns>
        [HttpPost, Route("Administrator/DeleteReviewer")]
        public string DeleteReviewer(string name)
        {
            ///当前用户身份检验
            string role = HttpContext.Current.Request.Cookies["account"]["role"];
            if (role != "admin")
            {
                return "forbidden";
            }
            else
            {
                ///在reviewer表中匹配对应id，删除表记录
                Reviewer find = db.Reviewer.FirstOrDefault(Reviewer => Reviewer.ReviewName == name);
                if (find != null)///name在表中
                {
                    try
                    {
                        db.Reviewer.Remove(find);
                        db.SaveChanges();
                    }
                    catch
                    {
                        return "failed";
                    }
                    return "success";
                }    
                else
                    return "failed";///没有该审核者
            }
        }

        /// <summary>
        /// 更新reviewer对象（BUG等待改库😭）
        /// </summary>
        /// <param name="oldName"> 
        /// eg:{"oldName":"zhao","newName":"afadf","newPasswd":"123345"}
        /// </param>
        /// <param name="newName"></param>
        /// <param name="newPasswd"></param>
        /// <returns>success: "success" or "failed"。error:权限不够"Authority Deficiency"</returns>
        [HttpPost, Route("Administrator/UpdateReviewer")]
        public string UpdateReviewer( string oldName, string newName, string newPasswd)
        {
            ///当前用户身份检验
            string role = HttpContext.Current.Request.Cookies["account"]["role"];
            if (role != "admin")
            {
                return "forbidden";
            }
            else
            {
                ///在reviewer表中匹配对应name，更新name，password
                int isnotIn=0;
                while (true)
                {
                    Reviewer find = db.Reviewer.FirstOrDefault(Reviewer => Reviewer.ReviewName == oldName );
                    if (find != null)///id在表中
                    {
                        try
                        {
                            find.ReviewName = newName;
                            find.Password = newPasswd;
                            db.SaveChanges();
                            isnotIn++;
                        }
                        catch
                        {
                            return "failed";
                        }
                    }
                    else
                        break;
                }
                if(isnotIn!=0)
                {
                    
                    return "success";
                }          
                else
                    return "failed";
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