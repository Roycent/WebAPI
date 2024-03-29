﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace WebAPI.Controllers
{
    public class Utils
    {
        public static HttpResponseMessage ConvertToJson<T>(T res)
        {
            JavaScriptSerializer json = new JavaScriptSerializer();
            StringContent sc = new StringContent(json.Serialize(res), System.Text.Encoding.GetEncoding("UTF-8"), "applcation/json");
            return new HttpResponseMessage { Content = sc };
        }
    }
}