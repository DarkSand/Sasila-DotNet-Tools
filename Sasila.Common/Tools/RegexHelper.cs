using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Sasila.Common.Tools
{
    /// <summary>
    /// 操作正则表达式的公共类
    /// </summary>    
    public class RegexHelper
    {
        /// <summary>
        /// 验证输入字符串是否与模式字符串匹配，匹配返回true
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <param name="pattern">模式字符串</param>        
        public static bool IsMatch(string input, string pattern)
        {
            return IsMatch(input, pattern, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 验证输入字符串是否与模式字符串匹配，匹配返回true
        /// </summary>
        /// <param name="input">输入的字符串</param>
        /// <param name="pattern">模式字符串</param>
        /// <param name="options">筛选条件</param>
        public static bool IsMatch(string input, string pattern, RegexOptions options)
        {
            return Regex.IsMatch(input, pattern, options);
        }

        /// <summary>
        /// input正则表达式
        /// </summary>
        public const string Input = "<input[^>]*>";

        /// <summary>
        /// 按给定正则表达式和html进行匹配
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="html"></param>
        /// <returns></returns>
        public static MatchCollection GetMatchs(string pattern, string html)
        {
            Regex reg = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            return reg.Matches(html);
        }
        /// <summary>
        /// 按给定正则表达式和html进行匹配,返回ListString
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="html"></param>
        /// <returns></returns>
        public static List<string> GetMatchsList(string pattern, string html)
        {
            List<string> list = new List<string>();
            Regex reg = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var mc = reg.Matches(html);
            foreach (Match m in mc)
            {
                list.Add(m.Value);
            }
            return list;
        }

        /// <summary>
        /// 按给定正则表达式和html进行匹配,返回ListString
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="html"></param>
        /// /// <param name="groupNo">分组编号</param>
        /// <returns></returns>
        public static List<string> GetMatchsList(string pattern, string html,int groupNo)
        {
            List<string> list = new List<string>();
            Regex reg = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var mc = reg.Matches(html);
            foreach (Match m in mc)
            {
                if (m.Groups.Count-1 >= groupNo)
                list.Add(m.Groups[groupNo].Value);
            }
            return list;
        }

        /// <summary>
        /// 批量获取input数据
        /// </summary>
        /// <param name="str">输入字符串</param>
        /// <param name="onlyHidden">是只获取隐藏的input</param>
        /// <returns>返回数据字典</returns>
        public static Dictionary<string, string> GetInput(string str,bool onlyHidden)
        {
            Dictionary<string, string> postData = new Dictionary<string, string>();
            MatchCollection mat = GetMatchs(Input, str);
            string postName = "";
            string postValue = "";
            string strTemp = "";
            foreach (Match m in mat)
            {
                strTemp = m.Value.Replace(" ", "");
                if (onlyHidden && !m.Value.Contains("hidden"))
                    continue;
                postName = StrHelper.Abstract(strTemp, "name=\"", "\"");
                if (string.IsNullOrEmpty(postName))
                    postName = StrHelper.Abstract(strTemp, "name='", "'");
                if (string.IsNullOrEmpty(postName))
                    postName = StrHelper.Abstract(strTemp, "id=\"", "\"");
                if (string.IsNullOrEmpty(postName))
                    continue;
                else
                {
                    postValue = StrHelper.Abstract(strTemp, "value=\"", "\"");
                    if (string.IsNullOrEmpty(postValue))
                        postValue = StrHelper.Abstract(strTemp, "value='", "'");
                    try
                    {
                        postData.Add(postName, postValue);
                    }
                    catch
                    {
                    }
                }
            }
            return postData;
        }
        /// <summary>
        /// 根据form表单名批量获取input数据
        /// </summary>
        /// <param name="str">html</param>
        /// <param name="fromName">form表单名</param>
        /// <param name="onlyHidden">是只获取隐藏的input</param>
        /// <returns>返回数据字典</returns>
        public static Dictionary<string, string> GetInput(string str, string fromName, bool onlyHidden)
        {
            Dictionary<string, string> postData = new Dictionary<string, string>();
            if (str.Contains("name=\"" + fromName + "\""))
            {
                str = StrHelper.Abstract(str, "name=\"" + fromName + "\"", "</form>");
            }
            else if (str.Contains("id=\"" + fromName + "\""))
            {
                str = StrHelper.Abstract(str, "id=\"" + fromName + "\"", "</form>");
            }
            else
            {
                return postData;
            }
            return GetInput(str,onlyHidden);
        }

        /// <summary>
        /// 格式化正则表达式
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static string Fomart(string s)
        {
            s = s.Replace(@"\", @"\\");
            s = s.Replace(".", @"\.");
            s = s.Replace("(", @"\(");
            s = s.Replace(")", @"\)");
            s = s.Replace("*", @"\*");
            s = s.Replace("?", @"\?");
            s = s.Replace("!", @"\!");
            s = s.Replace("$", @"\$");
            s = s.Replace("^", @"\^");
            s = s.Replace("[", @"\[");
            s = s.Replace("]", @"\]");
            s = s.Replace("|", @"\|");
            s = s.Replace("+", @"\+");
            s = s.Replace("{", @"\{");
            s = s.Replace("}", @"\}");
            //s = s.Replace("", @"\");
            return s;
        }

        /// <summary>
        /// 判断标签是否成对出现
        /// </summary>
        /// <param name="Tag"></param>
        /// <returns></returns>
        private static bool IsDouble(string Tag)
        {
            if (Tag == "br" || Tag == "hr"
                || Tag == "area"
                || Tag == "base"
                || Tag == "img"
                || Tag == "input"
                || Tag == "link"
                || Tag == "meta"
                || Tag == "basefont"
                || Tag == "param"
                || Tag == "col"
                || Tag == "frame")
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 获取标签中属性的值
        /// </summary>
        /// <param name="html">输入的字符串</param>
        /// <param name="Tag">标签名字</param>
        /// <param name="Attributes">属性名字</param>
        public static List<string> GetTagAttributes(string html, string Tag, string Attributes)
        {
            List<string> list = new List<string>();
            string pattern = "";
            if (IsDouble(Tag))
            {
                pattern = string.Format("<{0}((?!>).)*?{1}='(((?!>).)*?)'((?!>).)*?>|<{0}((?!>).)*?{1}=\"(((?!>).)*?)\"((?!>).)*?>", Tag, Attributes);
            }
            else
            {
                pattern = string.Format("<{0}((?!>).)*?{1}='(((?!>).)*?)'((?!>).)*?/>|<{0}((?!>).)*?{1}=\"(((?!>).)*?)\"((?!>).)*?/>", Tag, Attributes);
            }

            var mc = GetMatchs(pattern, html);
            foreach (Match m in mc)
            {
                if (string.IsNullOrEmpty(m.Groups[2].Value))
                {
                    list.Add(m.Groups[6].Value);
                }
                else
                {
                    list.Add(m.Groups[2].Value);
                }
            }
            return list;
        }

        /// <summary>
        /// 根据标签中的特征值，查找标签
        /// </summary>
        /// <param name="html">输入字符串</param>
        /// <param name="Tag">标签名字</param>
        /// <param name="Characteristic">特征字符串</param>
        /// <returns></returns>
        public static List<string> GetTagbyCharacteristic(string html, string Tag, string Characteristic)
        {
            Characteristic = Fomart(Characteristic);
            string pattern = "";
            List<string> list = new List<string>();
            if (IsDouble(Tag))
            {
                pattern = string.Format("<{0}((?!>).)*?{1}((?!>).)*?>", Tag, Characteristic);
            }
            else
            {
                pattern = string.Format("<{0}((?!>).)*?{1}((?!>).)*?/>", Tag, Characteristic);
            }
            var mc = GetMatchs(pattern, html);
            foreach (Match m in mc)
            {
                list.Add(m.Value);
            }
            return list;
        }

        /// <summary>
        /// 通过前后特征，匹配数据
        /// </summary>
        /// <param name="html">输入字符串</param>
        /// <param name="start">前特征</param>
        /// <param name="end">后特征</param>
        /// <returns></returns>
        public static List<string> RegexAbstract(string html, string start, string end)
        {
            start = Fomart(start);
            end = Fomart(end);
            List<string> list = new List<string>();
            string pattern = string.Format("{0}.*?{1}",start,end);

            var mc = GetMatchs(pattern, html);
            foreach (Match m in mc)
            {
                list.Add(m.Value);
            }
            return list;
        }

        /// <summary>
        /// 通过前中后特征，匹配数据
        /// </summary>
        /// <param name="html">输入字符串</param>
        /// <param name="start">前特征</param>
        /// <param name="middle">中特征</param>
        /// <param name="end">后特征</param>
        /// <returns></returns>
        public static List<string> RegexAbstract(string html, string start, string middle, string end)
        {
            start = Fomart(start);
            end = Fomart(end);
            middle = Fomart(middle);
            List<string> list = new List<string>();
            string pattern = string.Format("{0}.*?{1}.*?{2}", start,middle,end);

            var mc = GetMatchs(pattern, html);
            foreach (Match m in mc)
            {
                list.Add(m.Value);
            }
            return list;
        }

        /// <summary>
        /// 根据标签特征值获取完整标签内容
        /// </summary>
        /// <param name="html">输入字符串</param>
        /// <param name="Tag">标签名</param>
        /// <param name="Characteristic">特征值</param>
        /// <param name="isFull">是否获取完整内容，若为false则不包含标签，只有innerhtml</param>
        /// <returns></returns>
        public static List<string> GetTagInnerHtml(string html,string Tag, string Characteristic,bool isFull)
        {
            Characteristic = Fomart(Characteristic);
            List<string> list = new List<string>();
            string pattern = "";
            if (IsDouble(Tag))
            {
                pattern = string.Format("<{0}((?!>).)*?{1}((?!>).)*?>(.*?)</{0}>", Tag, Characteristic);
            }
            else
            {
                pattern = string.Format("<{0}((?!>).)*?{1}((?!>).)*?/>", Tag, Characteristic);
            }

            var mc = GetMatchs(pattern, html);
            foreach (Match m in mc)
            {
                if (isFull)
                {
                    list.Add(m.Value);
                }
                else
                {
                    if (IsDouble(Tag))
                    {
                        list.Add(m.Groups[3].Value);
                    }
                    else
                    {
                        list.Add(m.Value);
                    }
                }
            }
            return list;
        }
    }
}
