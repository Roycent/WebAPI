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
    public class ExpertInfoesController : ApiController
    {
        private WebAPIEntities db = new WebAPIEntities();

        /// <summary>
        /// 返回专家数据（从cookie中读取userID）
        /// </summary>
        /// <returns>
        /// Message:success/Message:error,Details:error infomation  Data:专家数据信息
        /// </returns>
        [HttpPost, Route("Expert/Info")]
        public HttpResponseMessage GetExpertInfo()
        {
            Dictionary<string, string> res = new Dictionary<string, string>();
            var cookie = HttpContext.Current.Request.Cookies["account"];
            if (cookie == null)
            {
                res.Add("Message", "error");
                res.Add("Details", "cookie error");
                return ConvertToJson(res);
            }
            long userID = long.Parse(HttpContext.Current.Request.Cookies["account"]["userID"]);
            UserExpert ue = db.UserExpert.FirstOrDefault(UserExpert => UserExpert.UserID == userID);
            if (ue == null)
            {
                res.Add("Message", "error");
                res.Add("Details", "not an expert");
                return ConvertToJson(res);
            }
            long expertID = (long)ue.ExpertID;
            ExpertInfo ei = db.ExpertInfo.Find(expertID);
            res.Add("Message", "success");

            db.Configuration.ProxyCreationEnabled = false;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("Name", ei.Name);
            data.Add("Workstation", ei.Workstation);
            data.Add("TimesCited", ei.TimesCited.ToString());
            data.Add("Results", ei.Results.ToString());
            data.Add("PUJournals", ei.PUJournals.ToString());
            data.Add("CSCDJournals", ei.CSCDJournals.ToString());
            data.Add("CTJournals", ei.CTJournals.ToString());
            data.Add("EIJournals", ei.EIJournals.ToString());
            data.Add("SCIEJournals", ei.SCIEJournals.ToString());
            data.Add("SSCIJournals", ei.SSCIJournals.ToString());
            data.Add("OtherJournals", ei.OtherJournals.ToString());
            data.Add("ConferencePapers", ei.ConferencePapers.ToString());
            data.Add("Field", ei.Field);
            data.Add("Books", ei.Books.ToString());
            data.Add("Others", ei.Others.ToString());
            string r = serializer.Serialize(data);
            res.Add("Data", r);
            return ConvertToJson(res);
        }

        /// <summary>
        /// 修改专家信息（改什么传什么）
        /// </summary>
        /// <param name="info">Name, Workstation, Field</param>
        /// <returns>Message:success / Message:error,Details:xxx</returns>
        [HttpPost, Route("Expert/ModifyInfo")]
        public HttpResponseMessage ModifyExpertInfo(ExpertInfo info)
        {
            Dictionary<string, string> res = new Dictionary<string, string>();
            var cookie = HttpContext.Current.Request.Cookies["account"];
            if (cookie == null)
            {
                res.Add("Message", "error");
                res.Add("Details", "cookie error");
                return ConvertToJson(res);
            }
            long userID = long.Parse(HttpContext.Current.Request.Cookies["account"]["userID"]);
            UserExpert ue = db.UserExpert.FirstOrDefault(UserExpert => UserExpert.UserID == userID);
            if (ue == null)
            {
                res.Add("Message", "error");
                res.Add("Details", "not an expert");
                return ConvertToJson(res);
            }
            long expertID = (long)ue.ExpertID;
            ExpertInfo ei = db.ExpertInfo.Find(expertID);
            if(ei.Name != null)
                ei.Name = info.Name;
            if(ei.Workstation != null)
                ei.Workstation = info.Workstation ;
            if(ei.Field != null)
                ei.Field = info.Field;
            db.SaveChanges();
            res.Add("Message", "success");
            return ConvertToJson(res);

        }

        /// <summary>
        /// 返回专家Paper
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("Expert/GetPapers")]
        public HttpResponseMessage GetPapersInfo()
        {
            Dictionary<string, string> res = new Dictionary<string, string>();
            var cookie = HttpContext.Current.Request.Cookies["account"];
            if (cookie == null)
            {
                res.Add("Message", "error");
                res.Add("Details", "cookie error");
                return ConvertToJson(res);
            }
            long userID = long.Parse(HttpContext.Current.Request.Cookies["account"]["userID"]);
            UserExpert ue = db.UserExpert.FirstOrDefault(UserExpert => UserExpert.UserID == userID);
            if (ue == null)
            {
                res.Add("Message", "error");
                res.Add("Details", "not an expert");
                return ConvertToJson(res);
            }
            long expertID = (long)ue.ExpertID;
            int paperCount = db.ExpertPaper.Count(ExpertPaper => ExpertPaper.ExpertID == expertID);
            Dictionary<string, string> data = new Dictionary<string, string>();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            if (paperCount == 0)
            {
                data.Add("number", "0");
                data.Add("paper", "[]");
                res.Add("Message", "success");
                res.Add("Data", serializer.Serialize(data));
                return ConvertToJson(res);
            }
            string papers = "[";
            var eps =
                from ExpertPaper in db.ExpertPaper
                where ExpertPaper.ExpertID == expertID
                select ExpertPaper;
            bool first = true;
            foreach (var ep in eps)
            {
                if (first != true)
                    papers = papers + ",";
                Dictionary<string, string> paperData = new Dictionary<string, string>();
                Paper paper = db.Paper.FirstOrDefault(Paper => Paper.PaperID == ep.PaperID);
                paperData.Add("Id", paper.PaperID.ToString());
                paperData.Add("Title", paper.Title);
                paperData.Add("Abstract", paper.Abstract);
                paperData.Add("Publisher", paper.Publisher);
                papers = papers + serializer.Serialize(paperData);
                first = false;
            }
            papers += "]";
            data.Add("number", paperCount.ToString());
            data.Add("papers", papers);
            res.Add("Message", "success");
            res.Add("Data", serializer.Serialize(data));
            return ConvertToJson(res);
            
        }

        /// <summary>
        /// 返回专家Patent
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("Expert/GetPatent")]
        public HttpResponseMessage GetPatentInfo()
        {
            Dictionary<string, string> res = new Dictionary<string, string>();
            var cookie = HttpContext.Current.Request.Cookies["account"];
            if (cookie == null)
            {
                res.Add("Message", "error");
                res.Add("Details", "cookie error");
                return ConvertToJson(res);
            }
            long userID = long.Parse(HttpContext.Current.Request.Cookies["account"]["userID"]);
            UserExpert ue = db.UserExpert.FirstOrDefault(UserExpert => UserExpert.UserID == userID);
            if (ue == null)
            {
                res.Add("Message", "error");
                res.Add("Details", "not an expert");
                return ConvertToJson(res);
            }
            long expertID = (long)ue.ExpertID;
            int paperCount = db.ExpertPatent.Count(ExpertPatent => ExpertPatent.ExpertID == expertID);
            Dictionary<string, string> data = new Dictionary<string, string>();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            if (paperCount == 0)
            {
                data.Add("number", "0");
                data.Add("paper", "[]");
                res.Add("Data", serializer.Serialize(data));
                return ConvertToJson(res);
            }
            string papers = "[";
            var eps =
                from ExpertPatent in db.ExpertPatent
                where ExpertPatent.ExpertID == expertID
                select ExpertPatent;
            bool first = true;
            foreach (var ep in eps)
            {
                if (first != true)
                    papers = papers + ",";
                Dictionary<string, string> paperData = new Dictionary<string, string>();
                Patent paper = db.Patent.FirstOrDefault(Patent => Patent.PatentID == ep.PatentID);
                paperData.Add("Id", paper.PatentID.ToString());
                paperData.Add("Title", paper.Title);
                paperData.Add("Abstract", paper.Abstract);
                paperData.Add("Publisher", paper.ApplyDate.ToString());
                papers = papers + serializer.Serialize(paperData);
                first = false;
            }
            papers += "]";
            data.Add("number", paperCount.ToString());
            data.Add("Data", papers);
            res.Add("Message", "success");
            res.Add("Data", serializer.Serialize(data));
            return ConvertToJson(res);

        }
    }
}