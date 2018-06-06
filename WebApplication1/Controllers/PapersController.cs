using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebAPI;

namespace WebAPI.Controllers
{
    public class PapersController : ApiController
    {
        private WebAPIEntities db = new WebAPIEntities();

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="fileContent"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("expert/paper")]
        public string UploadPaper([FromBody] byte[] fileContent)
        {
            if (fileContent != null)
            {
                string path = "/paper/";
                string fileName = Guid.NewGuid().ToString();
                string uploadResult = WriteFile(path, fileContent, fileName);
                if (uploadResult == "success")
                    return "success";
                else
                    return uploadResult;
            }
            return "failure";
        }

        public string WriteFile(string filePath, byte[] fileContent, string fileName)
        {
            DirectoryInfo di = new DirectoryInfo(filePath);
            if (!di.Exists)
                di.Create();
            FileStream fs = null;
            try
            {
                fs = File.Create(filePath + "\\" + fileName, fileContent.Length);
                fs.Write(fileContent, 0, fileContent.Length);
            }catch(Exception e)
            {
                return e.Message;
            }
            return "success";
        }

        // GET: api/Papers
        public IQueryable<Paper> GetPaper()
        {
            return db.Paper;
        }

        // GET: api/Papers/5
        [ResponseType(typeof(Paper))]
        public IHttpActionResult GetPaper(long id)
        {
            Paper paper = db.Paper.Find(id);
            if (paper == null)
            {
                return NotFound();
            }

            return Ok(paper);
        }

        // PUT: api/Papers/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutPaper(long id, Paper paper)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != paper.PaperID)
            {
                return BadRequest();
            }

            db.Entry(paper).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaperExists(id))
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

        // POST: api/Papers
        [ResponseType(typeof(Paper))]
        public IHttpActionResult PostPaper(Paper paper)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Paper.Add(paper);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (PaperExists(paper.PaperID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = paper.PaperID }, paper);
        }

        // DELETE: api/Papers/5
        [ResponseType(typeof(Paper))]
        public IHttpActionResult DeletePaper(long id)
        {
            Paper paper = db.Paper.Find(id);
            if (paper == null)
            {
                return NotFound();
            }

            db.Paper.Remove(paper);
            db.SaveChanges();

            return Ok(paper);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PaperExists(long id)
        {
            return db.Paper.Count(e => e.PaperID == id) > 0;
        }
    }
}