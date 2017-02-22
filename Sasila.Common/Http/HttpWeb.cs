using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.IO.Compression;
using System.IO;
using Sasila.Common.Tools;
using System.Text.RegularExpressions;
using System.Threading;
using System.Diagnostics;

namespace Sasila.Common.Http
{
    /// <summary>
    /// 更换代理委托
    /// </summary>
    /// <param name="msg"></param>
    /// <returns></returns>
    public delegate bool ChangeProxyCallback(out object msg);
    /// <summary>
    /// http获取网页内容抽象类
    /// </summary>
    public abstract class HttpWeb
    {
        /// <summary>
        /// 构造函数，初始化部分默认参数
        /// </summary>
        public HttpWeb()
        {
            HttpHeader = new Dictionary<string, string>();
            ReplaceDic = new Dictionary<string, string>();
            myCookie = new CookieContainer();
            myCookie.Capacity = 1024;
            myCookie.PerDomainCapacity = 1024;
            WebSet = new HttpWebSet();
            PostStreamEncoding = Encoding.UTF8;
            LocationUrl = string.Empty;
            ResponseUrl = string.Empty;
            ReUrl = string.Empty;
            TimeOut = 15000;
            throwException = false;
            AutoRedirect = true;
            OnlyAsyncRequest = false;
            AsyncResponse = false;
            ContentType = "application/x-www-form-urlencoded;";
            UserAgent = string.Empty;
            AddStrCookiesInHeader = false;
            IsXMLHttpRequest = false;
            IsGzipEncoding = false;
            Iszh_CNLanguage = false;
            FilterNullCookie = false;
            IsFiddler = false;
            KeepAlive = true;
            //设置并发链接数量
            if (ServicePointManager.DefaultConnectionLimit < 512)
            {
                ServicePointManager.DefaultConnectionLimit = 512;
            }
        }
        /// <summary>
        /// 是否Fiddler模式
        /// </summary>
        public bool IsFiddler { get; set; }
        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMsg { get; set; }
        /// <summary>
        /// 更换代理事件
        /// </summary>
        public event ChangeProxyCallback ChangeDisabledProxy;

        #region cookie处理

        /// <summary>
        /// cookie容器
        /// </summary>
        protected CookieContainer myCookie { get; set; }

        /// <summary>
        /// 添加cookie集合
        /// </summary>
        /// <param name="cookies">CookieCollection</param>
        public void SetCookie(CookieCollection cookies)
        {
            myCookie.Add(cookies);
        }
        /// <summary>
        /// 设置字符串cookies
        /// </summary>
        /// <param name="strCookie">格式cookiename=cookievalue;Domain=domain; Path=path,</param>
        public void SetCookies(string strCookie)
        {
            strCookie = strCookie.Replace(" ", "");
            string[] spits = strCookie.Split(',');
            string tempDomain = StrHelper.Abstract(strCookie, "Domain=", ";");
            foreach (string c in spits)
            {
                if (!string.IsNullOrEmpty(c))
                {
                    string name = StrHelper.Abstract(c, "", "=");
                    string value = StrHelper.Abstract(c, "=", ";");
                    string path = GetPath(c);
                    if (string.IsNullOrEmpty(path))
                        path = "/";
                    string domain = GetDomain(c);
                    if (string.IsNullOrEmpty(domain))
                    {
                        domain = tempDomain;
                    }
                    if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(value) || string.IsNullOrEmpty(path) || string.IsNullOrEmpty(domain))
                    {
                        continue;
                    }
                    else
                    {
                        try
                        {
                            domain = SpecialRules(domain);//特殊替换规则
                            Cookie cook = new Cookie(name, value, path, domain);
                            myCookie.Add(cook);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(string.Format("{0}-{1}\r\n", e.Message, c));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 特殊替换规则
        /// </summary>
        private string SpecialRules(string domain)
        {
            if (null != ReplaceDic)
            {
                foreach (KeyValuePair<string, string> item in ReplaceDic)
                {
                    if (domain == item.Key)
                    {
                        domain = item.Value;
                        break;
                    }
                }
            }
            return domain;
        }
        private string GetDomain(string cookieStr)
        {
            string subStr = "Domain=";
            if (!cookieStr.Contains(subStr))
                return "";
            string domain = StrHelper.Abstract(cookieStr, subStr, ";");
            if (string.IsNullOrEmpty(domain))
            {
                domain = StrHelper.Abstract(cookieStr, subStr, ",");
            }
            while (domain.Contains(','))
            {
                domain = StrHelper.Abstract(domain, "", ",");
            }
            while (domain.Contains(';'))
            {
                domain = StrHelper.Abstract(domain, "", ";");
            }
            if (string.IsNullOrEmpty(domain))
                domain = StrHelper.Abstract(cookieStr, subStr, "");
            return domain;
        }
        private string GetPath(string cookieStr)
        {
            string subStr = "Path=";
            if (!cookieStr.Contains(subStr))
                return "";
            string path = StrHelper.Abstract(cookieStr, subStr, ";");
            if (string.IsNullOrEmpty(path))
            {
                path = StrHelper.Abstract(cookieStr, subStr, ",");
            }
            while (path.Contains(','))
            {
                path = StrHelper.Abstract(path, "", ",");
            }
            while (path.Contains(';'))
            {
                path = StrHelper.Abstract(path, "", ";");
            }
            if (string.IsNullOrEmpty(path))
                path = StrHelper.Abstract(cookieStr, subStr, "");
            return path;
        }
        private void SetCookies(Uri uri, string strCookie)
        {
            if (uri != null && !string.IsNullOrEmpty(strCookie))
            {
                Regex reg = new Regex(@"(e|E)(x|X)(p|P)(i|I)(r|R)(e|E)(s|S)=[^;]*GMT;");
                strCookie = reg.Replace(strCookie, "");
                reg = new Regex(@"(e|E)(x|X)(p|P)(i|I)(r|R)(e|E)(s|S)=[^;]*GMT,");
                strCookie = reg.Replace(strCookie, "");
                reg = new Regex(@"(e|E)(x|X)(p|P)(i|I)(r|R)(e|E)(s|S)=[^;]*GMT");
                strCookie = reg.Replace(strCookie, "");
                reg = new Regex(@"(e|E)(x|X)(p|P)(i|I)(r|R)(e|E)(s|S)=[^;]*,");
                strCookie = reg.Replace(strCookie, "");

                reg = new Regex(@"(e|E)(x|X)(p|P)(i|I)(r|R)(e|E)(s|S)=[^;]*");
                strCookie = reg.Replace(strCookie, "");

                reg = new Regex(@";(s|S)(e|E)(c|C)(u|U)(r|R)(e|E)");
                strCookie = reg.Replace(strCookie, "");
                reg = new Regex(@";[ ](h|H)(t|T)(t|T)(p|P)(o|O)(n|N)(l|L)(y|Y)");
                strCookie = reg.Replace(strCookie, "");
                reg = new Regex(@"(h|H)(t|T)(t|T)(p|P)(o|O)(n|N)(l|L)(y|Y)");
                strCookie = reg.Replace(strCookie, "");
                reg = new Regex(@"(p|P)(a|A)(t|T)(h|H)=");
                strCookie = reg.Replace(strCookie, "Path=");
                reg = new Regex(@"(d|D)(o|O)(m|M)(a|A)(i|I)(n|N)=");
                strCookie = reg.Replace(strCookie, "Domain=");
                //组装成标准格式
                reg = new Regex(@"(p|P)(a|A)(t|T)(h|H)=[^(;|,)]*(;|,)");
                string tempCookie = reg.Replace(strCookie, "");
                reg = new Regex(@"(d|D)(o|O)(m|M)(a|A)(i|I)(n|N)=[^(;|,)]*(;|,)");
                tempCookie = reg.Replace(tempCookie, "");
                tempCookie = tempCookie.Replace(" ", "");
                tempCookie = tempCookie.Replace(",", ";");
                string[] tcs = tempCookie.Split(';');
                string resultCookie = "";
                string tempDomain = GetDomain(strCookie);
                string tempPath = "/";
                if (string.IsNullOrEmpty(tempDomain))
                {
                    tempDomain = "." + uri.Host.Abstract(".", "");
                }
                for (int i = 0; i < tcs.Length; i++)
                {
                    string str = tcs[i];
                    if (string.IsNullOrEmpty(str) || str.Contains("Domain=") || str.Contains("Path=") || str.Contains("Max-Age"))
                        continue;
                    if (FilterNullCookie)
                    {
                        reg = new Regex(@"=(n|N)(u|U)(l|L)(l|L)");
                        if (reg.IsMatch(str))
                        {
                            continue;
                        }
                    }
                    str = str.Replace(",", "");
                    str = str.Replace(";", "");
                    if (strCookie.Contains(str))
                    {
                        strCookie = StrHelper.Abstract(strCookie, str, "");
                    }
                    string domain = GetDomain(strCookie);
                    if (string.IsNullOrEmpty(domain))
                        domain = tempDomain;
                    string path = GetPath(strCookie);
                    if (string.IsNullOrEmpty(path))
                        path = tempPath;
                    if (!str.Contains("="))
                        continue;
                    if (string.IsNullOrEmpty(resultCookie))
                    {
                        resultCookie += string.Format("{0};Domain={1};Path={2}", str, domain, path);
                    }
                    else
                    {
                        resultCookie += string.Format(",{0};Domain={1};Path={2}", str, domain, path);
                    }
                }
                SetCookies(resultCookie);
            }
        }
        /// <summary>
        /// 获取cookie容器
        /// </summary>
        /// <returns></returns>
        public CookieContainer GetCookie()
        {
            return myCookie;
        }
        /// <summary>
        /// 获取所有cookies的字符串
        /// </summary>
        /// <param name="uri">网址</param>
        /// <param name="hadDomainAndPath">返回字符串中是否包含域信息</param>
        /// <returns>cookie字符串</returns>
        public string GetCookie(Uri uri, bool hadDomainAndPath)
        {
            string resStr = "";
            CookieCollection collection = new CookieCollection();
            if (uri != null)
            {
                collection = myCookie.GetCookies(uri);
            }
            else
            {
                collection = HtmlHelper.GetAllCookies(myCookie);
            }
            foreach (Cookie c in collection)
            {
                if (hadDomainAndPath)
                {
                    resStr += string.Format("{0}={1};Domain={2}; Path={3},", c.Name, c.Value, c.Domain, c.Path);
                }
                else
                {
                    resStr += string.Format("{0}={1};", c.Name, c.Value);
                }
            }
            return resStr;
        }

        /// <summary>
        /// 清空cookie
        /// </summary>
        public void ClearCookie()
        {
            myCookie = new CookieContainer();
        }
        /// <summary>
        /// 获取指定name的cookie值
        /// </summary>
        /// <param name="name">cookie名称</param>
        /// <returns>cookie值</returns>
        public string GetCookieValue(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                CookieCollection collection = HtmlHelper.GetAllCookies(myCookie);
                string cookieValue = string.Empty;
                foreach (Cookie c in collection)
                {
                    if (c.Name == name)
                    {
                        cookieValue = c.Value;
                        break;
                    }
                }
                return cookieValue;
            }
            else
            {
                return "";
            }
        }
        /// <summary>
        /// 删除cookie
        /// </summary>
        /// <param name="name">cookie名字</param>
        /// <param name="domain">cookie域名</param>
        /// <returns>删除的数目</returns>
        public int DeleteCookie(string name, string domain)
        {
            int i = 0;
            if (name == "" || domain == "")
            {
                return 0;
            }
            try
            {
                var listcookie = myCookie.ToListCookie();
                var tempcookie = myCookie.ToListCookie();
                if (name != null && domain != null)
                {
                    foreach (var cookie in listcookie)
                    {
                        if (cookie.Name == name && cookie.Domain == domain)
                        {
                            tempcookie.Remove(cookie);
                            i++;
                        }
                    }
                }
                else if (name != null && domain == null)
                {
                    foreach (var cookie in listcookie)
                    {
                        if (cookie.Name == name)
                        {
                            tempcookie.Remove(cookie);
                            i++;
                        }
                    }
                }
                else if (name == null && domain != null)
                {
                    foreach (var cookie in listcookie)
                    {
                        if (cookie.Domain == domain)
                        {
                            tempcookie.Remove(cookie);
                            i++;
                        }
                    }
                }
                else
                {
                    foreach (var cookie in listcookie)
                    {
                        tempcookie.Remove(cookie);
                        i++;
                    }
                }
                myCookie = tempcookie.ToCookieContainer();
            }
            catch (Exception)//异常返回-1
            {
                i = -1;
            }
            return i;
        }

        #endregion

        #region 访问控制处理
        /// <summary>
        /// 是否抛出异常
        /// </summary>
        public bool throwException { get; set; }
        /// <summary>
        /// 返回网页编码
        /// </summary>
        public Encoding ResponseEncoding { get; set; }
        /// <summary>
        /// 跳转的url，只有在AutoRedirect设置为false的时候才能获取
        /// </summary>
        public string LocationUrl { get; set; }
        /// <summary>
        /// 返回资源Uri地址
        /// </summary>
        public string ResponseUrl { get; set; }
        /// <summary>
        /// 超时时间
        /// </summary>
        public int TimeOut { get; set; }
        /// <summary>
        /// 是否自动跳转
        /// </summary>
        public bool AutoRedirect { get; set; }
        /// <summary>
        /// 获取或设置一个值，该值指示是否与 Internet 资源建立持久性连接默认为true。  
        /// </summary>
        public bool KeepAlive { get; set; }
        /// <summary>
        /// refererUrl
        /// </summary>
        public string ReUrl { get; set; }
        /// <summary>
        /// 提交类型(普通：application/x-www-form-urlencoded，文件：multipart/form-data)
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// 代理
        /// </summary>
        public WebProxy Proxy { get; set; }
        /// <summary>
        /// 快速设置代理IP
        /// </summary>
        /// <param name="strProxy"></param>
        public void SetProxy(string strProxy)
        {
            if (!string.IsNullOrEmpty(strProxy))
            {
                try
                {
                    WebProxy wp = new WebProxy(StrHelper.Abstract(strProxy, "", ":"), int.Parse(StrHelper.Abstract(strProxy, ":", "")));
                    NetworkCredential nCred = new NetworkCredential(null, null);
                    wp.Credentials = nCred;
                    Proxy = wp;
                }
                catch
                {
                    WebProxy wp = new WebProxy(strProxy);
                    NetworkCredential nCred = new NetworkCredential(null, null);
                    wp.Credentials = nCred;
                    Proxy = wp;
                }
            }
        }
        /// <summary>
        /// 用户代理（访问设备类型）
        /// </summary>
        public string UserAgent { get; set; }
        /// <summary>
        /// 网络访问设置
        /// </summary>
        public HttpWebSet WebSet { get; set; }
        /// <summary>
        /// 异步获取服务器响应结果（默认为false）
        /// </summary>
        public bool AsyncResponse { get; set; }
        /// <summary>
        /// 异步请求（如果此项设置为true,则只进行请求，不会Response）,此项生效的前提是必须是AsyncResponse属性为true
        /// </summary>
        public bool OnlyAsyncRequest { get; set; }
        /// <summary>
        /// 添加字符串cookies到http头(本设置为避免默认域处理cookies错误，默认为false，使用CookieContainer模式)
        /// </summary>
        public bool AddStrCookiesInHeader { get; set; }
        /// <summary>
        /// 是否添加X-Requested-With: XMLHttpRequest头
        /// </summary>
        public bool IsXMLHttpRequest { get; set; }
        /// <summary>
        /// Accept 头
        /// </summary>
        public string Accept { get; set; }
        /// <summary>
        /// 是否添加Accept-Encoding: gzip, deflate头
        /// </summary>
        public bool IsGzipEncoding { get; set; }
        /// <summary>
        /// 是否添加Accept-Language: zh-CN,zh;q=0.8
        /// </summary>
        public bool Iszh_CNLanguage { get; set; }
        /// <summary>
        /// 过滤空的cookie
        /// </summary>
        public bool FilterNullCookie { get; set; }
        /// <summary>
        /// post流时使用的编码（默认为UTF8）
        /// </summary>
        public Encoding PostStreamEncoding { get; set; }
        /// <summary>
        /// 自定义http头
        /// </summary>
        public Dictionary<string, string> HttpHeader { get; set; }
        /// <summary>
        /// 自定义替换键值对,用于批量替换cookie域
        /// </summary>
        public Dictionary<string, string> ReplaceDic { get; set; }
        #endregion

        #region 网页提交
        /// <summary>
        /// 网络异常重试次数
        /// </summary>
        private int _Retries { get; set; }
        /// <summary>
        /// 返回html
        /// </summary>
        private string tempHtml { get; set; }

        /// <summary>
        /// https访问处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        private bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        { //直接确认，否则打不开
            return true;
        }
        /// <summary>
        /// 创建网络访问对象
        /// </summary>
        /// <param name="url">URL地址</param>
        /// <param name="method">POST/GET</param>
        /// <returns></returns>
        private HttpWebRequest GetWebRequest(string url, string method)
        {
            this.tempHtml = string.Empty; //init empty
            HttpWebRequest req = null;
            if (url.Contains("https"))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                req = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url, true));
            }
            else
            {
                req = (HttpWebRequest)WebRequest.Create(new Uri(url, true));
            }
            if (IsFiddler) //Fiddler抓包
            {
                if (null != Proxy)
                {
                    req.Proxy = Proxy;
                }
            }
            else
            {
                req.Proxy = Proxy; //每次都设置代理，避免有些时候为空不设置但使用了IE代理
            }
            req.Method = method;
            if (method.ToLower() == "post")
            {
                req.ServicePoint.Expect100Continue = false;
                req.ContentType = ContentType;
            }
            req.KeepAlive = KeepAlive;
            req.AllowAutoRedirect = AutoRedirect;
            if (IsXMLHttpRequest)
                req.Headers.Add("X-Requested-With", "XMLHttpRequest");
            if (IsGzipEncoding)
                req.Headers.Add("Accept-Encoding", "gzip, deflate");
            if (Iszh_CNLanguage)
                req.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8");
            if (!AddStrCookiesInHeader)
                req.CookieContainer = myCookie;
            else
            {
                req.Headers.Add("Cookie", GetCookie(null, false));
            }
            if (string.IsNullOrEmpty(Accept))
            {
                req.Accept = "*/*";
            }
            else
            {
                req.Accept = Accept;
            }
            req.UserAgent = UserAgent;
            req.Timeout = TimeOut;
            req.ReadWriteTimeout = TimeOut;
            foreach (KeyValuePair<string, string> k in HttpHeader)
            {
                req.Headers.Add(k.Key, k.Value);
            }
            if (!string.IsNullOrEmpty(ReUrl))
                req.Referer = ReUrl;
            return req;
        }
        /// <summary>
        /// 解析网络访问结果
        /// </summary>
        /// <param name="rsp"></param>
        /// <returns></returns>
        private string GetResponseAsString(HttpWebRequest req)
        {
            using (HttpWebResponse rsp = (HttpWebResponse)req.GetResponse())
            {
                if (!FilterNullCookie)
                {
                    if (req.CookieContainer != null)
                    {
                        myCookie = req.CookieContainer;
                    }
                    if (rsp.Cookies != null)
                    {
                        myCookie.Add(rsp.Cookies);
                    }
                }
                SetCookies(req.RequestUri, rsp.Headers["Set-Cookie"]);
                LocationUrl = rsp.Headers.Get("Location");
                ResponseUrl = rsp.ResponseUri.ToString();
                return ResponseAnalysis(rsp);
            }
        }
        /// <summary>
        /// 返回数据分析（字符串）
        /// </summary>
        /// <param name="rsp"></param>
        /// <returns></returns>
        private string ResponseAnalysis(HttpWebResponse rsp)
        {
            try
            {
                if (rsp == null)
                    return "";
                Encoding encoding = Encoding.UTF8;
                if (ResponseEncoding == null)
                {
                    if (!string.IsNullOrEmpty(rsp.CharacterSet) && !rsp.CharacterSet.Contains("ISO-8859-1"))
                        encoding = Encoding.GetEncoding(rsp.CharacterSet);
                }
                else
                {
                    encoding = ResponseEncoding;
                }
                if (rsp.ContentEncoding.ToLower().Contains("gzip"))
                {
                    using (GZipStream stream = new GZipStream(rsp.GetResponseStream(), CompressionMode.Decompress))
                    {
                        using (StreamReader reader = new StreamReader(stream, encoding))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
                else if (rsp.ContentEncoding.ToLower().Contains("deflate"))
                {
                    using (DeflateStream stream = new DeflateStream(rsp.GetResponseStream(), CompressionMode.Decompress))
                    {
                        using (StreamReader reader = new StreamReader(stream, encoding))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
                else
                {
                    using (Stream stream = rsp.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream, encoding))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMsg = ex.Message + ex.Source + ex.StackTrace;
                return "";
            }
        }
        /// <summary>
        /// 返回数据分析（字节数组）
        /// </summary>
        /// <param name="rsp"></param>
        /// <returns></returns>
        private byte[] ResponseAnalysisAsBytes(HttpWebResponse rsp)
        {
            byte[] picBts = new byte[0];
            if (rsp.ContentEncoding.ToLower().Contains("gzip"))
            {
                using (GZipStream stream = new GZipStream(rsp.GetResponseStream(), CompressionMode.Decompress))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        int readSize = 0;
                        byte[] bytes = new byte[8 * 1024];
                        while ((readSize = stream.Read(bytes, 0, 1024)) > 0)
                        {
                            ms.Write(bytes, 0, readSize);
                        }
                        picBts = ms.ToArray();
                    }
                }
            }
            else if (rsp.ContentEncoding.ToLower().Contains("deflate"))
            {
                using (DeflateStream stream = new DeflateStream(rsp.GetResponseStream(), CompressionMode.Decompress))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        int readSize = 0;
                        byte[] bytes = new byte[8 * 1024];
                        while ((readSize = stream.Read(bytes, 0, 1024)) > 0)
                        {
                            ms.Write(bytes, 0, readSize);
                        }
                        picBts = ms.ToArray();
                    }
                }
            }
            else
            {
                using (System.IO.Stream responseStream = rsp.GetResponseStream())
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        int readSize = 0;
                        byte[] bytes = new byte[8 * 1024];
                        while ((readSize = responseStream.Read(bytes, 0, 1024)) > 0)
                        {
                            ms.Write(bytes, 0, readSize);
                        }
                        picBts = ms.ToArray();
                    }
                }
            }
            return picBts;
        }
        /// <summary>
        /// 更换代理
        /// </summary>
        /// <param name="webEx">异常对象</param>
        /// <returns>返回true表示更换成功或者未设置事件，返回fasle表示未更换或者更换失败</returns>
        private bool ChangeProxy(WebException webEx)
        {
            bool flag = false;
            if (webEx.Message.Contains("操作超时") || webEx.Message.Contains("请求已被取消"))
            {
                if (ChangeDisabledProxy != null)
                {
                    object objProxy = new object();
                    if (ChangeDisabledProxy(out objProxy))
                    {
                        SetProxy(objProxy.ToString());
                        flag = true;
                    }
                }
            }
            return flag;
        }

        #region 异步网络访问
        /// <summary>
        /// 手动通知事件
        /// </summary>
        private AutoResetEvent allDone = new AutoResetEvent(false);
        private Exception asyncException = new Exception();
        private WebException asyncWebException = new WebException();
        private void AsyncGetResponse(RequestState reqState)
        {
            this.allDone = new AutoResetEvent(false);
            this.ErrorMsg = "";
            this.asyncException = null;
            this.asyncWebException = null;
            if (reqState.request.Method.ToLower() == "post")
            {
                //开始异步请求
                IAsyncResult resultStream = (IAsyncResult)reqState.request.BeginGetRequestStream(new AsyncCallback(GetReqStreamCallback), reqState);
                //如果超时，回调函数中止请求
                RegisteredWaitHandle regStreamHandle = ThreadPool.RegisterWaitForSingleObject(resultStream.AsyncWaitHandle, new WaitOrTimerCallback(TimeoutCallback), reqState.request, TimeOut, true);
                //在允许的时间内执行完成。将在回调函数中发生
                this.allDone.WaitOne();
                regStreamHandle.Unregister(resultStream.AsyncWaitHandle);
                if (this.asyncException != null)
                    throw this.asyncException;
                else if (this.asyncWebException != null)
                    throw this.asyncWebException;
            }
            //开始异步请求
            IAsyncResult result = (IAsyncResult)reqState.request.BeginGetResponse(new AsyncCallback(RespCallback), reqState);
            if (OnlyAsyncRequest)
            {
                return;
            }
            //如果超时，回调函数中止请求
            ThreadPool.RegisterWaitForSingleObject(result.AsyncWaitHandle, new WaitOrTimerCallback(TimeoutCallback), reqState.request, TimeOut, true);

            //在允许的时间内执行完成。将在回调函数中发生
            this.allDone.WaitOne();

            //释放的HttpWebResponse资源。
            if (reqState.response != null)
                reqState.response.Close();
            if (this.asyncException != null)
                throw this.asyncException;
            else if (this.asyncWebException != null)
                throw this.asyncWebException;
        }
        /// <summary>
        /// 异步写入数据
        /// </summary>
        /// <param name="asynchronousResult"></param>
        private void GetReqStreamCallback(IAsyncResult asynchronousResult)
        {
            try
            {
                RequestState myRequestState = (RequestState)asynchronousResult.AsyncState;
                HttpWebRequest myHttpWebRequest = myRequestState.request;
                System.IO.Stream reqStream = myHttpWebRequest.EndGetRequestStream(asynchronousResult);
                reqStream.Write(myRequestState.postData, 0, myRequestState.postData.Length);
                reqStream.Close();
                return;

            }
            catch (WebException ex)
            {
                ErrorMsg = ex.Message;
                this.asyncException = ex;
            }
            catch (Exception ex)
            {
                ErrorMsg = ex.Message;
                this.asyncException = ex;
            }
            finally
            {
                this.allDone.Set();
            }
        }
        /// <summary>
        /// 异步获取返回对象
        /// </summary>
        /// <param name="asynchronousResult"></param>
        private void RespCallback(IAsyncResult asynchronousResult)
        {
            try
            {
                //存储异步请求状态
                RequestState myRequestState = (RequestState)asynchronousResult.AsyncState;
                HttpWebRequest myHttpWebRequest = myRequestState.request;
                myRequestState.response = (HttpWebResponse)myHttpWebRequest.EndGetResponse(asynchronousResult);
                if (!FilterNullCookie)
                {
                    if (myRequestState.request.CookieContainer != null)
                    {
                        myCookie = myRequestState.request.CookieContainer;
                    }
                    if (myRequestState.response.Cookies != null)
                    {
                        myCookie.Add(myRequestState.response.Cookies);
                    }
                }
                SetCookies(myRequestState.request.RequestUri, myRequestState.response.Headers["Set-Cookie"]);
                LocationUrl = myRequestState.response.Headers.Get("Location");
                ResponseUrl = myRequestState.response.ResponseUri.ToString();
                if (myRequestState.IsReturnBytes)
                {
                    myRequestState.resBytes = ResponseAnalysisAsBytes(myRequestState.response);
                }
                else
                {
                    myRequestState.resHtmlStr = ResponseAnalysis(myRequestState.response);
                }
                return;
            }
            catch (WebException ex)
            {
                ErrorMsg = ex.Message;
                this.asyncException = ex;
            }
            catch (Exception ex)
            {
                ErrorMsg = ex.Message;
                this.asyncException = ex;
            }
            finally
            {
                allDone.Set();
            }
        }
        /// <summary>
        /// 异步读取返回流数据
        /// </summary>
        /// <param name="asyncResult"></param>
        private void ReadCallBack(IAsyncResult asyncResult)
        {
            try
            {
                RequestState myRequestState = (RequestState)asyncResult.AsyncState;
                int read = myRequestState.streamResponse.EndRead(asyncResult);
                Encoding encoding = Encoding.UTF8;
                if (ResponseEncoding == null)
                {
                    if (!string.IsNullOrEmpty(myRequestState.response.CharacterSet) && !myRequestState.response.CharacterSet.Contains("ISO-8859-1"))
                        encoding = Encoding.GetEncoding(myRequestState.response.CharacterSet);
                }
                else
                {
                    encoding = ResponseEncoding;
                }
                // 读取
                if (read > 0)
                {
                    myRequestState.resHtmlStr += encoding.GetString(myRequestState.BufferRead, 0, read);
                    IAsyncResult asynchronousResult = myRequestState.streamResponse.BeginRead(myRequestState.BufferRead, 0, RequestState.BUFFER_SIZE, new AsyncCallback(ReadCallBack), myRequestState);
                    return;
                }

            }
            catch (WebException ex)
            {
                ErrorMsg = ex.Message;
            }
            //取消线程等待
            allDone.Set();
        }
        /// <summary>
        /// 超时关闭
        /// </summary>
        /// <param name="state"></param>
        /// <param name="timedOut"></param>
        private void TimeoutCallback(object state, bool timedOut)
        {
            if (timedOut)
            {
                HttpWebRequest request = state as HttpWebRequest;
                if (request != null)
                {
                    request.Abort();
                }
            }
        }
        #endregion

        /// <summary>
        /// get方式获取网页HTML
        /// </summary>
        /// <param name="getUrl">url</param>
        /// <returns>返回字符串</returns>
        public string DoGet(string getUrl)
        {
            int Retries = 0;
            do
            {
                Retries++;
                try
                {
                    HttpWebRequest req = GetWebRequest(getUrl, "GET");
                    if (!this.AsyncResponse)
                        this.tempHtml = GetResponseAsString(req);
                    else
                    {
                        RequestState reqState = new RequestState();
                        reqState.request = req;
                        reqState.IsReturnBytes = false;
                        AsyncGetResponse(reqState);
                        this.tempHtml = reqState.resHtmlStr;
                    }
                    break;
                }
                catch (WebException webEx)
                {
                    if (throwException)
                        throw webEx;
                    if (ChangeProxy(webEx))
                    {
                        Retries--;
                    }
                    this.tempHtml = ResponseAnalysis((HttpWebResponse)webEx.Response); //get exception response
                    //Trace.WriteLine(webEx.Message + getUrl, DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"));
                    Thread.Sleep(200);
                }
                finally
                {
                    this.WebSet.WebWait(); //网络访问等待
                    while (AppConfig.appRunState == AppRunState.Pause)  //暂停程序
                    {
                        this.WebSet.PauseWait();
                    }
                }
            } while (Retries <= this.WebSet.WebExceptionRetries);
            return this.tempHtml;
        }
        /// <summary>
        /// post提交网页获取HTML
        /// </summary>
        /// <param name="postUrl">提交地址</param>
        /// <param name="postData">提交数据</param>
        /// <returns>返回字符串</returns>
        public string DoPost(string postUrl, IDictionary<string, string> postData)
        {
            return DoPost(postUrl, HtmlHelper.BuildQuery(postData, Encoding.UTF8, false, false));
        }
        /// <summary>
        /// post提交网页获取HTML
        /// </summary>
        /// <param name="postUrl">提交地址</param>
        /// <param name="postData">提交数据</param>
        /// <returns>返回字符串</returns>
        public string DoPost(string postUrl, string postData)
        {
            byte[] postStr = PostStreamEncoding.GetBytes(postData);
            return DoPost(postUrl, postStr);
        }
        /// <summary>
        /// post提交网页获取HTML
        /// </summary>
        /// <param name="postUrl">提交地址</param>
        /// <param name="postData">提交数据</param>
        /// <param name="postEncode">提交数据编码方式</param>
        /// <returns>返回字符串</returns>
        public string DoPost(string postUrl, IDictionary<string, string> postData, Encoding postEncode)
        {
            return DoPost(postUrl, HtmlHelper.BuildQuery(postData, postEncode, false, false));
        }
        /// <summary>
        /// post提交网页获取HTML
        /// </summary>
        /// <param name="postUrl">提交地址</param>
        /// <param name="postData">提交数据</param>
        /// <returns>返回字符串</returns>
        public string DoPost(string postUrl, byte[] postData)
        {
            int Retries = 0;
            do
            {
                Retries++;
                try
                {
                    HttpWebRequest req = GetWebRequest(postUrl, "POST");

                    if (!this.AsyncResponse)
                    {
                        System.IO.Stream reqStream = req.GetRequestStream();
                        reqStream.Write(postData, 0, postData.Length);
                        reqStream.Close();
                        this.tempHtml = GetResponseAsString(req);
                    }
                    else
                    {
                        RequestState reqState = new RequestState();
                        reqState.request = req;
                        reqState.IsReturnBytes = false;
                        reqState.postData = postData;
                        AsyncGetResponse(reqState);
                        this.tempHtml = reqState.resHtmlStr;
                    }
                    break;
                }
                catch (WebException webEx)
                {
                    if (throwException)
                        throw webEx;
                    if (ChangeProxy(webEx))
                    {
                        Retries--;
                    }
                    this.tempHtml = ResponseAnalysis((HttpWebResponse)webEx.Response); //get exception response
                    //Trace.WriteLine(webEx.Message + postUrl, DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"));
                    Thread.Sleep(200);
                }
                finally
                {
                    this.WebSet.WebWait(); //网络访问等待
                    while (AppConfig.appRunState == AppRunState.Pause)  //暂停程序
                    {
                        this.WebSet.PauseWait();
                    }
                }
            } while (Retries <= this.WebSet.WebExceptionRetries);
            return this.tempHtml;
        }
        /// <summary>
        /// 下载图片到文件
        /// </summary>
        /// <param name="picUrl">图片URL</param>
        /// <param name="saveFilePath">保存文件地址</param>
        /// <returns>true/false</returns>
        public bool DownPicture(string picUrl, string saveFilePath)
        {
            bool downFlag = false;
            int Retries = 0;
            do
            {
                Retries++;
                try
                {
                    if (System.IO.File.Exists(saveFilePath)) { System.IO.File.Delete(saveFilePath); }
                    HttpWebRequest req = GetWebRequest(picUrl, "GET");
                    if (!this.AsyncResponse)
                    {
                        using (HttpWebResponse rsp = (HttpWebResponse)req.GetResponse())
                        {

                            using (System.IO.Stream responseStream = rsp.GetResponseStream())
                            {
                                int readSize = 0;
                                byte[] bytes = new byte[1024];
                                FileStream fs = new FileStream(saveFilePath, FileMode.Create, FileAccess.Write);
                                while ((readSize = responseStream.Read(bytes, 0, 1024)) > 0)
                                {
                                    fs.Write(bytes, 0, readSize);
                                    fs.Flush();
                                }
                                fs.Close();
                            }
                            SetCookies(req.RequestUri, rsp.Headers["Set-Cookie"]);
                        }
                        downFlag = File.Exists(saveFilePath);
                    }
                    else
                    {
                        RequestState reqState = new RequestState();
                        reqState.request = req;
                        reqState.IsReturnBytes = true;
                        AsyncGetResponse(reqState);
                        File.WriteAllBytes(saveFilePath, reqState.resBytes);
                    }
                    break;
                }
                catch (WebException webEx)
                {
                    if (throwException)
                        throw webEx;
                    if (ChangeProxy(webEx))
                    {
                        Retries--;
                    }
                    //Trace.WriteLine(webEx.Message + postUrl, DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"));
                    Thread.Sleep(200);
                }
                finally
                {
                    this.WebSet.WebWait(); //网络访问等待
                    while (AppConfig.appRunState == AppRunState.Pause)  //暂停程序
                    {
                        this.WebSet.PauseWait();
                    }
                }
            } while (Retries <= this.WebSet.WebExceptionRetries);
            return downFlag;
        }
        /// <summary>
        /// 下载图片到byte[]
        /// </summary>
        /// <param name="picUrl">图片URL</param>
        /// <returns>返回图片byte</returns>
        public byte[] DownPicture(string picUrl)
        {
            byte[] picBts = new byte[0];
            int Retries = 0;
            do
            {
                Retries++;
                try
                {
                    HttpWebRequest req = GetWebRequest(picUrl, "GET");
                    if (!this.AsyncResponse)
                    {

                        using (HttpWebResponse rsp = (HttpWebResponse)req.GetResponse())
                        {
                            SetCookies(req.RequestUri, rsp.Headers["Set-Cookie"]);

                            if (rsp.ContentEncoding.ToLower().Contains("gzip"))
                            {
                                using (GZipStream stream = new GZipStream(rsp.GetResponseStream(), CompressionMode.Decompress))
                                {
                                    using (MemoryStream ms = new MemoryStream())
                                    {
                                        int readSize = 0;
                                        byte[] bytes = new byte[8 * 1024];
                                        while ((readSize = stream.Read(bytes, 0, 1024)) > 0)
                                        {
                                            ms.Write(bytes, 0, readSize);
                                        }
                                        picBts = ms.ToArray();
                                    }
                                }
                            }
                            else if (rsp.ContentEncoding.ToLower().Contains("deflate"))
                            {
                                using (DeflateStream stream = new DeflateStream(rsp.GetResponseStream(), CompressionMode.Decompress))
                                {
                                    using (MemoryStream ms = new MemoryStream())
                                    {
                                        int readSize = 0;
                                        byte[] bytes = new byte[8 * 1024];
                                        while ((readSize = stream.Read(bytes, 0, 1024)) > 0)
                                        {
                                            ms.Write(bytes, 0, readSize);
                                        }
                                        picBts = ms.ToArray();
                                    }
                                }
                            }
                            else
                            {
                                using (System.IO.Stream responseStream = rsp.GetResponseStream())
                                {
                                    using (MemoryStream ms = new MemoryStream())
                                    {
                                        int readSize = 0;
                                        byte[] bytes = new byte[8 * 1024];
                                        while ((readSize = responseStream.Read(bytes, 0, 1024)) > 0)
                                        {
                                            ms.Write(bytes, 0, readSize);
                                        }
                                        picBts = ms.ToArray();
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        RequestState reqState = new RequestState();
                        reqState.request = req;
                        reqState.IsReturnBytes = true;
                        AsyncGetResponse(reqState);
                        picBts = reqState.resBytes;
                    }
                    break;
                }
                catch (WebException webEx)
                {
                    if (throwException)
                        throw webEx;
                    if (ChangeProxy(webEx))
                    {
                        Retries--;
                    }
                    Thread.Sleep(200);
                }
                finally
                {
                    this.WebSet.WebWait(); //网络访问等待
                    while (AppConfig.appRunState == AppRunState.Pause)  //暂停程序
                    {
                        this.WebSet.PauseWait();
                    }
                }
            } while (Retries <= this.WebSet.WebExceptionRetries);
            return picBts;
        }
        /// <summary>
        /// 上传formdata
        /// </summary>
        /// <param name="postUrl"></param>
        /// <param name="listFormData"></param>
        /// <returns></returns>
        public string DoPostFormData(string postUrl, List<FormData> listFormData)
        {
            List<byte> postData = new List<byte>();

            var boundary = Encoding.UTF8.GetBytes("\r\n--Feu_skMK0c4qMFPUs_GpG6MczNpcMyRMyV20A3rS\r\n");
            
            foreach (var item in listFormData)
            {
                postData.AddRange(boundary);
                postData.AddRange(item.GetByte());
            }

            postData.AddRange(Encoding.UTF8.GetBytes("\r\n--Feu_skMK0c4qMFPUs_GpG6MczNpcMyRMyV20A3rS--"));

            var tempcontenttype = this.ContentType;
            this.ContentType = "multipart/form-data; boundary=Feu_skMK0c4qMFPUs_GpG6MczNpcMyRMyV20A3rS";

            var tempHtml = this.DoPost(postUrl, postData.ToArray());

            this.ContentType = tempcontenttype;

            return tempHtml;
        }

        #endregion
    }

    /// <summary>
    /// formdata类型
    /// </summary>
    public class FormData
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 内容类型
        /// </summary>
        public string Content_Type { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public List<byte> Value { get; set; }
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 普通类型
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public FormData(string name, string value)
        {
            Name = name;
            Value = Encoding.UTF8.GetBytes(value).ToList();
        }
        /// <summary>
        /// 文件锁
        /// </summary>
        private static object LockObj = new object();
        /// <summary>
        /// 图片类型
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
        public FormData(string name, FilePath path)
        {
            Name = name;
            Content_Type = "application/octet-stream";
            FileName = path.filepath.Split('\\').Last();
            Value = new List<byte>();
            try
            {
                lock (LockObj)
                {
                    using (FileStream fs = new FileStream(path.filepath, FileMode.Open, FileAccess.Read))
                    {
                        byte[] buffer = new byte[fs.Length];
                        int count = 0;
                        do
                        {
                            count = fs.Read(buffer, 0, buffer.Length);
                            if (count > 0)
                            {
                                Value.AddRange(buffer);
                            }
                        } while (count > 0);
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        /// <summary>
        /// 获取比特流
        /// </summary>
        /// <returns></returns>
        public byte[] GetByte()
        {
            var data = string.Format("Content-Disposition: form-data; name=\"{0}\"; ", Name);
            if (!string.IsNullOrEmpty(FileName))
            {
                data += string.Format("filename=\"{0}\"",FileName);
            }
            if (!string.IsNullOrEmpty(Content_Type))
            {
                data += string.Format("\r\nContent-Type: {0}", Content_Type);
            }
            data += "\r\n\r\n";

            var bits = Encoding.UTF8.GetBytes(data).ToList();
            bits.AddRange(Value);
            return bits.ToArray();
        }
    }
    /// <summary>
    /// 文件路径
    /// </summary>
    public class FilePath
    {
        /// <summary>
        /// 文件路径
        /// </summary>
        public string filepath { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        public FilePath(string path)
        {
            filepath = path;
        }
    }
}
