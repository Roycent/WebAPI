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
    /// Paper相关控制器 by 邢智涣
    /// </summary>
    public class PapersController : ApiController
    {
        private WebAPIEntities db = new WebAPIEntities();

        private long GenPaperID()
        {
            byte[] uuid = Guid.NewGuid().ToByteArray();
            return BitConverter.ToInt64(uuid, 0);
        }
        /// <summary>
        /// 文件上传
        /// 此接口为form-data形式的文件上传接口，title直接写在url中
        /// 如： post [www.xxx.com]/expert/UploadPaper?title=paperTitle
        /// paperID、UpID、Title、HasFullText会自动生成
        /// IPC、Abstract、Type、ReferencedNum、DOI、Publisher、Keyword、IsFree、Price等请调用ModifyPaperInfo接口进行设置
        /// </summary>
        /// <returns>上传paper后生成的paperID或失败</returns>
        [HttpPost]
        [Route("expert/UploadPaper")]
        public async Task<HttpResponseMessage> UploadPaper(string title)
        {
            long paperID = GenPaperID();
            Dictionary<string, string> res = new Dictionary<string, string>();
            try
            {
                var root = System.Web.Hosting.HostingEnvironment.MapPath("/paper");
                var provider = new MultipartFormDataStreamProvider(root);
                await Request.Content.ReadAsMultipartAsync(provider);

                foreach (var file in provider.FileData)
                {
                    string fileName = file.Headers.ContentDisposition.FileName.Trim('"');
                    string fileExt = fileName.Substring(fileName.LastIndexOf('.'));
                    FileInfo fileInfo = new FileInfo(file.LocalFileName);

                    string newFileName = paperID.ToString() + "-" + title + "-" + DateTime.Now.ToString().Replace(" ","-") + fileExt;
                    newFileName = newFileName.Replace("/", "-");
                    newFileName = newFileName.Replace(":", "-");
                    string saveUrl = Path.Combine(root, newFileName);
                    fileInfo.MoveTo(saveUrl);

                    Paper newPaper = new Paper
                    {
                        PaperID = paperID,
                        Title = title,
                        UpID = long.Parse(HttpContext.Current.Request.Cookies["account"]["UserID"]),
                        UpDate = DateTime.Now,
                        HasFullText = 1,
                        Address = saveUrl
                    };
                    db.Paper.Add(newPaper);
                    db.SaveChanges();
                }
                res.Add("Message", "success");
                res.Add("paperID", paperID.ToString());
                return ConvertToJson(res);
            }
            catch(Exception e)
            {
                res.Add("Message", "failed");
                res.Add("Details", e.Message);
                return ConvertToJson(res);
            }
        }

        /// <summary>
        /// 更新上传的文件
        /// </summary>
        /// <param name="paperID">
        /// 接口同上传文件接口,url中写入要更改的paperID
        /// 如： post [www.xxx.com]/expert/UpdatePaper?paperID=61234539523
        /// </param>
        /// <returns>Message: "success"或"failed" Details: 错误具体信息</returns>
        [HttpPost, Route("expert/UpdatePaper")]
        public async Task<HttpResponseMessage> UpdatePaper(long paperID)
        {
            Dictionary<string, string> res = new Dictionary<string, string>();
            Paper find = db.Paper.Find(paperID);
            if(find == null)
            {
                res.Add("Message", "failed");
                res.Add("Details", "paper not found");
                return ConvertToJson(res);
            }
            long userID = long.Parse(HttpContext.Current.Request.Cookies["account"]["userID"]);
            if(userID != find.UpID)
            {
                res.Add("Message", "forbidden");
                return ConvertToJson(res);
            }
            string title = find.Title;
            try
            {
                var root = System.Web.Hosting.HostingEnvironment.MapPath("/paper");
                var provider = new MultipartFormDataStreamProvider(root);
                await Request.Content.ReadAsMultipartAsync(provider);

                foreach (var file in provider.FileData)
                {
                    string fileName = file.Headers.ContentDisposition.FileName.Trim('"');
                    string fileExt = fileName.Substring(fileName.LastIndexOf('.'));
                    FileInfo fileInfo = new FileInfo(file.LocalFileName);

                    string newFileName = paperID.ToString() + "-" + title + "-" + DateTime.Now.ToString().Replace(" ", "-") + fileExt;
                    newFileName = newFileName.Replace("/", "-");
                    newFileName = newFileName.Replace(":", "-");
                    string saveUrl = Path.Combine(root, newFileName);
                    fileInfo.MoveTo(saveUrl);

                    find.Address = saveUrl;
                    db.SaveChanges();
                }
                res.Add("Message", "success");
                return ConvertToJson(res);
            }
            catch (Exception e)
            {
                res.Add("Message", "failed");
                res.Add("Details", e.Message);
                return ConvertToJson(res);
            }
        }

        /// <summary>
        /// 更改论文信息 也可以在首次上传后直接调用增加信息
        /// </summary>
        /// <param name="newPaper">Paper的各项需要手动提供的数据</param>
        /// <returns>"success"或"failed"</returns>
        [Route("expert/ModifyPaperInfo")]
        public HttpResponseMessage ModifyPaperInfo(Paper newPaper)
        {
            Dictionary<string, string> res = new Dictionary<string, string>();
            try
            {
                Paper find = db.Paper.Find(newPaper.PaperID);
                find.Abstract = newPaper.Abstract;
                find.IPC = newPaper.IPC;
                find.Type = newPaper.Type;
                find.ReferencedNum = newPaper.ReferencedNum;
                find.DOI = newPaper.DOI;
                find.Publisher = newPaper.Publisher;
                find.KeyWord = newPaper.KeyWord;
                find.IsFree = newPaper.IsFree;
                find.Price = newPaper.Price;
                db.SaveChanges();
            }
            catch(Exception e)
            {
                res.Add("Message", "failed");
                res.Add("Details", e.Message);
                return ConvertToJson(res);
            }

            res.Add("Message", "success");
            return ConvertToJson(res);
        }

        /// <summary>
        /// 删除paper,前提为登录者为paper上传者或管理员
        /// </summary>
        /// <param name="p">PaperID</param>
        /// <returns>"forbidden"无权限 "failed" 失败 "success" 成功</returns>
        [HttpPost, Route("expert/DeletePaper")]
        public HttpResponseMessage DeletePaper(Paper p)
        {
            Dictionary<string, string> res = new Dictionary<string, string>();
            long userID = long.Parse(HttpContext.Current.Request.Cookies["account"]["userID"]);
            string role = HttpContext.Current.Request.Cookies["account"]["role"];
            Paper find = db.Paper.Find(p.PaperID);
            if(userID != find.UpID && role != "admin")
            {
                res.Add("Message", "forbidden");
                return ConvertToJson(res);
            }
            if (find == null)
            {
                res.Add("Message", "failed");
                return ConvertToJson(res);

            }

            try
            {
                db.Paper.Remove(find);
                db.SaveChanges();
            }
            catch(Exception e)
            {
                res.Add("Message", "failed");
                res.Add("Details", e.Message);
            }
            res.Add("Message", "success");
            return ConvertToJson(res);
        }

    }
}