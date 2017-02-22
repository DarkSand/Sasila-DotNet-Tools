using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Resources;

namespace Sasila.Common.Tools
{
    /// <summary>
    /// 随机中文、数字、字母Helper
    /// </summary>
    public class RandomHelper
    {
        public static ResourceManager resourceManager = new ResourceManager(typeof(ReceiveInfoResource));
        /// <summary>
        /// 随机姓列表[x]
        /// </summary>
        public static List<string> surNameList { get; set; }
        /// <summary>
        /// 随机名列表[m]
        /// </summary>
        public static List<string> nameList { get; set; }
        /// <summary>
        /// 随机街道列表[r]
        /// </summary>
        public static List<string> streetList { get; set; }
        /// <summary>
        /// 随机镇列表[t]
        /// </summary>
        public static List<string> townList { get; set; }
        /// <summary>
        /// 随机乡列表[c]
        /// </summary>
        public static List<string> countrysideList { get; set; }

        /// <summary>
        /// 随机汉字
        /// </summary>
        /// <param name="count">随机位数</param>
        /// <returns></returns>
        public static string GetRandomChinese(int count)
        {
            if (count <= 0)
            {
                return string.Empty;
            }
            Encoding gb = Encoding.GetEncoding("gb2312");
            object[] bytes = CreateRegionCode(count);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                string temp = gb.GetString((byte[])Convert.ChangeType(bytes[i], typeof(byte[])));
                sb.Append(temp);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取随机姓
        /// </summary>
        /// <returns></returns>
        public static string GetRandomSurname()
        {
            string temp = string.Empty;
            Random r = new Random(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0));
            if (null == surNameList || surNameList.Count == 0)
            {
                object tempObj = resourceManager.GetObject("surname");
                if (null != tempObj && !string.IsNullOrEmpty(tempObj.ToString()))
                {
                    surNameList = tempObj.ToString().Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    temp = surNameList[r.Next(0, surNameList.Count)];
                }
            }
            else
            {
                temp = surNameList[r.Next(0, surNameList.Count)];
            }
            return temp;
        }
        /// <summary>
        /// 获取随机名
        /// </summary>
        /// <returns></returns>
        public static string GetRandomName()
        {
            string temp = string.Empty;
            Random r = new Random(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0));
            if (null == nameList || nameList.Count == 0)
            {
                object tempObj = resourceManager.GetObject("name");
                if (null != tempObj && !string.IsNullOrEmpty(tempObj.ToString()))
                {
                    nameList = tempObj.ToString().Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    temp = nameList[r.Next(0, nameList.Count)];
                }
            }
            else
            {
                temp = nameList[r.Next(0, nameList.Count)];
            }
            return temp;
        }
        /// <summary>
        /// 获取一个镇名
        /// </summary>
        /// <returns></returns>
        public static string GetRandomTownName()
        {
            string temp = string.Empty;
            Random r = new Random(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0));
            if (null == townList || townList.Count == 0)
            {
                object tempObj = resourceManager.GetObject("town");
                if (null != tempObj && !string.IsNullOrEmpty(tempObj.ToString()))
                {
                    townList = tempObj.ToString().Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    temp = townList[r.Next(0, townList.Count)];
                }
            }
            else
            {
                temp = townList[r.Next(0, townList.Count)];
            }
            return temp;
        }
        /// <summary>
        /// 获取一个乡名
        /// </summary>
        /// <returns></returns>
        public static string GetRandomCoutrySideName()
        {
            string temp = string.Empty;
            Random r = new Random(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0));
            if (null == countrysideList || countrysideList.Count == 0)
            {
                object tempObj = resourceManager.GetObject("town");
                if (null != tempObj && !string.IsNullOrEmpty(tempObj.ToString()))
                {
                    countrysideList = tempObj.ToString().Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    temp = countrysideList[r.Next(0, countrysideList.Count)];
                }
            }
            else
            {
                temp = countrysideList[r.Next(0, countrysideList.Count)];
            }
            return temp;
        }
        /// <summary>
        /// 获取一个街道名
        /// </summary>
        /// <returns></returns>
        public static string GetRandomStreetName()
        {
            string temp = string.Empty;
            Random r = new Random(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0));
            if (null == streetList || streetList.Count == 0)
            {
                object tempObj = resourceManager.GetObject("street");
                if (null != tempObj && !string.IsNullOrEmpty(tempObj.ToString()))
                {
                    streetList = tempObj.ToString().Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    temp = streetList[r.Next(0, streetList.Count)];
                }
            }
            else
            {
                temp = streetList[r.Next(0, streetList.Count)];
            }
            return temp;
        }
        /// <summary>
        /// 获取指定位数字母组成的字符串
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string GetStrRandom(int count)
        {
            string temp = "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";
            string res = "";
            string[] data = temp.Split(',');

            for (int i = 0; i < count; i++)
            {
                Random r = new Random(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0));
                res += data[r.Next(52)];
            }
            return res;
        }
        /// <summary>
        /// 获取指定位数数字组成的字符串
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string GetIntRandom(int count)
        {
            string temp = "0,1,2,3,4,5,6,7,8,9";
            string res = "";
            string[] data = temp.Split(',');

            for (int i = 0; i < count; i++)
            {
                Random r = new Random(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0));
                res += data[r.Next(10)];
            }
            return res;
        }
        /// <summary>
        /// 获取随机数字
        /// </summary>
        /// <param name="minValue">下限</param>
        /// <param name="maxValue">上限</param>
        /// <returns></returns>
        public static int GetIntRandom(int minValue, int maxValue)
        {
            Random r = new Random(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0));
            return r.Next(minValue, maxValue);
        }

        /// <summary>
        /// 按指定格式生成随机字符串
        /// </summary>
        /// <param name="randomStr">[d]数字[s]字母</param>
        /// <returns></returns>
        public static string GetRandomResult(string randomStr)
        {
            string[] data = randomStr.Split(']');
            string res = "";
            foreach (string s in data)
            {
                string temp = s;
                if (s.Contains("[d"))
                {
                    temp = temp.Replace("[d", GetIntRandom(1));
                }
                if (s.Contains("[s"))
                {
                    temp = temp.Replace("[s", GetStrRandom(1));
                }
                if (s.Contains("[w"))
                {
                    temp = temp.Replace("[w", GetStrRandom(1)).ToLower();
                }
                if (s.Contains("[W"))
                {
                    temp = temp.Replace("[W", GetStrRandom(1)).ToUpper();
                }
                if (s.Contains("[z"))
                {
                    temp = temp.Replace("[z", GetRandomChinese(1));
                }
                if (s.Contains("[x"))
                {
                    temp = temp.Replace("[x", GetRandomSurname());
                }
                if (s.Contains("[m"))
                {
                    temp = temp.Replace("[m", GetRandomName());
                }
                if (s.Contains("[t"))
                {
                    temp = temp.Replace("[t", GetRandomTownName());
                }
                if (s.Contains("[c"))
                {
                    temp = temp.Replace("[c", GetRandomCoutrySideName());
                }
                if (s.Contains("[r"))
                {
                    temp = temp.Replace("[r", GetRandomStreetName());
                }
                res += temp;
            }
            return res;
        }
        /// <summary>
        /// 获取指定位数由数字和字母组成的字符串
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string GetRandomResult(int count)
        {
            string res = "";
            Random ran = new Random(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0));
            for (int i = 0; i < count; i++)
            {
                if (ran.Next(10) % 2 == 0)
                {
                    res += GetStrRandom(1);
                }
                else
                {
                    res += GetIntRandom(1);
                }
            }
            return res;
        }
        /// <summary>
        /// 随机手机号码
        /// </summary>
        /// <returns></returns>
        public static string GetRandomMobile()
        {
            Random ran = new Random(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0));
            string mobile = "";
            //13(0-9) 15(0-9) 18(2-8)
            string h = "130,131,132,133,134,135,136,137,138,139,150,151,152,153,154,155,156,157,158,159,182,183,184,185,186,187,188";
            string[] header = h.Split(',');
            mobile = header[ran.Next(0, header.Length)];
            //132 1232 1232
            string end = "";
            do
            {
                string temp = ran.NextDouble().ToString();
                temp = StrHelper.Abstract(temp, ".", "");
                end = temp;
            } while (end.Length < 9);
            end = end.Substring(0, 8);
            mobile += end;
            return mobile;
        }

        private static object[] CreateRegionCode(int strlength)
        {
            //定义一个字符串数组储存汉字编码的组成元素 
            string[] rBase = new String[16] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f" };
            Random ran = new Random(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0));

            object[] bytes = new object[strlength];
            /**
             每循环一次产生一个含两个元素的十六进制字节数组，并将其放入bytes数组中 
             每个汉字有四个区位码组成 
             区位码第1位和区位码第2位作为字节数组第一个元素 
             区位码第3位和区位码第4位作为字节数组第二个元素 
            **/
            for (int i = 0; i < strlength; i++)
            {
                //区位码第1位 
                int r1 = ran.Next(11, 14);
                string str_r1 = rBase[r1].Trim();

                //区位码第2位 
                ran = new Random(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0)); // 更换随机数发生器的 种子避免产生重复值
                int r2;
                if (r1 == 13)
                {
                    r2 = ran.Next(0, 7);
                }
                else
                {
                    r2 = ran.Next(0, 16);
                }
                string str_r2 = rBase[r2].Trim();

                //区位码第3位 
                ran = new Random(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0)); // 更换随机数发生器的 种子避免产生重复值
                int r3 = ran.Next(10, 16);
                string str_r3 = rBase[r3].Trim();

                //区位码第4位 
                ran = new Random(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0)); // 更换随机数发生器的 种子避免产生重复值
                int r4;
                if (r3 == 10)
                {
                    r4 = ran.Next(1, 16);
                }
                else if (r3 == 15)
                {
                    r4 = ran.Next(0, 15);
                }
                else
                {
                    r4 = ran.Next(0, 16);
                }
                string str_r4 = rBase[r4].Trim();

                // 定义两个字节变量存储产生的随机汉字区位码 
                byte byte1 = Convert.ToByte(str_r1 + str_r2, 16);
                byte byte2 = Convert.ToByte(str_r3 + str_r4, 16);
                // 将两个字节变量存储在字节数组中 
                byte[] str_r = new byte[] { byte1, byte2 };

                // 将产生的一个汉字的字节数组放入object数组中 
                bytes.SetValue(str_r, i);
            }
            return bytes;
        }
    }
}
