using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using static WebAPI.Controllers.Utils;

namespace WebAPI.Controllers
{
    public class ReturnData<T>
    {
        public string Message { get; set; }
        public T Data { get; set; }
    }

    /// <summary>
    /// 返回Paper数据
    /// </summary>
    public class Returnpaper
    {
        public bool access { get; set; }
        public Paper paper { get; set; }
    }
    /// <summary>
    /// 返回Papers数据
    /// </summary>
    public class Returnpapers
    {
        public string type { get; set; }
        public List<Dictionary<string, string>> papers { get; set; }
    }

    /// <summary>
    /// 返回Patents数据
    /// </summary>
    public class Returnpatents
    {
        public string type { get; set; }
        public List<Dictionary<string, string>> patents { get; set; }
    }

    /// <summary>
    /// 返回experts数据
    /// </summary>
    public class Returnexperts
    {
        public string type { get; set; }
        public List<Dictionary<string, string>> experts { get; set; }
    }

    /// <summary>
    /// 返回专家数据
    /// </summary>
    public class ExpertData
    {
        public ExpertInfo expert;
        public List<Paper> PaperList;
        public List<Patent> PatentList;
    }


    /// <summary>
    /// 
    /// </summary>
    public class Relation
    {
        public string ScholarID;
        public string PaperID;
    }

    public class ResourceController : ApiController
    {
        private WebAPIEntities db = new WebAPIEntities();


        /// <summary>
        /// 用于搜索资源，资源搜索API
        /// </summary>
        /// <param name="type"></param>
        /// <param name="keywords"></param>
        /// <param name="page"></param>
        /// <param name="sortBy"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("resource")]
        public HttpResponseMessage SearchSource(string type, string keywords, int page, string sortBy)
        {
            JavaScriptSerializer Json = new JavaScriptSerializer();
            ReturnData<Returnpapers> Rerurn = new ReturnData<Returnpapers>();
            Rerurn.Message = "type empty";

            if (type == "paper")
            { return PaperType(keywords, ref page, ref sortBy); }
            if (type == "patent")
            { return PatentType(keywords, ref page, ref sortBy); }
            if (type == "expert")
            { return ExpertType(keywords, ref page, ref sortBy); }

            return ConvertToJson(Rerurn);
        }


        private HttpResponseMessage ExpertType(string keywords, ref int page, ref string sortBy)
        {
            ReturnData<Returnexperts> returndata = new ReturnData<Returnexperts>();
            returndata.Message = "success";
            returndata.Data = new Returnexperts();
            returndata.Data.type = "expert";
            returndata.Data.experts = new List<Dictionary<string, string>>();
            if (keywords == "") { return ConvertToJson(returndata); }
            if (page == null) { page = 1; }

            var results =
            from ExpertInfo in db.ExpertInfo
            where ExpertInfo.Name.IndexOf(keywords) != -1
            orderby ExpertInfo.Results descending
            select ExpertInfo;

            foreach (var result in results.Skip((page - 1) * 20).Take(20))
            {
                Dictionary<string, string> mid = new Dictionary<string, string>();
                mid.Add("id", result.ExpertID.ToString());
                mid.Add("name", result.Name);
                mid.Add("workstation", result.Workstation);
                mid.Add("field", result.Field);
                mid.Add("timescited", result.TimesCited.ToString());
                mid.Add("results", result.Results.ToString());
                returndata.Data.experts.Add(mid);
            }
            return ConvertToJson(returndata);
        }
        //TODO：下面两个的函数没有写按照什么排序，全部按照默认来排序的。以后要写一下
        private HttpResponseMessage PatentType(string keywords, ref int page, ref string sortBy)
        {
            ReturnData<Returnpatents> returndata = new ReturnData<Returnpatents>();
            returndata.Message = "success";
            returndata.Data = new Returnpatents();
            returndata.Data.type = "patent";
            returndata.Data.patents = new List<Dictionary<string, string>>();
            if (keywords == "") { return ConvertToJson(returndata); }
            if (page == null) { page = 1; }

            var results =
            from Patent in db.Patent
            where Patent.Title.IndexOf(keywords) != -1
            orderby Patent.ApplyDate descending
            select Patent;
            foreach (var result in results.Skip((page - 1) * 20).Take(20))
            {
                Dictionary<string, string> mid = new Dictionary<string, string>();
                mid.Add("id", result.PatentID.ToString());
                mid.Add("title", result.Title);
                mid.Add("time", result.ApplyDate.ToString());
                mid.Add("publicnum", result.PublicNum);
                mid.Add("patentee", result.Applicant);
                mid.Add("address", result.ApplicantAddress);
                returndata.Data.patents.Add(mid);
            }
            return ConvertToJson(returndata);
        }
        private HttpResponseMessage PaperType(string keywords, ref int page, ref string sortBy)
        {
            ReturnData<Returnpapers> returndata = new ReturnData<Returnpapers>();
            returndata.Message = "success";
            returndata.Data = new Returnpapers();
            returndata.Data.type = "paper";
            returndata.Data.papers = new List<Dictionary<string, string>>();
            if (keywords == "") { return ConvertToJson(returndata); }
            if (page == null) { page = 1; }
            if (sortBy == "") { sortBy = "Reference"; }

            var results =
            from Paper in db.Paper
            where Paper.Title.IndexOf(keywords) != -1
            orderby Paper.ReferencedNum descending
            select Paper;
            foreach (var result in results.Skip((page - 1) * 20).Take(20))
            {
                Dictionary<string, string> mid = new Dictionary<string, string>();
                mid.Add("id", result.PaperID.ToString());
                mid.Add("title", result.Title);
                mid.Add("author", result.Authors);
                mid.Add("publisher", result.Publisher);
                mid.Add("keywords", result.KeyWord);
                mid.Add("summary", result.Abstract);
                returndata.Data.papers.Add(mid);
            }
            return ConvertToJson(returndata);
        }


        /// <summary>
        /// 获取对应id的paper的详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("resource/paper")]
        public HttpResponseMessage RequestPaper(long id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            JavaScriptSerializer Json = new JavaScriptSerializer();
            ReturnData<Returnpaper> returndata = new ReturnData<Returnpaper>();
            returndata.Data = new Returnpaper();
            returndata.Message = "success";
            returndata.Data.access = false;
            var cookie = HttpContext.Current.Request.Cookies["account"];
            if (cookie != null && cookie["role"].ToString() == "user")
            {
                long userID = long.Parse(cookie["UserID"].ToString());
                var result =
                    from Download in db.Download
                    where Download.UserID == userID && Download.PaperID == id
                    select Download;
                if (result != null) { returndata.Data.access = true; }
            }
            returndata.Data.paper = db.Paper.Find(id);
            returndata.Data.paper.KeyWord = returndata.Data.paper.KeyWord.Replace("[", "").Replace("]", "").Replace("'", "");
            return ConvertToJson(returndata);
        }

        /// <summary>
        /// 获取对应id的patent的详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("resource/patent")]
        public HttpResponseMessage RequestPatent(long id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            JavaScriptSerializer Json = new JavaScriptSerializer();
            ReturnData<Patent> returndata = new ReturnData<Patent>();
            returndata.Message = "success";
            returndata.Data = db.Patent.Find(id);
            return ConvertToJson(returndata);
        }

        /// <summary>
        /// 获取对应id的expert的详细信息,包含论文等
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[HttpGet]
        //[Route("resource/expert")]
        //public HttpResponseMessage GetExpertInformation(long id)
        //{
        //    db.Configuration.ProxyCreationEnabled = false;//禁用外键防止循环引用。
        //    JavaScriptSerializer Json = new JavaScriptSerializer();
        //    ReturnData<ExpertData> returndata = new ReturnData<ExpertData>();
        //    returndata.Data = new ExpertData();
        //    returndata.Data.PaperList = new List<Paper>();
        //    returndata.Data.PatentList = new List<Patent>();
        //    returndata.Message = "succcess";
        //    returndata.Data.expert = db.ExpertInfo.Find(id);
        //    var papers =
        //    from ExpertPaper in db.ExpertPaper
        //    where ExpertPaper.ExpertID == id
        //    select ExpertPaper;
        //    foreach (var mid in papers)
        //    { returndata.Data.PaperList.Add(mid.Paper); }
        //    var patents =
        //    from ExpertPatent in db.ExpertPatent
        //    where ExpertPatent.ExpertID == id
        //    select ExpertPatent;
        //    foreach (var mid in patents)
        //    { returndata.Data.PatentList.Add(mid.Patent); }
        //    return ConvertToJson(returndata);
        //}


        //TODO:还有三个接口不着急弄。将上个接口拆分的接口。
        [HttpGet]
        [Route("resource/expert")]
        public HttpResponseMessage GetExpert(long id)
        {
            db.Configuration.ProxyCreationEnabled = false;//禁用外键防止循环引用。
            JavaScriptSerializer Json = new JavaScriptSerializer();
            ReturnData<ExpertInfo> returndata = new ReturnData<ExpertInfo>();
            returndata.Data = new ExpertInfo();
            returndata.Message = "succcess";
            returndata.Data = db.ExpertInfo.Find(id);
            return ConvertToJson(returndata);
        }

        [HttpGet]
        [Route("resource/expert/patent")]
        public HttpResponseMessage GetPatent(long id)
        {
            db.Configuration.ProxyCreationEnabled = false;//禁用外键防止循环引用。
            JavaScriptSerializer Json = new JavaScriptSerializer();
            ReturnData<ExpertData> returndata = new ReturnData<ExpertData>();

            return ConvertToJson(returndata);
        }

        [HttpGet]
        [Route("resource/expert/paper")]
        public HttpResponseMessage GetPaper(long id)
        {
            db.Configuration.ProxyCreationEnabled = false;//禁用外键防止循环引用。
            JavaScriptSerializer Json = new JavaScriptSerializer();
            ReturnData<ExpertData> returndata = new ReturnData<ExpertData>();

            return ConvertToJson(returndata);
        }

        private long GenExpertID()
        {
            Random rd = new Random();
            int r = rd.Next(1000);
            long ExpertID = (long)(DateTime.Now.ToUniversalTime() - new DateTime(2018, 3, 24)).TotalMilliseconds + r;
            ExpertInfo tryFind = db.ExpertInfo.Find(ExpertID);
            while (tryFind != null)
            {
                ExpertID = GenExpertID();
                tryFind = db.ExpertInfo.Find(ExpertID);
            }
            return ExpertID;
        }
        private long GenPaperID()
        {
            Random rd = new Random();
            int r = rd.Next(1000);
            long PaperID = (long)(DateTime.Now.ToUniversalTime() - new DateTime(2018, 3, 24)).TotalMilliseconds + r;
            Paper tryFind = db.Paper.Find(PaperID);
            while (tryFind != null)
            {
                PaperID = GenPaperID();
                tryFind = db.Paper.Find(PaperID);
            }
            return PaperID;
        }
        private long GenPatentID()
        {
            Random rd = new Random();
            int r = rd.Next(1000);
            long PatentID = (long)(DateTime.Now.ToUniversalTime() - new DateTime(2018, 3, 24)).TotalMilliseconds + r;
            Patent tryFind = db.Patent.Find(PatentID);
            while (tryFind != null)
            {
                PatentID = GenPatentID();
                tryFind = db.Patent.Find(PatentID);
            }
            return PatentID;
        }


        /// <summary>
        /// 爬虫接口PostExpert
        /// </summary>
        /// <param name="expert"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("resource/PostExpert")]
        public string PostExpert(ExpertInfo expert)
        {
            try
            {
                expert.ExpertID=GenExpertID();
                db.ExpertInfo.Add(expert);
                db.SaveChanges();
                return "success";
            }
            catch (Exception ex)
            {
                return "error    "+ex.Message;
            }
        }


        /// <summary>
        /// 爬虫接口PostPaper
        /// </summary>
        /// <param name="paper">
        /// 
        /// </param>
        /// <returns></returns>
        [HttpPost]
        [Route("resource/PostPaper")]
        public string PostPaper(Paper paper)
        {
            try
            {
                paper.PaperID = GenPaperID();
                db.Paper.Add(paper);
                db.SaveChanges();
                return "success";
            }
            catch (Exception ex)
            {
                return "error    " + ex.Message;
            }
        }


        /// <summary>
        /// 爬虫接口PostPatent
        /// </summary>
        /// <param name="patent"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("resource/PostPatent")]
        public string PostPatent(Patent patent)
        {
            try
            {
                patent.PatentID = GenPatentID();
                db.Patent.Add(patent);
                db.SaveChanges();
                return "success";
            }
            catch (Exception ex)
            {
                return "error    " + ex.Message;
            }
        }


        /// <summary>
        /// 爬虫接口PostRaletion
        /// </summary>
        /// <param name="relation"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("resource/PostRelation")]
        public string PostRalation(string ScholarID,string PaperID)
        {
            try
            {
                ExpertPaper EP = new ExpertPaper();
                EP.ExpertID = db.ExpertInfo.FirstOrDefault(ExpertInfo => ExpertInfo.BaiduID == ScholarID).ExpertID;
                EP.PaperID = db.Paper.FirstOrDefault(Paper => Paper.BaiduID == PaperID).PaperID;
                db.ExpertPaper.Add(EP);
                db.SaveChanges();
                return "success";
            }
            catch (Exception ex)
            {
                return "error    " + ex.Message;
            }
        }
    }
}
