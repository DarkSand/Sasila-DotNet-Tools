using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Data;

namespace Sasila.Common.Tools
{
    /// <summary>
    /// Xml的操作公共类
    /// </summary>
    public class XmlHelper
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public XmlHelper()
        {

        }

        /// <summary>  
        /// 导入XML文件  
        /// </summary>  
        /// <param name="xmlPath">XML文件路径</param>  
        public static XmlDocument XMLLoad(string xmlPath)
        {
            XmlDocument xmldoc = new XmlDocument();
            try
            {
                string filename = AppDomain.CurrentDomain.BaseDirectory.ToString() + xmlPath;
                if (File.Exists(filename))
                {
                    xmldoc.Load(filename);
                }
            }
            catch (Exception e)
            { }
            return xmldoc;
        }

        /// <summary>  
        /// 读取指定路径和节点的串联值  
        /// </summary>  
        /// <param name="path">XML文件路径</param>  
        /// <param name="node">节点</param>   
        /// 使用示列:  
        /// XmlHelper.Read(path, "/Node", "")  
        /// XmlHelper.Read(path, "/Node/Element[@Attribute='Name']")  
        public static string Read(string path, string node)
        {
            string value = "";
            try
            {
                XmlDocument doc = XMLLoad(path);
                XmlNode xn = doc.SelectSingleNode(node);
                value = xn.InnerText;
            }
            catch { }
            return value;
        }

        /// <summary>  
        /// 读取指定路径和节点的属性值  
        /// </summary>  
        /// <param name="path">XML文件路径</param>  
        /// <param name="node">节点</param>  
        /// <param name="attribute">属性名，非空时返回该属性值，否则返回串联值</param>  
        /// 使用示列:  
        /// XmlHelper.Read(path, "/Node", "")  
        /// XmlHelper.Read(path, "/Node/Element[@Attribute='Name']", "Attribute")  
        public static string Read(string path, string node, string attribute)
        {
            string value = "";
            try
            {
                XmlDocument doc = XMLLoad(path);
                XmlNode xn = doc.SelectSingleNode(node);
                value = (attribute.Equals("") ? xn.InnerText : xn.Attributes[attribute].Value);
            }
            catch { }
            return value;
        }

        /// <summary>
        /// 读取指定路径和所有节点集合
        /// </summary>
        /// <param name="path">XML文件路径</param>
        /// <param name="node">节点</param>
        /// 使用示列:  
        /// XmlHelper.Read(path, "/Node", "")  
        /// XmlHelper.Read(path, "/Node/Element[@Attribute='Name']", "Attribute")  
        /// <returns></returns>
        public static XmlNodeList ReadNodeList(string path, string node)
        {
            XmlNodeList listNodes = null;
            try
            {
                XmlDocument doc = XMLLoad(path);
                listNodes = doc.SelectNodes(node);
            }
            catch { }
            return listNodes;
        }

        /// <summary>
        /// XML转DataSet
        /// </summary>
        /// <param name="xmlData">xml原始数据</param>
        /// <returns></returns>
        public static DataSet ConvertXMLToDataSet(string xmlData)
        {
            StringReader stream = null;
            XmlTextReader reader = null;
            try
            {
                DataSet xmlDS = new DataSet();
                stream = new StringReader(xmlData);
                reader = new XmlTextReader(stream);
                xmlDS.ReadXml(reader);
                return xmlDS;
            }
            catch (Exception ex)
            {
                string strTest = ex.Message;
                return null;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
                if (reader != null)
                    reader.Close();
            }
        }
    }
}
