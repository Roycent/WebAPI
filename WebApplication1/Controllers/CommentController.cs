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
            ReturnData<List<Comment>> returndata = new ReturnData<List<Comment>>();
            if (type == "") { returndata.Message = "Type Error"; }
            else if (id == null) { returndata.Message = "ID Empty"; }
            var results =
                from Comment in db.Comment
                where Comment.TypeID == id && Comment.Type == type
                select Comment;
            foreach(var result in results)
            {returndata.Data.Add(result);}
            return ConvertToJson(returndata);
        }

        [HttpPost]
        [Route("User/Comment")]
        public HttpResponseMessage PostComment(Comment comment)
        {
            comment.Time = System.DateTime.Now;
            ReturnData<List<Comment>> returndata = new ReturnData<List<Comment>>();
            var cookie = HttpContext.Current.Request.Cookies["account"];
            if (cookie != null)
            {
                comment.UserID= long.Parse(cookie["userID"]);
                comment.IsPass = true;
            }
            returndata.Message = "No Loging";

            return ConvertToJson(returndata);
        }
    }
}
