using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Sasila.Common.Cryption
{
    /// <summary>
    /// MD5加密
    /// </summary>
    public class MD5Cryption
    {
        /// <summary>
        /// MD5加密
        /// </summary>
        public MD5Cryption() { }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="encryptStr">明文</param>
        /// <returns>暗码</returns>
        public static string EncryptMD5(string encryptStr)
        {
            byte[] result = Encoding.Default.GetBytes(encryptStr);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            encryptStr = BitConverter.ToString(output).Replace("-", "");
            return encryptStr;
        }
        /// <summary>
        /// MD5加密字符串
        /// </summary>
        /// <param name="encryptStr">源字符串</param>
        /// <param name="encode">编码</param>
        /// <returns>加密后字符串</returns>
        public static string EncryptMD5(string encryptStr,Encoding encode)
        {
            // 创建MD5类的默认实例：MD5CryptoServiceProvider
            MD5 md5 = MD5.Create();
            byte[] bs = encode.GetBytes(encryptStr);
            byte[] hs = md5.ComputeHash(bs);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hs)
            {
                // 以十六进制格式格式化
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// MD516位加密
        /// </summary>
        /// <param name="encryptStr">源字符串</param>
        /// <returns>加密后字符串</returns>
        public static string EncryptMD5By16(string encryptStr)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string output = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(encryptStr)), 4, 8);
            output = output.Replace("-", "");
            return output;
        }
    }
}
