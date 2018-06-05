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
    public class PasswordModify
    {
        public string oldPasswd { get; set; }
        public string newPasswd { get; set; }
    }
    public class UsersController : ApiController
    {
        private WebAPIEntities db = new WebAPIEntities();

        //controller前加Route,可以直接设置该函数的URL,比如下面的这个URL就是http://localhost:xxxx/Users/Login
        //调试web api可以用postman 
        //body的类型选"x-www-form-urlencoded"
        //下面是一个读取数据库并添加cookie的例子
        [Route("User/Login")]
        public string Login(Users user)
        {
            Users find = db.Users.First(Users => Users.UserName == user.UserName);
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

        [Route("User/Logout")]
        public string Logout()
        {
            HttpContext.Current.Response.Cookies.Remove("account");
            return "success";
        }

        [Route("User/Info")]
        public IHttpActionResult Info()
        {
            int userID = int.Parse(HttpContext.Current.Request.Cookies["account"]["UserID"]);
            Users find = db.Users.Find(userID);
            return Ok(find);
        }

        [Route("User/ModifyPassword")]
        public string ModifyPassword(PasswordModify pw)
        {
            int userID =  int.Parse(HttpContext.Current.Request.Cookies["account"]["UserID"]);
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

        // GET: api/Users
        public IQueryable<Users> GetUsers()
        {
            return db.Users;
        }

        // GET: api/Users/5
        [ResponseType(typeof(Users))]
        public IHttpActionResult GetUsers(long id)
        {
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return NotFound();
            }

            return Ok(users);
        }

        // PUT: api/Users/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutUsers(long id, Users users)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != users.UserID)
            {
                return BadRequest();
            }

            db.Entry(users).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsersExists(id))
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

        // POST: api/Users
        [ResponseType(typeof(Users))]
        public IHttpActionResult PostUsers(Users users)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Users.Add(users);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (UsersExists(users.UserID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = users.UserID }, users);
        }

        // DELETE: api/Users/5
        [ResponseType(typeof(Users))]
        public IHttpActionResult DeleteUsers(long id)
        {
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return NotFound();
            }

            db.Users.Remove(users);
            db.SaveChanges();

            return Ok(users);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UsersExists(long id)
        {
            return db.Users.Count(e => e.UserID == id) > 0;
        }
    }
}