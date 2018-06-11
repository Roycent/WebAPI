﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using System.Collections.Generic;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 返回Papers数据
    /// </summary>
    public class Returnpapers
    {
        public string type { get; set; }
        public List<Dictionary<string,string>> papers { get; set; }
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
    /// 
    /// </summary>
    public class ReturnPaper
    {
        public string access { get; set; }
        public Paper data { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ReturnPatent
    {
        public string access { get; set; }
        public Patent data { get; set; }
    }

    /// <summary>
    /// 这个当后面修改数据库的时候需要进行修改。
    /// </summary>
    public class ReturnExpert
    {
        public string access { get; set; }
        public ExpertInfo data { get; set; }
        public List<Paper> paperlist { get; set; }
        public List<Patent> paptentlist { get; set; }
    }
    public class RecourceController : ApiController
    {
        private WebAPIEntities db = new WebAPIEntities();

        [HttpPost]
        [Route("resource")]
        public string SearchSource(string type,string keywords)
        {
            JavaScriptSerializer Json = new JavaScriptSerializer();
            Dictionary<string, string> map = new Dictionary<string, string>();
            map.Add("error", "TypeError");
            if (type== "paper")
            {
                Returnpapers Papers = new Returnpapers();
                Papers.type = "paper";
                Papers.papers = new List<Dictionary<string, string>>();
                var results =
                from Paper in db.Paper
                where Paper.Title.IndexOf(keywords) != -1
                select Paper;
                foreach(var result in results)
                {
                    Dictionary<string, string> mid = new Dictionary<string, string>();
                    mid.Add("id", result.PaperID.ToString());
                    mid.Add("title", result.Title);
                    mid.Add("author", result.Users.UserName);
                    mid.Add("summary", result.Abstract);
                    Papers.papers.Add(mid);
                }
                return Json.Serialize(Papers);
            }
            if (type == "patent")
            {
                Returnpatents Patents = new Returnpatents();
                Patents.type = "patent";
                Patents.patents = new List<Dictionary<string, string>>();
                var results =
                from Patent in db.Patent
                where Patent.Title.IndexOf(keywords) != -1
                select Patent;
                foreach (var result in results)
                {
                    Dictionary<string, string> mid = new Dictionary<string, string>();
                    mid.Add("id", result.PatentID.ToString());
                    mid.Add("title", result.Title);
                    mid.Add("author", result.Users.UserName);
                    mid.Add("Abstract", result.Abstract);
                    Patents.patents.Add(mid);
                }
                return Json.Serialize(Patents);
            }
            if(type== "expert")
            {
                Returnexperts Expect = new Returnexperts();
                Expect.type = "expert";
                Expect.experts = new List<Dictionary<string, string>>();
                var results =
                from ExpertInfo in db.ExpertInfo
                where ExpertInfo.FirstName.IndexOf(keywords) != -1
                select ExpertInfo;
                foreach (var result in results)
                {
                    Dictionary<string, string> mid = new Dictionary<string, string>();
                    mid.Add("id", result.UserID.ToString());
                    mid.Add("firstname", result.FirstName);
                    mid.Add("listname", result.LastName);
                    mid.Add("workstation", result.Workstation);
                    Expect.experts.Add(mid);
                }
                return Json.Serialize(Expect);
            }
            return Json.Serialize(map);
        }

        [HttpPost]
        [Route("resource/paper")]
        public string RequestPaper(int id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            JavaScriptSerializer Json = new JavaScriptSerializer();
            ReturnPaper paper = new ReturnPaper();
            paper.access = "success";
            paper.data=db.Paper.Find(id);
            return Json.Serialize(paper);
        }

        [HttpPost]
        [Route("resource/patent")]
        public string RequestPatent(int id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            JavaScriptSerializer Json = new JavaScriptSerializer();
            ReturnPatent patent = new ReturnPatent();
            patent.access = "success";
            patent.data = db.Patent.Find(id);
            return Json.Serialize(patent);
        }

        [HttpPost]
        [Route("resource/expert")]
        public string GetExpertInformation(int id)
        {
            db.Configuration.ProxyCreationEnabled = false;//禁用外键防止循环引用。
            JavaScriptSerializer Json = new JavaScriptSerializer();
            ReturnExpert expert = new ReturnExpert();
            expert.paperlist = new List<Paper>();
            expert.paptentlist = new List<Patent>();
            expert.access = "succcess";
            expert.data = db.ExpertInfo.Find(id);
            //TODO：这要是找不到好的方法就直接用第一个函数的那种方法。
            return Json.Serialize(expert);
        }
    }
}
