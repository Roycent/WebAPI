using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 返回Paper数据
    /// </summary>
    public class Returnpapers
    {
        public string type { get; set; }
        public List<Paper> papers { get; set; }
    }

    /// <summary>
    /// 返回Patent数据
    /// </summary>
    public class Returnpatent
    {
        public string type { get; set; }
        public List<Patent> patents { get; set; }
    }

    /// <summary>
    /// 返回Patent数据
    /// </summary>
    public class Returnexpert
    {
        public string type { get; set; }
        public List<ExpertInfo> experts { get; set; }
    }

    public class RecourceController : ApiController
    {

        [Route("resource")]
        public string GetResource()
        {
            JavaScriptSerializer Json = new JavaScriptSerializer();
            Returnexpert expect = new Returnexpert();
            
            return Json.Serialize(expect);
        }
    }
}
