using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Sasila.Common.Cryption
{
    /// <summary>
    /// SHA1加密
    /// </summary>
    public class SHA1Cryption
    {
        /// <summary>
        /// SHA1加密
        /// </summary>
        public SHA1Cryption() 
        {
            
        }

        /// <summary>
        /// SHA1加密
        /// </summary>
        /// <param name="encryptStr">明文</param>
        /// <returns>暗码</returns>
        public static string EncryptSHA1(string encryptStr)
        {
            byte[] result = Encoding.Default.GetBytes(encryptStr);
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] output = sha1.ComputeHash(result);
            encryptStr = BitConverter.ToString(output).Replace("-", "");
            return encryptStr;
        }
        /// <summary>
        /// SHA1加密加密字符串
        /// </summary>
        /// <param name="encryptStr">源字符串</param>
        /// <param name="encode">编码</param>
        /// <returns>加密后字符串</returns>
        public static string EncryptSHA1(string encryptStr, Encoding encode)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] bs = encode.GetBytes(encryptStr);
            byte[] hs = sha1.ComputeHash(bs);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hs)
            {
                // 以十六进制格式格式化
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
        /// <summary>
        /// SHA1加密加密字符串
        /// </summary>
        /// <param name="encryptStr">源字符串</param>
        /// <param name="encode">编码</param>
        /// <returns>加密后byte数组</returns>
        public static byte[] EncryptSHA1Bytes(string encryptStr, Encoding encode)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] bs = encode.GetBytes(encryptStr);
            byte[] hs = sha1.ComputeHash(bs);
            return hs;
        }
    }
}
