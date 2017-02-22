using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sasila.Common.Http
{
    /// <summary>
    /// 网页访问web
    /// </summary>
    public class HttpWebPage:HttpWeb
    {
        public HttpWebPage()
        {
            UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
        }
    }
}
