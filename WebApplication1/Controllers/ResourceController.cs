using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using static WebAPI.Controllers.Utils;

namespace WebAPI.Controllers
{
    public class ReturnData<T>
    {
        public string message { get; set; }
        public T data { get; set; }
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
    /// 这个当后面修改数据库的时候需要进行修改。
    /// </summary>
    public class ExpertData
    {
        public ExpertInfo expert;
        public List<Paper> PaperList;
        public List<Patent> PatentList;
    }

    public class ResourceController : ApiController
    {
        private WebAPIEntities db = new WebAPIEntities();

        [HttpGet]
        [Route("resource")]
        public HttpResponseMessage SearchSource(string type, string keywords)
        {
            JavaScriptSerializer Json = new JavaScriptSerializer();
            Dictionary<string, string> map = new Dictionary<string, string>();
            map.Add("error", "TypeError");
            if (type == "paper")
            {
                ReturnData<Returnpapers> returndata = new ReturnData<Returnpapers>();
                returndata.message = "success";
                returndata.data = new Returnpapers();
                returndata.data.type = "paper";
                returndata.data.papers = new List<Dictionary<string, string>>();
                var results =
                from Paper in db.Paper
                where Paper.Title.IndexOf(keywords) != -1
                select Paper;
                foreach (var result in results.Take(0))
                {
                    Dictionary<string, string> mid = new Dictionary<string, string>();
                    mid.Add("id", result.PaperID.ToString());
                    mid.Add("title", result.Title);
                    mid.Add("author", result.Authors);
                    mid.Add("publisher", result.Publisher);
                    mid.Add("keywords", result.KeyWord);
                    mid.Add("summary", result.Abstract);
                    returndata.data.papers.Add(mid);
                }
                return ConvertToJson(returndata);
            }
            if (type == "patent")
            {
                ReturnData<Returnpatents> returndata = new ReturnData<Returnpatents>();
                returndata.message = "success";
                returndata.data = new Returnpatents();
                returndata.data.type = "patent";
                returndata.data.patents = new List<Dictionary<string, string>>();
                var results =
                from Patent in db.Patent
                where Patent.Title.IndexOf(keywords) != -1
                select Patent;
                foreach (var result in results)
                {
                    Dictionary<string, string> mid = new Dictionary<string, string>();
                    mid.Add("id", result.PatentID.ToString());
                    mid.Add("title", result.Title);
                    mid.Add("time", result.ApplyDate.ToString());
                    mid.Add("publicnum", result.PublicNum);  
                    mid.Add("patentee", result.Applicant);
                    mid.Add("address", result.ApplicantAddress);
                    returndata.data.patents.Add(mid);
                }
                return ConvertToJson(returndata);
            }
            if (type == "expert")
            {
                ReturnData<Returnexperts> returndata = new ReturnData<Returnexperts>();
                returndata.message = "success";
                returndata.data = new Returnexperts();
                returndata.data.type = "expert";
                returndata.data.experts = new List<Dictionary<string, string>>();
                var results =
                from ExpertInfo in db.ExpertInfo
                where ExpertInfo.Name.IndexOf(keywords) != -1
                select ExpertInfo;
                
                foreach (var result in results)
                {
                    Dictionary<string, string> mid = new Dictionary<string, string>();
                    mid.Add("id", result.ExpertID.ToString());
                    mid.Add("name", result.Name);
                    mid.Add("workstation", result.Workstation);
                    mid.Add("field", result.Field);
                    mid.Add("timescited", result.TimesCited.ToString());
                    mid.Add("results", result.Results.ToString());
                    returndata.data.experts.Add(mid);
                }
                return ConvertToJson(returndata);
            }
            return ConvertToJson(map);
        }

        [HttpGet]
        [Route("resource/paper")]
        public HttpResponseMessage RequestPaper(int id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            JavaScriptSerializer Json = new JavaScriptSerializer();
            ReturnData<Paper> returndata = new ReturnData<Paper>();
            returndata.message = "success";
            returndata.data = db.Paper.Find(id);
            return ConvertToJson(returndata);
        }

        [HttpGet]
        [Route("resource/patent")]
        public HttpResponseMessage RequestPatent(int id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            JavaScriptSerializer Json = new JavaScriptSerializer();
            ReturnData<Patent> returndata = new ReturnData<Patent>();
            returndata.message = "success";
            returndata.data = db.Patent.Find(id);
            return ConvertToJson(returndata);
        }

        [HttpGet]
        [Route("resource/expert")]
        public HttpResponseMessage GetExpertInformation(int id)
        {
            db.Configuration.ProxyCreationEnabled = false;//禁用外键防止循环引用。
            JavaScriptSerializer Json = new JavaScriptSerializer();
            ReturnData<ExpertData> returndata = new ReturnData<ExpertData>();
            returndata.data = new ExpertData();
            returndata.data.PaperList = new List<Paper>();
            returndata.data.PatentList = new List<Patent>();
            returndata.message = "succcess";
            returndata.data.expert = db.ExpertInfo.Find(id);
            var papers =
            from ExpertPaper in db.ExpertPaper
            where ExpertPaper.ExpertID==id
            select ExpertPaper;
            foreach (var mid in papers)
            { returndata.data.PaperList.Add(mid.Paper);}
            var patents =
            from ExpertPatent in db.ExpertPatent
            where ExpertPatent.ExpertID == id
            select ExpertPatent;
            foreach (var mid in patents)
            { returndata.data.PatentList.Add(mid.Patent); }
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
        public class Raletion
        {
            public string ShcolarID;
            public List<string> list;
        }

        /// <summary>
        /// 爬虫接口PostExpert
        /// </summary>
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
        [HttpPost]
        [Route("resource/PostPaper")]
        public string PostPaper(Paper paper)
        {
            try
            {
                paper.PaperID = GenPaperID();
                if (paper.Abstract == "[]"){paper.Abstract = null;}
                if (paper.Title == "[]") { paper.Title = null; }
                if (paper.IPC == "[]") { paper.IPC = null; }
                if (paper.Type == "[]") { paper.Type = null; }
                if (paper.DOI == "[]") { paper.DOI = null; }
                if (paper.Publisher == "[]") { paper.Publisher = null; }
                if (paper.KeyWord == "[]") { paper.KeyWord = null; }
                if (paper.Address == "[]") { paper.Address = null; }
                if (paper.Authors == "[]") { paper.Authors = null; }
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
        /// 
        /// </summary>
        [HttpPost]
        [Route("resource/PostRaletion")]
        public string PostRalation(Raletion raletion)
        {
            try
            {
                foreach (var data in raletion.list)
                {
                    ExpertPaper EP = new ExpertPaper();
                    EP.ExpertID = db.ExpertInfo.FirstOrDefault(ExpertInfo => ExpertInfo.BaiduID == raletion.ShcolarID).ExpertID;
                    EP.PaperID = db.Paper.FirstOrDefault(Paper => Paper.BaiduID == data).PaperID;
                    db.ExpertPaper.Add(EP);
                    db.SaveChanges();
                }
                return "success";
            }
            catch (Exception ex)
            {
                return "error    " + ex.Message;
            }
        }
    }
}
