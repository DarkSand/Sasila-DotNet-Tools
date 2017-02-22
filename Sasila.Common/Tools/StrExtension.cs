using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Web.Extensions;
using System.Collections;

namespace Sasila.Common.Tools
{
    /// <summary>
    /// String扩展类
    /// </summary>
    public static class StrExtension
    {
        /// <summary>
        /// URL编码
        /// </summary>
        /// <param name="value"></param>
        /// <param name="e">编码类型</param>
        /// <returns></returns>
        public static string UrlEncode(this string value, Encoding e)
        {
            return System.Web.HttpUtility.UrlEncode(value, e);
        }
        /// <summary>
        /// URL解码
        /// </summary>
        /// <param name="value"></param>
        /// <param name="e">编码类型</param>
        /// <returns></returns>
        public static string UrlDecode(this string value, Encoding e)
        {
            return System.Web.HttpUtility.UrlDecode(value, e);
        }
        /// <summary>
        /// URL编码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string UrlEncode(this string value)
        {
            return System.Web.HttpUtility.UrlEncode(value);
        }
        /// <summary>
        /// URL解码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string UrlDecode(this string value)
        {
            return System.Web.HttpUtility.UrlDecode(value);
        }
        /// <summary>
        /// HTML编码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string HtmlEncode(this string value)
        {
            return System.Web.HttpUtility.HtmlEncode(value);
        }
        /// <summary>
        /// HTML解码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string HtmlDecode(this string value)
        {
            return System.Web.HttpUtility.HtmlDecode(value);
        }
        /// <summary>
        /// ASCII码转为十六进制
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToHex(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }
            byte[] bytes = Encoding.ASCII.GetBytes(value);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString("X2"));
            }
            return sb.ToString();
        }
        /// <summary>
        /// 十六进制转为Byte[]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] HexToByte(this string value)
        {
            byte[] returnBytes = new byte[0];
            if (string.IsNullOrEmpty(value))
            {
                return returnBytes;
            }
            value = value.Replace(" ", "");
            if ((value.Length % 2) != 0)
            {
                value += " ";
            }
            returnBytes = new byte[value.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(value.Substring(i * 2, 2), 16);
            }
            return returnBytes;
        }
        /// <summary>
        /// 16进制转ASCII码
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static string HexToAscii(this string hexString)
        {
            if (string.IsNullOrEmpty(hexString))
            {
                return hexString;
            }
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i <= hexString.Length - 2; i += 2)
            {
                sb.Append(
                      Convert.ToString(
                        Convert.ToChar(int.Parse(hexString.Substring(i, 2), System.Globalization.NumberStyles.HexNumber))));
            }
            return sb.ToString();
        }
        /// <summary>
        /// 提取字符串
        /// </summary>
        /// <param name="subStr">源字符串</param>
        /// <param name="startStr">提取的开始字符</param>
        /// <param name="endStr">提取的结束字符</param>
        /// <returns></returns>
        public static string Abstract(this string subStr, string startStr, string endStr)
        {
            if (string.IsNullOrEmpty(subStr))
                return "";
            string res = string.Empty;
            if (!string.IsNullOrEmpty(startStr))
            {
                if (!subStr.Contains(startStr))
                {
                    return res;
                }
                else
                {
                    subStr = subStr.Substring(subStr.IndexOf(startStr) + startStr.Length);
                }
            }
            if (!string.IsNullOrEmpty(endStr))
            {
                if (!subStr.Contains(endStr))
                {
                    return res;
                }
                else
                {
                    res = subStr.Substring(0, subStr.IndexOf(endStr));
                }
            }
            else
            {
                res = subStr;
            }
            return res;
        }
        /// <summary>
        /// Unicode编码转换为中文，主要用于JS返回的结果码解析
        /// </summary>
        /// <param name="unicodeStr">Unicode字符串</param>
        public static string UnicodeToGB(this string unicodeStr)
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
        /// 中文转换为Unicode编码，主要用于JS返回的结果码解析
        /// </summary>
        /// <param name="gbStr"></param>
        /// <returns></returns>
        public static string GBToUnicode(this string gbStr)
        {
            string outStr = "";
            if (string.IsNullOrEmpty(gbStr))
            {
                return outStr;
            }
            for (int i = 0; i < gbStr.Length; i++)
            {
                if ((int)gbStr[i] > 127)
                    outStr += "\\u" + ((int)gbStr[i]).ToString("x");
                else
                    outStr += gbStr[i];
            }
            return outStr;
        }
        /// <summary>
        /// String转化为CookieContainer,格式必须为 cookiename=cookievalue;Domain=domain;Path=path, 否则返回null
        /// </summary>
        /// <param name="cookieStr">cookiename=cookievalue;Domain=domain;Path=path,</param>
        /// <returns></returns>
        public static CookieContainer ToCookieContainer(this string cookieStr)
        {
            CookieContainer cc = new CookieContainer();
            cc.Capacity = 1024;
            cc.PerDomainCapacity = 1024;
            try
            {
                var list = cookieStr.Split(',').ToList();
                foreach (string cstr in list)
                {
                    if (!string.IsNullOrEmpty(cstr.Trim()))
                    {
                        var cArray = cstr.Split(';');
                        cc.Add(new Cookie()
                        {
                            Name = cArray[0].Abstract("", "=").Trim(),
                            Value = cArray[0].Abstract("=", "").Trim(),
                            Domain = cArray[1].Abstract("Domain=", "").Trim(),
                            Path = cArray[2].Abstract("Path=", "").Trim()
                        });
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
            return cc;
        }
        /// <summary>
        /// json格式string序列化为dictionay
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Dictionary<string, object> toDictionary(this string value)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer javaSer = new System.Web.Script.Serialization.JavaScriptSerializer();
                return javaSer.Deserialize<Dictionary<string, object>>(value);
            }
            catch (Exception)
            {
                return new Dictionary<string, object>();
            }
        }
        /// <summary>
        /// 对象转化为listDictionary
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static List<Dictionary<string, object>> toListDictionary(this object value)
        {
            try
            {
                return (value as ArrayList).Cast<Dictionary<string, object>>().ToList();
            }
            catch (Exception)
            {
                return new List<Dictionary<string, object>>();
            }
        }
        /// <summary>
        /// string转化为listDictionary
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static List<Dictionary<string, object>> AsListDictionary(this string value)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer javaSer = new System.Web.Script.Serialization.JavaScriptSerializer();
                return javaSer.Deserialize<ArrayList>(value).Cast<Dictionary<string,object>>().ToList();
            }
            catch (Exception)
            {
                return new List<Dictionary<string, object>>();
            }
        }
        /// <summary>
        /// 当作 Dictionary处理
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Dictionary<string, object> AsDictionay(this object value)
        {
            try
            {
                return value as Dictionary<string, object>;
            }
            catch (Exception)
            {
                return new Dictionary<string, object>();
            }
        }
        /// <summary>
        /// List分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">List</param>
        /// <param name="PageSize">每页包含元素数量</param>
        /// <returns></returns>
        public static List<List<T>> SplitPage<T>(this List<T> value, int PageSize)
        {
            List<List<T>> list = new List<List<T>>();

            try
            {
                int size = (int)Math.Ceiling((decimal)value.Count / PageSize);
                for (int i = 1; i <= size; i++)
                {
                    list.Add(new PagingUtil<T>(value, PageSize, i).ToList());
                }
            }
            catch (Exception)
            {
            }

            return list;
        }
        /// <summary>
        /// 查找html节点(不会返回null)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parttern"></param>
        /// <returns></returns>
        public static List<HtmlNode> QuerySelectorAll(this string value, string parttern)
        {
            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(value);
                var page = doc.DocumentNode;
                var list = page.QuerySelectorAll(parttern).ToList();
                return list;
            }
            catch (Exception)
            {
                return new List<HtmlNode>();
            }
        }
        /// <summary>
        /// 查找html节点(可能返回null)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parttern"></param>
        /// <returns></returns>
        public static HtmlNode QuerySelector(this string value, string parttern)
        {
            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(value);
                var page = doc.DocumentNode;
                var node = page.QuerySelector(parttern);
                return node;
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// 获取查找节点的InnerText(避免出现空指针异常)(不会返回null)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parttern"></param>
        /// <returns></returns>
        public static string QuerySelectorInnerText(this string value, string parttern)
        {
            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(value);
                var page = doc.DocumentNode;
                var node = page.QuerySelector(parttern);
                return node.InnerText;
            }
            catch (Exception)
            {
                return "";
            }
        }
        /// <summary>
        /// 获取查找节点的属性(避免出现空指针异常)(不会返回null)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parttern"></param>
        /// <param name="attributename">属性名</param>
        /// <returns></returns>
        public static string QuerySelectorAttribute(this string value, string parttern ,string attributename)
        {
            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(value);
                var page = doc.DocumentNode;
                var node = page.QuerySelector(parttern);
                return node.Attributes[attributename].Value;
            }
            catch (Exception)
            {
                return "";
            }
        }
        /// <summary>
        /// 获取查找节点的InnerText(避免出现空指针异常)(不会返回null)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parttern"></param>
        /// <returns></returns>
        public static List<string> QuerySelectorInnerTextAll(this string value, string parttern)
        {
            List<string> list = new List<string>();
            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(value);
                var page = doc.DocumentNode;
                var nodelist = page.QuerySelectorAll(parttern);
                foreach (var item in nodelist)
                {
                    list.Add(item.InnerText);
                }
            }
            catch (Exception)
            {
            }
            return list;
        }
        /// <summary>
        /// 获取查找节点的属性(避免出现空指针异常)(不会返回null)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parttern"></param>
        /// <param name="attributename">属性名</param>
        /// <returns></returns>
        public static List<string> QuerySelectorAttributeAll(this string value, string parttern, string attributename)
        {
            List<string> list = new List<string>();
            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(value);
                var page = doc.DocumentNode;
                var nodelist = page.QuerySelectorAll(parttern);
                foreach (var item in nodelist)
                {
                    list.Add(item.Attributes[attributename].Value);
                }
            }
            catch (Exception)
            {
            }
            return list;
        }
        /// <summary>
        /// 查找html节点(不会返回null)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parttern"></param>
        /// <returns></returns>
        public static List<string> QuerySelectorAllStr(this string value, string parttern)
        {
            try
            {
                List<string> listStr = new List<string>();
                var doc = new HtmlDocument();
                doc.LoadHtml(value);
                var page = doc.DocumentNode;
                var list = page.QuerySelectorAll(parttern).ToList();
                foreach (var node in list)
                {
                    listStr.Add(node.OuterHtml);
                }
                return listStr;
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }
        /// <summary>
        /// 查找html节点(不会返回null)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parttern"></param>
        /// <returns></returns>
        public static string QuerySelectorStr(this string value, string parttern)
        {
            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(value);
                var page = doc.DocumentNode;
                var node = page.QuerySelector(parttern);
                return node.OuterHtml;
            }
            catch (Exception)
            {
                return "";
            }
        }
        /// <summary>
        /// 获取查找节点的InnerText(避免出现空指针异常)(不会返回null)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parttern"></param>
        /// <returns></returns>
        public static string QuerySelectorInnerText(this HtmlNode value, string parttern)
        {
            try
            {
                var node = value.QuerySelector(parttern);
                return node.InnerText;
            }
            catch (Exception)
            {
                return "";
            }
        }
        /// <summary>
        /// 获取查找节点的属性(避免出现空指针异常)(不会返回null)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parttern"></param>
        /// <param name="attributename">属性名</param>
        /// <returns></returns>
        public static string QuerySelectorAttribute(this HtmlNode value, string parttern, string attributename)
        {
            try
            {
                var node = value.QuerySelector(parttern);
                return node.Attributes[attributename].Value;
            }
            catch (Exception)
            {
                return "";
            }
        }
        /// <summary>
        /// 获取查找节点的InnerText(避免出现空指针异常)(不会返回null)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parttern"></param>
        /// <returns></returns>
        public static List<string> QuerySelectorInnerTextAll(this HtmlNode value, string parttern)
        {
            List<string> list = new List<string>();
            try
            {
                var nodelist = value.QuerySelectorAll(parttern);
                foreach (var item in nodelist)
                {
                    list.Add(item.InnerText);
                }
            }
            catch (Exception)
            {
            }
            return list;
        }
        /// <summary>
        /// 获取查找节点的属性(避免出现空指针异常)(不会返回null)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parttern"></param>
        /// <param name="attributename">属性名</param>
        /// <returns></returns>
        public static List<string> QuerySelectorAttributeAll(this HtmlNode value, string parttern, string attributename)
        {
            List<string> list = new List<string>();
            try
            {
                var nodelist = value.QuerySelectorAll(parttern);
                foreach (var item in nodelist)
                {
                    list.Add(item.Attributes[attributename].Value);
                }
            }
            catch (Exception)
            {
            }
            return list;
        }
        /// <summary>
        /// 查找html节点(不会返回null)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parttern"></param>
        /// <returns></returns>
        public static List<string> QuerySelectorAllStr(this HtmlNode value, string parttern)
        {
            try
            {
                List<string> listStr = new List<string>();
                var list = value.QuerySelectorAll(parttern).ToList();
                foreach (var node in list)
                {
                    listStr.Add(node.OuterHtml);
                }
                return listStr;
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }
        /// <summary>
        /// 查找html节点(不会返回null)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parttern"></param>
        /// <returns></returns>
        public static string QuerySelectorStr(this HtmlNode value, string parttern)
        {
            try
            {
                var node = value.QuerySelector(parttern);
                return node.OuterHtml;
            }
            catch (Exception)
            {
                return "";
            }
        }
        /// <summary>
        /// html格式string转HtmlNode(可能返回null)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static HtmlNode ToHtmlNode(this string value)
        {
            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(value);
                return doc.DocumentNode;
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// 去除换行符，回车符，制表符
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string TrimSpecial(this string value)
        {
            return value.Replace("\r", "").Replace("\n", "").Replace("\t", "");
        }
    }

    /// <summary>
    /// Cookie 扩展类
    /// </summary>
    public static class CookieExtension
    {
        /// <summary>
        /// ListCookie 转 CookieContainer 
        /// </summary>
        /// <param name="cc"></param>
        /// <returns></returns>
        public static CookieContainer ToCookieContainer(this List<Cookie> lc)
        {
            CookieContainer cc = new CookieContainer();
            cc.Capacity = 1024;
            cc.PerDomainCapacity = 1024;
            foreach (var c in lc)
            {
                cc.Add(c);
            }
            return cc;
        }
        /// <summary>
        /// CookieContainer 转 ListCookie
        /// </summary>
        /// <param name="cc"></param>
        /// <returns></returns>
        public static List<Cookie> ToListCookie(this CookieContainer cc)
        {
            return HtmlHelper.GetAllCookies(cc).Cast<Cookie>().ToList();
        }
        /// <summary>
        /// CookieContainer 转 CookieStr
        /// </summary>
        /// <param name="cc"></param>
        /// <returns></returns>
        public static string ToCookieStr(this CookieContainer cc)
        {
            string cookieStr = "";
            var list = cc.ToListCookie();
            foreach (Cookie cookie in list)
            {
                cookieStr += cookie.ToCookieStr();
            }
            return cookieStr;
        }
        /// <summary>
        /// Cookie 转 CookieStr
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string ToCookieStr(this Cookie c)
        {
            return string.Format("{0}={1};Domain={2};Path={3},", c.Name, c.Value, c.Domain, c.Path);
        }
    }
}
