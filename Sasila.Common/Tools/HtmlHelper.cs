using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Net;
using Sasila.Common.Http;
using System.Collections;
using System.Runtime.InteropServices;

namespace Sasila.Common.Tools
{
    /// <summary>
    /// html常用操作
    /// </summary>
    public class HtmlHelper
    {
        //设置系统时间的API函数
        [DllImport("kernel32.dll")]
        private static extern bool SetLocalTime(ref SYSTEMTIME time);

        /// <summary>
        /// 向IE设置Cookies
        /// </summary>
        /// <param name="lpszUrlName">url</param>
        /// <param name="lbszCookieName">cookie名称</param>
        /// <param name="lpszCookieData">cookie值</param>
        /// <returns></returns>
        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool InternetSetCookie(string lpszUrlName, string lbszCookieName, string lpszCookieData);

        /// <summary>
        /// 读取IE的Cookies
        /// </summary>
        /// <param name="lpszUrlName">url</param>
        /// <param name="lbszCookieName">cookie名称</param>
        /// <param name="lpszCookieData">cookie值</param>
        /// <param name="lpdwSize"></param>
        /// <returns></returns>
        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool InternetGetCookie(string lpszUrlName, string lbszCookieName, StringBuilder lpszCookieData, ref int lpdwSize);

        [DllImport("kernel32.dll")]
        public static extern Int32 GetLastError();

        [StructLayout(LayoutKind.Sequential)]
        private struct SYSTEMTIME
        {
            public short year;
            public short month;
            public short dayOfWeek;
            public short day;
            public short hour;
            public short minute;
            public short second;
            public short milliseconds;
        }

        /// <summary>
        /// 网页中的UNICODE编码转换为汉字，主要用于JS返回的结果码解析
        /// </summary>
        /// <param name="unicodeStr"></param>
        /// <returns></returns>
        public static string UnicodeToGB(string unicodeStr)
        {
            try
            {
                MatchCollection mc = Regex.Matches(unicodeStr, "(\\\\u([\\w]{4}))");
                if (mc != null && mc.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (Match m2 in mc)
                    {
                        string v = m2.Value;
                        string word = v.Substring(2);
                        byte[] codes = new byte[2];
                        int code = Convert.ToInt32(word.Substring(0, 2), 16);
                        int code2 = Convert.ToInt32(word.Substring(2), 16);
                        codes[0] = (byte)code2;
                        codes[1] = (byte)code;
                        unicodeStr = unicodeStr.Replace(v, Encoding.Unicode.GetString(codes));
                        //sb.Append(Encoding.Unicode.GetString(codes));
                    }
                    return unicodeStr;
                }
                else
                {
                    return unicodeStr;
                }
            }
            catch
            {
                return "";
            }
        }
        /// <summary>
        /// 格林威治时差（毫秒）转换成标准时间
        /// </summary>
        /// <param name="gelinMilliseconds"></param>
        /// <returns></returns>
        public static string GelinMillisecondsToDate(string gelinMilliseconds)
        {
            gelinMilliseconds += "0000";
            DateTime date = DateTime.Parse("1970-01-01 08:00:00").AddTicks(long.Parse(gelinMilliseconds));
            return date.ToString("yyyy-MM-dd HH:mm:ss");
            //DateTime dateTimeStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            //long lTime = long.Parse(gelinMilliseconds + "0000000");
            //TimeSpan toNow = new TimeSpan(lTime);
            //return dateTimeStart.Add(toNow).ToString("yyyy-MM-dd HH:mm:ss");//by mjw
        }
        /// <summary>
        /// 格林威治时差（毫秒）转换成标准时间
        /// </summary>
        /// <param name="gelinMilliseconds"></param>
        /// <returns></returns>
        public static long DateToGelinMilliseconds(DateTime time)
        {
            DateTime gelin = DateTime.Parse("1970-01-01 08:00:00");
            TimeSpan span = time.Subtract(gelin);
            return (long)span.TotalMilliseconds;
        }
        /// <summary>
        /// 获取当前时间与格林位置时间的时差（毫秒）
        /// </summary>
        /// <returns></returns>
        public static string GetGelinMilliseconds()
        {
            return DateToGelinMilliseconds(DateTime.Now).ToString();
        }
        /// <summary>
        /// 组装post字符串(包含空字符串)
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string BuildQuery(IDictionary<string, string> parameters, Encoding encode)
        {
            return BuildQuery(parameters, encode, false, false);
        }
        /// <summary>
        /// 组装post字符串
        /// </summary>
        /// <param name="parameters">数据字典</param>
        /// <param name="encode">编码</param>
        /// <param name="filterEmptyName">是否过滤Name为空</param>
        /// <param name="filterEmptyValue">是否过滤Value为空</param>
        /// <returns>组装好的字符串</returns>
        public static string BuildQuery(IDictionary<string, string> parameters, Encoding encode, bool filterEmptyName, bool filterEmptyValue)
        {
            StringBuilder postData = new StringBuilder();

            foreach (KeyValuePair<string, string> k in parameters)
            {
                if (filterEmptyName && string.IsNullOrEmpty(k.Key))
                    continue;
                if (filterEmptyValue && string.IsNullOrEmpty(k.Value))
                    continue;
                if (postData.Length > 0)
                {
                    postData.Append(string.Format("&{0}={1}", HttpUtility.UrlEncode(k.Key, encode), HttpUtility.UrlEncode(k.Value, encode)));
                }
                else
                {
                    postData.Append(string.Format("{0}={1}", HttpUtility.UrlEncode(k.Key, encode), HttpUtility.UrlEncode(k.Value, encode)));
                }
            }

            return postData.ToString();
        }
        /// <summary>
        /// 获取百度标准时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetBaiduDate()
        {
            DateTime dt = DateTime.Parse("1970-01-01 08:00:00");
            try
            {
                HttpWebPage openWeb = new HttpWebPage();
                openWeb.ReUrl = "http://bjtime.cn/";
                string tempHtml = openWeb.DoGet("http://bjtime.cn/header11.asp");
                dt = DateTime.Parse(GelinMillisecondsToDate(tempHtml));
            }
            catch
            {
            }
            return dt;
        }

        /// <summary>
        /// 设置Cookie到IE（过期时间自动设置为一天）
        /// </summary>
        /// <param name="uri">uri</param>
        /// <param name="name">cookie名称</param>
        /// <param name="value">cookie值</param>
        /// <returns></returns>
        public static bool SetCookieToIE(Uri uri, string name, string value)
        {
            string expires = DateTime.Now.AddDays(1).GetDateTimeFormats('r')[0].ToString();
            string domain = uri.Host.Replace("www", "");
            if (!InternetSetCookie(uri.AbsoluteUri, name, value + string.Format(";Expires={0};Domain={1};", expires, domain)))
            {
                GetLastError().ToString();
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 获取IE的Cookie
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetIECookie(Uri uri, string name)
        {
            int size = 1000;
            StringBuilder sbCookie = new StringBuilder(size);
            if (!InternetGetCookie(uri.AbsoluteUri, name, sbCookie, ref size))
            {
                GetLastError().ToString();
            }
            return sbCookie.ToString();
        }

        /// <summary>
        /// 本地时间同步百度时间
        /// </summary>
        /// <returns></returns>
        public static bool SynchronizationBaiduDate()
        {
            bool synFlag = false;
            DateTime dt = GetBaiduDate();
            if (dt.Year == 1970)
            {
                return synFlag;
            }
            SYSTEMTIME st;
            st.year = (short)dt.Year;
            st.month = (short)dt.Month;
            st.dayOfWeek = (short)dt.DayOfWeek;
            st.day = (short)dt.Day;
            st.hour = (short)dt.Hour;
            st.minute = (short)dt.Minute;
            st.second = (short)dt.Second;
            st.milliseconds = (short)dt.Millisecond;
            synFlag = SetLocalTime(ref st);
            return synFlag;
        }
        /// <summary>
        /// 获取外网IP地址
        /// </summary>
        /// <returns></returns>
        public static string GetIpAddress()
        {
            try
            {
                string tempHtml = "";
                HttpWebPage openWeb = new HttpWebPage();
                int times = 5;
                do
                {
                    times--;
                    try
                    {
                        openWeb = new HttpWebPage();
                        openWeb.ReUrl = "https://www.baidu.com";
                        openWeb.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; rv:11.0) like Gecko";
                        openWeb.AutoRedirect = false;
                        tempHtml = openWeb.DoGet("https://www.baidu.com/s?ie=UTF-8&wd=ip%E5%9C%B0%E5%9D%80");
                        tempHtml = StrHelper.Abstract(tempHtml, ">本机IP:", "<");
                        tempHtml = tempHtml.Replace("&nbsp;", "");
                        if (string.IsNullOrEmpty(tempHtml))
                        {
                            openWeb = new HttpWebPage();
                            openWeb.ResponseEncoding = Encoding.GetEncoding("gb2312");
                            tempHtml = openWeb.DoGet("http://1111.ip138.com/ic.asp");
                            tempHtml = StrHelper.Abstract(tempHtml, "您的IP是：[", "]");
                        }
                        if (!string.IsNullOrEmpty(tempHtml)) break;
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                } while (times > 0);
                return tempHtml;
            }
            catch
            {
                return "";
            }
        }
        /// <summary>
        /// 将CookieContainer转换为cookie列表
        /// </summary>
        /// <param name="cc"></param>
        /// <returns></returns>
        public static CookieCollection GetAllCookies(CookieContainer cc)
        {
            CookieCollection lstCookies = new CookieCollection();

            Hashtable table = (Hashtable)cc.GetType().InvokeMember("m_domainTable",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField |
                System.Reflection.BindingFlags.Instance, null, cc, new object[] { });

            foreach (object pathList in table.Values)
            {
                SortedList lstCookieCol = (SortedList)pathList.GetType().InvokeMember("m_list",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField
                    | System.Reflection.BindingFlags.Instance, null, pathList, new object[] { });
                foreach (CookieCollection colCookies in lstCookieCol.Values)
                    foreach (Cookie c in colCookies) lstCookies.Add(c);
            }
            return lstCookies;
        }
    }
}
