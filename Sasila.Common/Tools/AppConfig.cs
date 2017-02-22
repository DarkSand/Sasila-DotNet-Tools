using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Configuration;
using System.Web.Script.Serialization;
using Sasila.Common.Http;
using Sasila.Common.DEncrypt;
using System.IO;

namespace Sasila.Common.Tools
{
    /// <summary>
    /// 程序运行状态
    /// </summary>
    public enum AppRunState
    {
        /// <summary>
        /// 正在运行
        /// </summary>
        Run = 0,
        /// <summary>
        /// 暂停状态
        /// </summary>
        Pause = 1,
        /// <summary>
        /// 停止状态
        /// </summary>
        Stop = 2
    }

    public class ParamsItem
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    /// <summary>
    /// config文件操作
    /// </summary>
    public class AppConfig
    {
        public AppConfig()
        {
        }

        /// <summary>
        /// 创建新的Config文件
        /// </summary>
        /// <param name="configFileName">config文件名</param>
        private void CreateNewConfigFile(string configFileName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlDeclaration declaration = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
            XmlNode rootNode = xmlDoc.CreateElement("configuration");
            XmlNode settingNode = xmlDoc.CreateElement("appSettings");
            xmlDoc.AppendChild(rootNode);
            rootNode.AppendChild(settingNode);
            xmlDoc.InsertBefore(declaration, xmlDoc.DocumentElement);
            xmlDoc.Save(configFileName);
        }

        /// <summary>
        /// 初始化AppConfig，主要是检测Config文件是否存在及合法性
        /// </summary>
        /// <param name="configFileName">Config文件名</param>
        public void InitConfig(string configFileName)
        {
            if (File.Exists(configFileName)) //存在检测节点是否合法
            {
                bool isLoad = false; //标识是否加载
                XmlDocument xmlDoc = new XmlDocument();
                try
                {
                    xmlDoc.Load(configFileName);
                    isLoad = true;
                }
                catch (Exception)
                {

                }
                if (isLoad) //加载成功，判断是否有appSettings节点，没有则重新生成
                {
                    XmlNode node = xmlDoc.SelectSingleNode("//appSettings");
                    if (null == node)
                    {
                        CreateNewConfigFile(configFileName);
                    }
                }
                else  //加载失败重新创建
                {
                    CreateNewConfigFile(configFileName);
                }
            }
            else //不存在重新创建
            {
                CreateNewConfigFile(configFileName);
            }
        }

        /// <summary>
        /// 程序运行状态
        /// </summary>
        public static AppRunState appRunState = AppRunState.Stop;

        /// <summary>
        /// config里面是否有当前键
        /// </summary>
        public bool HadKey { get; set; }
        /// <summary>
        /// 保存参数到config文件
        /// </summary>
        /// <param name="strkey"></param>
        /// <param name="strvalue"></param>
        public void SaveConfig(string strkey, object strvalue)
        {
            strvalue = null == strvalue ? string.Empty : strvalue.ToString();

            HadKey = false;
            if (strkey.Contains("_DES"))
            {
                strvalue = DESCryption.Encrypt(strvalue.ToString());
            }
            XmlDocument doc = new XmlDocument();
            //获得配置文件的全路径
            string strFileName = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            strFileName = strFileName.Insert(strFileName.LastIndexOf("\\"), "\\config");
            strFileName = strFileName.Replace(".vshost", "");
            if (!File.Exists(strFileName))
            {
                strFileName = strFileName.Replace(".config", ".exe.config");
            }
            System.IO.File.SetAttributes(strFileName, System.IO.FileAttributes.Normal);
            doc.Load(strFileName);
            //判断是否有该节点，没有则加上
            XmlNode node = doc.SelectSingleNode("//appSettings");
            XmlElement addElem = (XmlElement)node.SelectSingleNode("//add[@key='" + strkey + "']");
            if (addElem != null)
            {
                HadKey = true;
                addElem.SetAttribute("value", strvalue.ToString());
            }
            else
            {
                XmlElement entry = doc.CreateElement("add");
                entry.SetAttribute("key", strkey);
                entry.SetAttribute("value", strvalue.ToString());
                node.AppendChild(entry);
            }
            //保存上面的修改
            doc.Save(strFileName);
        }
        /// <summary>
        /// 从config文件读取数据
        /// </summary>
        /// <param name="strkey">键</param>
        /// <param name="defValue">值</param>
        /// <returns></returns>
        public string GetConfig(string strkey, object defValue)
        {
            defValue = null == defValue ? string.Empty : defValue.ToString();

            HadKey = false;
            //获得配置文件的全路径
            string strFileName = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            strFileName = strFileName.Insert(strFileName.LastIndexOf("\\"), "\\config");
            strFileName = strFileName.Replace(".vshost", "");
            if (!File.Exists(strFileName))
            {
                strFileName = strFileName.Replace(".config", ".exe.config");
            }
            XmlDocument doc = new XmlDocument();
            System.IO.File.SetAttributes(strFileName, System.IO.FileAttributes.Normal);
            doc.Load(strFileName);
            //判断是否有该节点，没有则加上
            XmlNode node = doc.SelectSingleNode("//appSettings");
            XmlElement addElem = (XmlElement)node.SelectSingleNode("//add[@key='" + strkey + "']");
            //string result = ConfigurationManager.AppSettings[strkey];
            string result = "";
            if (addElem != null)
            {
                HadKey = true;
                result = addElem.GetAttribute("value");

                if (strkey.Contains("_DES"))
                {
                    result = DESCryption.Decrypt(result.ToString());
                }
            }
            else
            {
                //获取的时候不新建节点
                //XmlElement entry = doc.CreateElement("add");
                //entry.SetAttribute("key", strkey);
                //entry.SetAttribute("value", defValue.ToString());
                //node.AppendChild(entry);
                result = defValue.ToString();
            }
            doc.Save(strFileName);
            return result;
        }
        /// <summary>
        /// 从服务器获取配置值
        /// </summary>
        /// <param name="strKey">获取的参数名</param>
        /// <param name="defValue">默认的参数值</param>
        /// <returns></returns>
        public string GetWebConfig(string strKey, object defValue)
        {
            string result = string.Empty;
            HttpWebApp openweb = new HttpWebApp();
            JavaScriptSerializer javaSer = new JavaScriptSerializer();
            string activeUrl = string.Format("http://reg.joy-software.com/AjaxService/GetParams.ashx?action={0}&type=.net", DESCryption.Encrypt(strKey));
            try
            {
                string tempHtml = openweb.DoGet(activeUrl);
                Dictionary<string, object> dic = javaSer.Deserialize<Dictionary<string, object>>(tempHtml);
                if (string.IsNullOrEmpty(dic["ErrorMsg"].ToString()))
                {
                    tempHtml = StrHelper.Abstract(tempHtml, "[", "]");
                    tempHtml = "[" + tempHtml + "]";
                    List<ParamsItem> list = javaSer.Deserialize<List<ParamsItem>>(tempHtml);
                    result = list.Single(l => l.Key == strKey).Value;
                }
            }
            catch (Exception)
            {
                //异常不处理
            }
            finally
            {
                if (string.IsNullOrEmpty(result))
                {
                    //获取config文件数据或默认数据
                    result = GetConfig(strKey, defValue);
                }
                SaveConfig(strKey, result);
            }
            return result;
        }
        /// <summary>
        /// 批量获取网络参数
        /// </summary>
        /// <param name="keys">获取的参数名和值键值对</param>
        /// <returns></returns>
        public Dictionary<string, object> GetWebConfig(Dictionary<string, object> keys)
        {
            string result = string.Empty;
            string strKey = "";
            Dictionary<string, object> dicParams = new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> s in keys)
            {
                strKey += string.IsNullOrEmpty(strKey) ? s.Key : "," + s.Key;
            }
            if (string.IsNullOrEmpty(strKey))
                return dicParams;
            HttpWebApp openweb = new HttpWebApp();
            JavaScriptSerializer javaSer = new JavaScriptSerializer();
            string activeUrl = string.Format("http://reg.joy-software.com/AjaxService/GetParams.ashx?action={0}&type=.net", DESCryption.Encrypt(strKey));
            try
            {
                string tempHtml = openweb.DoGet(activeUrl);
                Dictionary<string, object> dic = javaSer.Deserialize<Dictionary<string, object>>(tempHtml);
                if (string.IsNullOrEmpty(dic["ErrorMsg"].ToString()))
                {
                    tempHtml = StrHelper.Abstract(tempHtml, "[", "]");
                    tempHtml = "[" + tempHtml + "]";
                    List<ParamsItem> list = javaSer.Deserialize<List<ParamsItem>>(tempHtml);
                    foreach (KeyValuePair<string, object> s in keys)
                    {
                        result = list.Single(l => l.Key == s.Key).Value;
                        if (!string.IsNullOrEmpty(result) && !dicParams.Keys.Contains(s.Key))
                        {
                            dicParams.Add(s.Key, result);
                        }
                    }
                }
            }
            catch (Exception)
            {
                //异常不处理，返回默认值
            }
            finally
            {
                //获取未获取到的参数
                if (dicParams.Count != keys.Count)
                {
                    foreach (KeyValuePair<string, object> s in keys)
                    {
                        if (!dicParams.Keys.Contains(s.Key))
                        {
                            dicParams.Add(s.Key, GetConfig(s.Key, s.Value));
                        }
                    }
                }
                //保存参数
                foreach (KeyValuePair<string, object> s in dicParams)
                {
                    SaveConfig(s.Key, s.Value);
                }
            }
            return dicParams;
        }
    }
}
