using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using WebAPI;
using static WebAPI.Controllers.Utils;

namespace WebAPI.Controllers
{
    /// <summary>
    /// patent相关控制器 by 邢智涣
    /// </summary>
    public class PatentsController : ApiController
    {
        private WebAPIEntities db = new WebAPIEntities();

        private long GenPatentID()
        {
            byte[] uuid = Guid.NewGuid().ToByteArray();
            return BitConverter.ToInt64(uuid, 0);
        }

        /// <summary>
        /// 上传Patent  参数及返回同上传Paper
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        [HttpPost,Route("Expert/UploadPatent")]
        public async Task<HttpResponseMessage> UploadPatent(string title)
        {
            long patentID = GenPatentID();
            Dictionary<string, string> res = new Dictionary<string, string>();
            try
            {
                var root = System.Web.Hosting.HostingEnvironment.MapPath("/patent");
                var provider = new MultipartFormDataStreamProvider(root);
                await Request.Content.ReadAsMultipartAsync(provider);

                foreach (var file in provider.FileData)
                {
                    string fileName = file.Headers.ContentDisposition.FileName.Trim('"');
                    string fileExt = fileName.Substring(fileName.LastIndexOf('.'));
                    FileInfo fileInfo = new FileInfo(file.LocalFileName);

                    string newFileName = patentID.ToString() + "-" + title + "-" + DateTime.Now.ToString().Replace(" ", "-") + fileExt;
                    newFileName = newFileName.Replace("/", "-");
                    newFileName = newFileName.Replace(":", "-");
                    string saveUrl = Path.Combine(root, newFileName);
                    fileInfo.MoveTo(saveUrl);

                    Patent newPatent = new Patent
                    {
                        PatentID = patentID,
                        Title = title,
                        UpID = long.Parse(HttpContext.Current.Request.Cookies["account"]["UserID"]),
                        UpDate = DateTime.Now,
                    };
                    db.Patent.Add(newPatent);
                    db.SaveChanges();
                }
                res.Add("Message", "success");
                res.Add("paperID", patentID.ToString());
                return ConvertToJson(res);
            }
            catch (Exception e)
            {
                res.Add("Message", "failed");
                res.Add("Details", e.Message);
                return ConvertToJson(res);
            }
        }

        [HttpPost,Route("Expert/ModifyPatentInfo")]
        public HttpResponseMessage ModifyPatentInfo(Patent newPatent)
        {
            Dictionary<string, string> res = new Dictionary<string, string>();
            try
            {
                var cookie = HttpContext.Current.Request.Cookies["account"];
                if (cookie == null)
                {
                    res.Add("Message", "failed");
                    res.Add("Details", "cookie error");
                    return ConvertToJson(res);
                }
                Patent find = db.Patent.Find(newPatent.PatentID);
                if (long.Parse(cookie["UserID"]) != find.UpID)
                {
                    res.Add("Message", "forbidden");
                    return ConvertToJson(res);
                }
                find.IPC = newPatent.IPC;
                find.Abstract = newPatent.Abstract;
                find.ApplyNum = newPatent.ApplyNum;
                find.ApplyDate = newPatent.ApplyDate;
                find.Applicant = newPatent.Applicant;
                find.ApplicantAddress = newPatent.ApplicantAddress;
                find.SIPC = newPatent.SIPC;
                find.State = newPatent.Agencies;
                find.Agent = newPatent.Agent;
                find.PublicNum = newPatent.PublicNum;
                db.SaveChanges();
                res.Add("Message", "success");
                return ConvertToJson(res);
            }catch(Exception e)
            {
                res.Add("Message", "failed");
                res.Add("Details", e.Message);
                return ConvertToJson(res);
            }
        }

    }
}