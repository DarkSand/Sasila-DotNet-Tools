using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Sasila.Common.Cryption
{
    /// <summary>
    /// DES3加解密（KEY一般是24位）
    /// </summary>
    public class DES3Cryption
    {
        #region DES3 IV(CBC)模式加密

        /// <summary>
        /// DES3 IV(CBC)加密
        /// </summary>
        /// <param name="encryptStr">需加密字符串</param>
        /// <param name="encryptKey">加密KEY</param>
        /// <param name="encryptIv">加密IV</param>
        /// <returns></returns>
        public static string CBCEncrypt(string encryptStr, byte[] encryptKey, byte[] encryptIv)
        {
            if (string.IsNullOrEmpty(encryptStr))
            {
                return string.Empty;
            }
            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();

            DES.Key = encryptKey;
            DES.IV = encryptIv;
            DES.Mode = CipherMode.CBC;
            DES.Padding = PaddingMode.PKCS7;
            ICryptoTransform DESEncrypt = DES.CreateEncryptor();

            byte[] toEncryptArray = Encoding.UTF8.GetBytes(encryptStr);
            return Convert.ToBase64String(DESEncrypt.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length));
        }

        /// <summary>
        /// DES3 IV(CBC)解密
        /// </summary>
        /// <param name="decryptStr">需解密字符串</param>
        /// <param name="decryptKey">解密KEY</param>
        /// <param name="decryptIv">解密IV</param>
        /// <returns></returns>
        public static string CBCDecrypt(string decryptStr, byte[] decryptKey, byte[] decryptIv)
        {
            if (string.IsNullOrEmpty(decryptStr))
            {
                return string.Empty;
            }
            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();

            DES.Key = decryptKey;
            DES.IV = decryptIv;
            DES.Mode = CipherMode.CBC;
            DES.Padding = System.Security.Cryptography.PaddingMode.PKCS7;

            ICryptoTransform DESDecrypt = DES.CreateDecryptor();

            byte[] todecrypArray = Convert.FromBase64String(decryptStr);
            return Encoding.UTF8.GetString(DESDecrypt.TransformFinalBlock(todecrypArray, 0, todecrypArray.Length));
        }

        #endregion

        #region DES3 ECB模式通用加密

        /// <summary>
        /// DES8位密钥ECB加密
        /// </summary>
        /// <param name="encryptStr">加密的明文</param>
        /// <param name="encryptKey">24位加密key</param>
        /// <returns>返回base64字符串</returns>
        public static string ECBEncrypt(string encryptStr, byte[] encryptKey)
        {
            if (string.IsNullOrEmpty(encryptStr))
            {
                return string.Empty;
            }
            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();

            DES.Key = encryptKey;
            DES.Mode = CipherMode.ECB;
            DES.Padding = PaddingMode.PKCS7;
            ICryptoTransform DESEncrypt = DES.CreateEncryptor();

            byte[] toEncryptArray = ASCIIEncoding.UTF8.GetBytes(encryptStr);
            return Convert.ToBase64String(DESEncrypt.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length));
        }

        /// <summary>
        /// DES8位密钥ECB解密
        /// </summary>
        /// <param name="decryptStr">需要解密的字符串（base64）</param>
        /// <param name="decryptKey">24位加密key</param>
        /// <returns>明文</returns>
        public static string ECBDecrypt(string decryptStr, byte[] decryptKey)
        {
            if (string.IsNullOrEmpty(decryptStr))
            {
                return string.Empty;
            }
            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();

            DES.Key = decryptKey;
            DES.Mode = CipherMode.ECB;
            DES.Padding = System.Security.Cryptography.PaddingMode.PKCS7;

            ICryptoTransform DESDecrypt = DES.CreateDecryptor();

            byte[] todecrypArray = Convert.FromBase64String(decryptStr);
            return ASCIIEncoding.UTF8.GetString(DESDecrypt.TransformFinalBlock(todecrypArray, 0, todecrypArray.Length));
        }

        #endregion
    }
}
