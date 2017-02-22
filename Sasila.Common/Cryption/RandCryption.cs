using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sasila.Common.Cryption
{
    /// <summary>
    /// 随机数加解密
    /// </summary>
    public class RandCryption
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public RandCryption() { }

        private static uint seed;

        /// <summary>
        /// 模拟随机数
        /// </summary>
        /// <returns></returns>
        private static int Rand()
        {
            seed = seed * 214013 + 2531011;
            return ((int)seed >> 16) & 0x7FFF;
        }

        #region 随机数加密

        /// <summary>
        /// 随机数加密
        /// </summary>
        /// <param name="text">明文</param>
        /// <returns>暗码</returns>
        public static string RandEncrypt(string text)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            uint key1 = 47;
            uint key2 = 147;
            seed = key1;
            for (int i = 0; i < data.Length; ++i)
            {
                data[i] = (byte)~data[i];
                data[i] += (byte)(Rand() % key2);
            }
            StringBuilder ret = new StringBuilder();
            foreach (byte b in data)
            {
                ret.AppendFormat("{0:X2}", b);
            }
            return ret.ToString();
            //return Convert.ToBase64String(data);
        }

        #endregion

        #region 随机数解密

        /// <summary>
        /// 随机数解密
        /// </summary>
        /// <param name="text">暗码</param>
        /// <returns>明文</returns>
        public static string RandDecrypt(string text)
        {
            //byte[] data = Convert.FromBase64String(text);
            int len;
            len = text.Length / 2;
            byte[] data = new byte[len];
            int x, j;
            for (x = 0; x < len; x++)
            {
                j = Convert.ToInt32(text.Substring(x * 2, 2), 16);
                data[x] = (byte)j;
            }
            uint key1 = 47;
            uint key2 = 147;

            seed = key1;
            for (int i = 0; i < data.Length; ++i)
            {
                data[i] -= (byte)(Rand() % key2);
                data[i] = (byte)~data[i];
            }
            return Encoding.UTF8.GetString(data);
        }

        #endregion
    }
}
