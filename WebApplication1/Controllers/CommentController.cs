using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using static WebAPI.Controllers.Utils;

namespace WebAPI.Controllers
{
    public class CommentController : ApiController
    {
        private WebAPIEntities db = new WebAPIEntities();

        [HttpGet]
        [Route("resource/comment")]
        public HttpResponseMessage GetComment(long id,string type)
        {
            db.Configuration.ProxyCreationEnabled = false;
            ReturnData<List<Comment>> returndata = new ReturnData<List<Comment>>();
            returndata.Data = new List<Comment>();
            if (type == "") { returndata.Message = "Type Error"; }
            else if (id == null) { returndata.Message = "ID Empty"; }
            else
            {
                returndata.Message = "success";
                var results =
                    from Comment in db.Comment
                    where Comment.TypeID == id && Comment.Type == type
                    select Comment;
                foreach(var result in results)
                {
                    result.Review = null;
                    result.Users = null;
                    if (result.IsPass==true) { returndata.Data.Add(result); }
                }
            }
            return ConvertToJson(returndata);
        }

        [HttpPost]
        [Route("User/Comment")]
        public HttpResponseMessage PostComment(Comment comment)
        {
            comment.Time = System.DateTime.Now;
            ReturnData<string> returndata = new ReturnData<string>();
            var cookie = HttpContext.Current.Request.Cookies["account"];
            if (cookie != null)
            {
                comment.UserID= long.Parse(cookie["userID"]);
                comment.IsPass = true;
                db.Comment.Add(comment);
                db.SaveChanges();
                returndata.Message = "success";
                return ConvertToJson(returndata);
            }
            returndata.Message = "No Loging";

            return ConvertToJson(returndata);
        }
    }
}
