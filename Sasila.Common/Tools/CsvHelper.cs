using System.Data;
using System.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sasila.Common.Tools
{
    /// <summary>
    ///     文件转换类
    /// </summary>
    public static class CsvHelper
    {
        /// <summary>
        /// 通过文本获取分隔符（检测文本前10行）
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string GetDataSpeciByText(string data)
        {
            var spcei = string.Empty;
            if (string.IsNullOrEmpty(data))
            {
                return spcei;
            }
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(data)))
            {
                using (StreamReader reader = new StreamReader(ms, false))
                {
                    reader.Peek();
                    while (reader.Peek() > 0)
                    {
                        string str = reader.ReadLine();
                        if (string.IsNullOrEmpty(str)) //过滤掉空行
                        {
                            continue;
                        }
                        if (str.Contains("----"))
                        {
                            spcei = "----";
                        }
                        else if (str.Contains("\t"))
                        {
                            spcei = "\t";
                        }
                        else if (str.Contains(","))
                        {
                            spcei = ",";
                        }
                        else
                        {
                            spcei = ",";
                        }
                        break;
                    }
                }
            }
            return spcei;
        }

        /// <summary>
        /// 通过文件获取分隔符（检测文本前10行）
        /// </summary>
        /// <param name="strFilePath">文件路径</param>
        /// <returns></returns>
        public static string GetDataSpeciByFile(string strFilePath)
        {
            var spcei = string.Empty;
            if (!File.Exists(strFilePath))
            {
                return spcei;
            }
            using (StreamReader reader = new StreamReader(strFilePath, Encoding.UTF8, false))
            {
                reader.Peek();
                while (reader.Peek() > 0)
                {
                    string str = reader.ReadLine();
                    if (string.IsNullOrEmpty(str)) //过滤掉空行
                    {
                        continue;
                    }
                    if (str.Contains("----"))
                    {
                        spcei = "----";
                    }
                    else if (str.Contains("\t"))
                    {
                        spcei = "\t";
                    }
                    else if (str.Contains(","))
                    {
                        spcei = ",";
                    }
                    else
                    {
                        spcei = ",";
                    }
                    break;
                }
            }
            return spcei;
        }

        /// <summary>
        /// 将文本内容转换为DataTable
        /// </summary>
        /// <param name="data">文本</param>
        /// <param name="speci">每一列数据分隔符</param>
        /// <param name="dt">Source DataTable</param>
        /// <returns></returns>
        public static DataTable TextToDataTable(string data, string speci, DataTable dt)
        {
            try
            {
                if (string.IsNullOrEmpty(speci))
                {
                    speci = GetDataSpeciByText(data);
                }
                if (!string.IsNullOrEmpty(speci))
                {
                    using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(data)))
                    {
                        using (StreamReader reader = new StreamReader(ms, false))
                        {
                            reader.Peek();
                            while (reader.Peek() > 0)
                            {
                                string str = reader.ReadLine();
                                if (string.IsNullOrEmpty(str)) //过滤掉空行
                                {
                                    continue;
                                }
                                string[] split = str.Split(new string[] { speci }, StringSplitOptions.None);

                                DataRow dr = dt.NewRow();
                                for (int i = 0; i < dt.Columns.Count && i < split.Length; i++)
                                {
                                    dr[i] = split[i];
                                }
                                dt.Rows.Add(dr);
                            }
                        }
                    }
                }
                else
                {
                    data = "未识别到分隔符";
                }
            }
            catch (Exception ex)
            {
                data = ex.Message;
            }
            return dt;
        }

        /// <summary>
        /// 将文件内容转换为DataTable
        /// </summary>
        /// <param name="strFilePath">文件路径</param>
        /// <param name="speci">每一列数据分隔符（空则根据文件类型识别,支持.txt,.csv,.xls）</param>
        /// <param name="dt">Source DataTable</param>
        /// <returns></returns>
        public static DataTable FileToDataTable(string strFilePath, string speci, DataTable dt)
        {
            try
            {
                if (string.IsNullOrEmpty(speci))
                {
                    if (Path.GetExtension(strFilePath).ToLower() == ".txt")
                    {
                        speci = GetDataSpeciByFile(strFilePath);
                    }
                    else if (Path.GetExtension(strFilePath).ToLower() == ".csv")
                    {
                        speci = ",";
                    }
                    else if (Path.GetExtension(strFilePath).ToLower() == ".xls")
                    {
                        speci = "\t";
                    }
                }
                if (!string.IsNullOrEmpty(speci))
                {
                    using (StreamReader reader = new StreamReader(strFilePath, System.Text.Encoding.Default, false))
                    {
                        reader.Peek();
                        while (reader.Peek() > 0)
                        {
                            string str = reader.ReadLine();
                            if (string.IsNullOrEmpty(str)) //过滤掉空行
                            {
                                continue;
                            }
                            string[] split = str.Split(new string[] { speci }, StringSplitOptions.None);

                            DataRow dr = dt.NewRow();
                            for (int i = 0; i < dt.Columns.Count && i < split.Length; i++)
                            {
                                dr[i] = split[i];
                            }
                            dt.Rows.Add(dr);
                        }
                    }
                }
                else
                {
                    strFilePath = "未识别到分隔符";
                }
            }
            catch (Exception ex)
            {
                strFilePath = ex.Message;
            }
            return dt;
        }

        /// <summary>
        /// DataTable导出到文件
        /// </summary>
        /// <param name="dt">Source DataTable</param>
        /// <param name="strFilePath">导出文件路径</param>
        /// <param name="columname">列名</param>
        /// <param name="speci">分隔</param>
        /// <returns></returns>
        public static bool DataTableToFile(DataTable dt, string strFilePath, string columname, string speci)
        {
            try
            {
                using (StreamWriter strmWriterObj = new StreamWriter(strFilePath, false, System.Text.Encoding.Default))
                {
                    if (!string.IsNullOrEmpty(columname))
                    {
                        strmWriterObj.WriteLine(columname);
                    }
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        sb = new StringBuilder();
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            if (j > 0)
                            {
                                sb.Append(speci);
                            }
                            string content = dt.Rows[i][j].ToString();
                            if (content.Contains(","))
                            {
                                content = content.Replace(',', '，');//修改时间：2016-5-9,修改人：李奡煦熙，修改原因：为了不影响Csv文件默然的根据英文逗号分隔多列数据，在这里把导出内容中原有的英文逗号替换为中文逗号。
                            }
                            sb.Append(content);
                        }
                        sb = sb.Replace("\r\n", "");
                        strmWriterObj.WriteLine(sb.ToString());
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /*下面的是以前的方法，现在都用上面新写的，下面的以后会删除的*/

        /// <summary>
        /// 导出报表为Csv
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <param name="strFilePath">物理路径</param>
        /// <param name="tableheader">表头</param>
        /// <param name="columname">字段标题,逗号分隔</param>
        public static bool dt2csv(DataTable dt, string strFilePath, string tableheader, string columname)
        {
            try
            {
                string strBufferLine = "";
                StreamWriter strmWriterObj = new StreamWriter(strFilePath, false, System.Text.Encoding.GetEncoding("gb2312"));
                if (!string.IsNullOrEmpty(tableheader))
                    strmWriterObj.WriteLine(tableheader);
                strmWriterObj.WriteLine(columname);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    strBufferLine = "";
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        if (j > 0)
                            strBufferLine += ",";
                        string content = dt.Rows[i][j].ToString();
                        if (content.Contains(","))
                        {
                            content = content.Replace(',', '，');//修改时间：2016-5-9,修改人：李奡煦熙，修改原因：为了不影响Csv文件默然的根据英文逗号分隔多列数据，在这里把导出内容中原有的英文逗号替换为中文逗号。
                        }
                        strBufferLine += content;
                    }
                    strBufferLine = strBufferLine.Replace("\r\n", "");
                    strBufferLine = strBufferLine.Replace("\t", "");
                    strmWriterObj.WriteLine(strBufferLine);
                }
                strmWriterObj.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 将Csv读入DataTable(按文件列)
        /// </summary>
        /// <param name="filePath">csv文件路径</param>
        /// <param name="n">表示第n行是字段title,第n+1行是记录开始</param>
        /// <param name="dt"></param>
        public static DataTable csv2dt(string filePath, int n, DataTable dt)
        {
            try
            {
                StreamReader reader = new StreamReader(filePath, System.Text.Encoding.GetEncoding("gb2312"), false);
                int i = 0, m = 0;
                reader.Peek();
                while (reader.Peek() > 0)
                {
                    m = m + 1;
                    string str = reader.ReadLine();
                    if (m >= n + 1)
                    {
                        str = str.Replace("；", ";");
                        string[] split = str.Split(',');

                        System.Data.DataRow dr = dt.NewRow();
                        for (i = 0; i < split.Length; i++)
                        {
                            dr[i] = split[i];
                        }
                        dt.Rows.Add(dr);
                    }
                }

            }
            catch (Exception ex)
            {
                filePath = ex.Message;
            }
            return dt;
        }
        /// <summary>
        /// 将Csv读入DataTable(按datatable列)
        /// </summary>
        /// <param name="filePath">csv文件路径</param>
        /// <param name="n">表示第n行是字段title,第n+1行是记录开始</param>
        /// <param name="dt"></param>
        public static DataTable csv2dtEx(string filePath, int n, DataTable dt)
        {
            try
            {
                StreamReader reader = new StreamReader(filePath, System.Text.Encoding.GetEncoding("gb2312"), false);
                int i = 0, m = 0;
                reader.Peek();
                while (reader.Peek() > 0)
                {
                    m = m + 1;
                    string str = reader.ReadLine();
                    if (m >= n + 1)
                    {
                        str = str.Replace("；", ";");
                        string[] split = str.Split(',');

                        System.Data.DataRow dr = dt.NewRow();
                        for (i = 0; i < dt.Columns.Count; i++)
                        {
                            dr[i] = split[i];
                        }
                        dt.Rows.Add(dr);
                    }
                }
            }
            catch (Exception ex)
            {
                filePath = ex.Message;
            }
            return dt;
        }

        #region 以“----”分隔
        /// <summary>
        /// 将Csv读入DataTable(按文件列)以“----”分隔
        /// </summary>
        /// <param name="filePath">csv文件路径</param>
        /// <param name="n">表示第n行是字段title,第n+1行是记录开始</param>
        /// <param name="dt"></param>
        public static DataTable csv2dtSpeci(string filePath, int n, DataTable dt)
        {
            try
            {
                StreamReader reader = new StreamReader(filePath, System.Text.Encoding.GetEncoding("gb2312"), false);
                int i = 0, m = 0;
                reader.Peek();
                while (reader.Peek() > 0)
                {
                    m = m + 1;
                    string str = reader.ReadLine();
                    string tempstr;
                    if (m >= n + 1)
                    {
                        str = str.Replace("；", ";");
                        tempstr = str;
                        List<string> split = new List<string>();
                        do
                        {
                            if (str.Contains("----"))
                            {
                                tempstr = StrHelper.Abstract(str, "", "----");
                                split.Add(tempstr);
                                str = str.Replace(tempstr + "----", "");
                            }
                            else
                            {
                                tempstr = str;
                                if (!string.IsNullOrEmpty(tempstr))
                                {
                                    split.Add(tempstr);
                                }
                                break;
                            }
                        } while (true);

                        System.Data.DataRow dr = dt.NewRow();
                        for (i = 0; i < split.Count; i++)
                        {
                            dr[i] = split[i];
                        }
                        dt.Rows.Add(dr);
                    }
                }

            }
            catch (Exception ex)
            {
                filePath = ex.Message;
            }
            return dt;
        }
        /// <summary>
        /// 将Csv读入DataTable(按datatable列)以“----”分隔
        /// </summary>
        /// <param name="filePath">csv文件路径</param>
        /// <param name="n">表示第n行是字段title,第n+1行是记录开始</param>
        /// <param name="dt"></param>
        public static DataTable csv2dtExSpeci(string filePath, int n, DataTable dt)
        {
            try
            {
                StreamReader reader = new StreamReader(filePath, System.Text.Encoding.GetEncoding("gb2312"), false);
                int i = 0, m = 0;
                reader.Peek();
                while (reader.Peek() > 0)
                {
                    m = m + 1;
                    string str = reader.ReadLine();
                    string tempstr;
                    if (m >= n + 1)
                    {
                        str = str.Replace("；", ";");
                        tempstr = str;
                        List<string> split = new List<string>();
                        do
                        {
                            if (str.Contains("----"))
                            {
                                tempstr = StrHelper.Abstract(str, "", "----");
                                split.Add(tempstr);
                                str = str.Replace(tempstr + "----", "");
                            }
                            else
                            {
                                tempstr = str;
                                if (!string.IsNullOrEmpty(tempstr))
                                {
                                    split.Add(tempstr);
                                }
                                break;
                            }
                        } while (true);

                        System.Data.DataRow dr = dt.NewRow();
                        for (i = 0; i < dt.Columns.Count; i++)
                        {
                            dr[i] = split[i];
                        }
                        dt.Rows.Add(dr);
                    }
                }

            }
            catch (Exception ex)
            {
                filePath = ex.Message;
            }
            return dt;
        }
        #endregion
    }
}
