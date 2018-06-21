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
using static WebAPI.Controllers.Utils;

namespace WebAPI.Controllers
{
    public class ReviewersController : ApiController
    {
        private WebAPIEntities db = new WebAPIEntities();

        /// <summary>
        /// 返回CensorContent数据
        /// </summary>
        public class ReturnContent
        {
            public List<Dictionary<string, string>> Content { get; set; }
        }

        /// <summary>
        /// 传入审核判断
        /// </summary>
        public class InCensor
        {
            public string type { get; set; }
            public string id { get; set; }
            public string confirm { get; set; }
        }

        /// <summary>
        /// 审核者登录
        /// </summary>
        /// <param name="reviewer">
        /// eg:{"Name":"user1","Password":"123456"}
        /// </param>
        /// <returns>成功:{"Message":"success"};密码错误:{"Message":"密码错误"};用户不存在{"Message":"用户不存在"}
        /// </returns>
        [HttpPost, Route("Censor/Login")]
        public HttpResponseMessage GetCensoredContent(Reviewer reviewer)
        {
            Dictionary<string, string> res = new Dictionary<string, string>();
            Reviewer find = db.Reviewer.FirstOrDefault(Reviewer => Reviewer.Name == reviewer.Name);
            JavaScriptSerializer Json = new JavaScriptSerializer();
            if (find == null)
            {
                res.Add("Message", "用户不存在");
                return ConvertToJson(res);
            }
            else
            {
                if (reviewer.Password != find.Password)
                {
                    res.Add("Message", "密码错误");
                    return ConvertToJson(res);
                }
                else
                {
                    HttpCookie cookie = new HttpCookie("account");
                    cookie["role"] = "censor";
                    cookie["Name"] = find.Name;
                    cookie["ReviewerID"] = find.ReviewerID.ToString();
                    cookie.Expires = DateTime.Now.AddMinutes(120);
                    HttpContext.Current.Response.Cookies.Add(cookie);
                    res.Add("Message", "success");
                    return ConvertToJson(res);
                }
            }
        }

        /// <summary>
        /// 获取审核全部内容
        /// </summary>
        /// <returns>
        /// 成功:{"Message":"success","Data":{"Content":[{"id":"12333","type":"paper","detail":"English Paper"}]};
        /// 失败:{"Message":"failed"};
        /// 权限不足:{"Message":"forbidden"}</returns>
        [Route("Censor/Content")]
        public HttpResponseMessage GetCesoredCotent()
        {
            string role = HttpContext.Current.Request.Cookies["account"]["role"];
            if (role != "censor")
            {
                Dictionary<string, string> res = new Dictionary<string, string>();
                res.Add("Message", "forbidden");
                return ConvertToJson(res);
            }
            else
            {
                ReturnData<ReturnContent> res = new ReturnData<ReturnContent>();
                ReturnContent returnContent = new ReturnContent();
                returnContent.Content = new List<Dictionary<string, string>>();
                var results =
                    from Review in db.Review
                    select Review;
                //try
                //{
                    foreach (var result in results.Take(10))
                    {
                        string type="暂缺";
                        string detail="暂缺";
                        Dictionary<string, string> mid = new Dictionary<string, string>();
                        
                        //if (result.UserID != null)
                        //{
                        //    type = "user";
                        //    Users find = db.Users.FirstOrDefault(Users => Users.UserID == result.UserID);
                        //    detail = find.Nickname;
                        //}
                         if (result.PaperID != null)
                        {
                            mid.Add("id", result.PaperID.ToString());
                            type = "paper";
                            Paper find = db.Paper.FirstOrDefault(Paper => Paper.PaperID == result.PaperID);
                            detail = find.Title;
                        }
                        else if (result.PatentID != null)
                        {
                            mid.Add("id", result.PatentID.ToString());
                            type = "patent";
                            Patent find = db.Patent.FirstOrDefault(Patent => Patent.PatentID == result.PatentID);
                            detail = find.Title;
                        }
                        else if (result.CommentID != null)
                        {
                            type = "comment";
                            Comment find = db.Comment.FirstOrDefault(Comment => Comment.CommentID == result.CommentID);
                            detail = find.Content;
                        }
                        mid.Add("type", type);
                        mid.Add("detail",detail);
                        returnContent.Content.Add(mid);                        
                    }
                //}
                //catch
                //{
                //    res.Message = "failed";
                //    return ConvertToJson(res);
                //}
                res.Message = "success";
                res.Data = returnContent;
                return ConvertToJson(res);
            }
        }

        /// <summary>
        /// 获取审核论文内容
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// 成功:{"Message":"success","Data":{"Content":[{……}]};
        /// 失败:{"Message":"failed"};
        /// 权限不足:{"Message":"forbidden"}</returns>
        [HttpGet,Route("Censor/Paper")]
        public HttpResponseMessage GetCesoredPaper([FromUri]string id)
        {
            string role = HttpContext.Current.Request.Cookies["account"]["role"];
            if (role != "censor")
            {
                Dictionary<string, string> res = new Dictionary<string, string>();
                res.Add("Message", "forbidden");
                return ConvertToJson(res);
            }
            else
            {
                ReturnData<ReturnContent> res = new ReturnData<ReturnContent>();
                ReturnContent returnContent = new ReturnContent();
                returnContent.Content = new List<Dictionary<string, string>>();
                try
                {
                    Paper find = db.Paper.FirstOrDefault(Paper => Paper.PaperID.ToString() == id);
                    Dictionary<string, string> mid = new Dictionary<string, string>();
                    mid.Add("PaperID", find.PaperID.ToString());
                    mid.Add("Title", find.Title);
                    mid.Add("Abstract", find.Abstract);
                    mid.Add("ReferencedNum", find.ReferencedNum.ToString());
                    mid.Add("KeyWord", find.KeyWord);
                    mid.Add("IsFree", find.IsFree.ToString());
                    mid.Add("IsPass", find.IsPass.ToString());
                    mid.Add("Authors", find.Authors);
                    mid.Add("BaiduID", find.BaiduID);
                    mid.Add("PublishYear", find.PublishYear.ToString());
                    returnContent.Content.Add(mid);
                }
                catch
                {
                    res.Message = "failed";
                    return ConvertToJson(res);
                }
                res.Message = "success";
                res.Data = returnContent;
                return ConvertToJson(res);
            }
        }

        /// <summary>
        /// 获取审核专利内容
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// 成功:{"Message":"success","Data":{"Content":[{……}]};
        /// 失败:{"Message":"failed"};
        /// 权限不足:{"Message":"forbidden"}</returns>
        [Route("Censor/Patent")]
        public HttpResponseMessage GetCesoredPatent([FromUri]string id)
        {
            string role = HttpContext.Current.Request.Cookies["account"]["role"];
            if (role != "censor")
            {
                Dictionary<string, string> res = new Dictionary<string, string>();
                res.Add("Message", "forbidden");
                return ConvertToJson(res);
            }
            else
            {
                ReturnData<ReturnContent> res = new ReturnData<ReturnContent>();
                ReturnContent returnContent = new ReturnContent();
                returnContent.Content = new List<Dictionary<string, string>>();
                try
                {
                    Patent find = db.Patent.FirstOrDefault(Patent => Patent.PatentID.ToString() == id);
                    Dictionary<string, string> mid = new Dictionary<string, string>();
                    mid.Add("PatentID", find.PatentID.ToString());
                    mid.Add("Title", find.Title);
                    mid.Add("IPC", find.IPC.ToString());
                    mid.Add("Abstract", find.Abstract);
                    mid.Add("ApplyDate", find.ApplyDate.ToString());
                    mid.Add("Applicant", find.Applicant);
                    mid.Add("ApplicantAddress", find.ApplicantAddress.ToString());
                    mid.Add("SIPC", find.SIPC.ToString());
                    mid.Add("State", find.State);
                    mid.Add("Agencies", find.Agencies);
                    mid.Add("PublicNum", find.PublicNum);
                    mid.Add("IsPass", find.IsPass.ToString());
                    returnContent.Content.Add(mid);
                }
                catch
                {
                    res.Message = "failed";
                    return ConvertToJson(res);
                }
                res.Message = "success";
                res.Data = returnContent;
                return ConvertToJson(res);
            }
        }

        ///<summary>
        ///对审核的确认
        /// </summary>
        /// <param name="inCensor">eg:{"type":"type","id":"id","confirm":"true"}</param>
        /// <returns>
        /// 成功:{"Message":"success"};
        /// 失败:{"Message":"failed"};
        /// 权限不足:{"Message":"forbidden"}</returns>
        [HttpPost, Route("Censor/Confirm")]
        public HttpResponseMessage ConfirmCensor(InCensor inCensor)
        {
            string role = HttpContext.Current.Request.Cookies["account"]["role"];
            if (role != "censor")
            {
                Dictionary<string, string> res = new Dictionary<string, string>();
                res.Add("Message", "forbidden");
                return ConvertToJson(res);
            }
            else
            {
                ReturnData<ReturnContent> res = new ReturnData<ReturnContent>();
                try
                {
                    if (inCensor.type=="paper")
                    {
                        Paper find = db.Paper.FirstOrDefault(Paper => Paper.PaperID.ToString() == inCensor.id);
                        if (inCensor.confirm == "true")
                            find.IsPass = true;
                        else
                            find.IsPass = false;
                        Review find2= db.Review.FirstOrDefault(Review => Review.PaperID.ToString() == inCensor.id);
                        db.Review.Remove(find2);
                        db.SaveChanges();
                    }
                    else if(inCensor.type == "patent")
                    {
                        Patent find = db.Patent.FirstOrDefault(Patent => Patent.PatentID.ToString() == inCensor.id);
                        if (inCensor.confirm == "true")
                            find.IsPass = true;
                        else
                            find.IsPass = false;
                        Review find2 = db.Review.FirstOrDefault(Review => Review.PatentID.ToString() == inCensor.id);
                        db.Review.Remove(find2);
                        db.SaveChanges();
                    }
                    else if(inCensor.type == "comment")
                    {
                        Comment find = db.Comment.FirstOrDefault(Comment => Comment.CommentID.ToString() == inCensor.id);
                        if (inCensor.confirm == "true")
                            find.IsPass = true;
                        else
                            find.IsPass = false;
                        Review find2 = db.Review.FirstOrDefault(Review => Review.CommentID.ToString() == inCensor.id);
                        db.Review.Remove(find2);
                        db.SaveChanges();
                    }
                }
                catch
                {
                    res.Message = "failed";
                    return ConvertToJson(res);
                }
                res.Message = "success";
                return ConvertToJson(res);
            }
        }
    }
}