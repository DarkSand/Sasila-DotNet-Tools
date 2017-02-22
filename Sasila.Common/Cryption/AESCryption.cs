using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using Sasila.Common.Tools;

namespace Sasila.Common.Cryption
{
    /// <summary>
    /// AES加解密（KEY一般是16位）
    /// </summary>
    public class AESCryption
    {
        #region AES16位ECB模式通用加密

        /// <summary>
        /// AES16位密钥ECB加密
        /// </summary>
        /// <param name="encryptStr">加密的明文</param>
        /// <param name="encryptKey">16位加密key</param>
        /// <returns>返回base64字符串</returns>
        public static string ECBEncrypt(string encryptStr, string encryptKey)
        {
            if (string.IsNullOrEmpty(encryptStr))
            {
                return string.Empty;
            }
            byte[] keyArray = new byte[16];

            byte[] keyArray1 = Encoding.ASCII.GetBytes(encryptKey);
            for (int i = 0; i < keyArray1.Length; i++)
            {
                if (i >= keyArray.Length)
                {
                    break;
                }
                keyArray[i] = keyArray1[i];
            }
            byte[] toEncryptArray = Encoding.ASCII.GetBytes(encryptStr);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        /// AES16位密钥ECB解密
        /// </summary>
        /// <param name="decryptStr">需要解密的字符串（base64）</param>
        /// <param name="decryptKey">16位加密key</param>
        /// <returns>明文</returns>
        public static string ECBDecrypt(string decryptStr, string decryptKey)
        {
            if (string.IsNullOrEmpty(decryptStr))
            {
                return string.Empty;
            }
            byte[] keyArray = new byte[16];

            byte[] keyArray1 = Encoding.ASCII.GetBytes(decryptKey);
            for (int i = 0; i < keyArray1.Length; i++)
            {
                if (i >= keyArray.Length)
                {
                    break;
                }
                keyArray[i] = keyArray1[i];
            }
            byte[] toEncryptArray = Convert.FromBase64String(decryptStr);
            System.Security.Cryptography.RijndaelManaged rm = new System.Security.Cryptography.RijndaelManaged
            {
                Key = keyArray,
                Mode = System.Security.Cryptography.CipherMode.ECB,
                Padding = System.Security.Cryptography.PaddingMode.PKCS7
            };
            System.Security.Cryptography.ICryptoTransform cTransform = rm.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Encoding.ASCII.GetString(resultArray);
        }

        #endregion

        #region AES16位IV(CBC)模式加密

        /// <summary>
        /// AES16位密钥IV加密
        /// </summary>
        /// <param name="encryptStr">加密的明文</param>
        /// <param name="encryptKey">16位加密key</param>
        /// <returns>返回base64字符串</returns>
        public static string Encrypt(byte[] encryptStr, byte[] encryptKey, byte[] encryptIv)
        {
            if (null == encryptStr || encryptStr.Length == 0)
            {
                return string.Empty;
            }
            byte[] keyArray = new byte[16];

            byte[] keyArray1 = encryptKey;
            for (int i = 0; i < keyArray1.Length; i++)
            {
                if (i >= keyArray.Length)
                {
                    break;
                }
                keyArray[i] = keyArray1[i];
            }
            byte[] toEncryptArray = encryptStr;
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.IV = encryptIv;

            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        /// AES16位密钥解密
        /// </summary>
        /// <param name="decryptStr">需要解密的字符串（base64）</param>
        /// <param name="decryptKey">16位加密key</param>
        /// <returns>明文</returns>
        public static string Decrypt(string decryptStr, byte[] decryptKey, byte[] decryptIv)
        {
            if (string.IsNullOrEmpty(decryptStr))
            {
                return string.Empty;
            }
            byte[] keyArray = new byte[16];

            byte[] keyArray1 = decryptKey;
            for (int i = 0; i < keyArray1.Length; i++)
            {
                if (i >= keyArray.Length)
                {
                    break;
                }
                keyArray[i] = keyArray1[i];
            }
            byte[] toEncryptArray = Convert.FromBase64String(decryptStr);
            System.Security.Cryptography.RijndaelManaged rm = new System.Security.Cryptography.RijndaelManaged
            {
                Key = keyArray,
                IV = decryptIv
            };
            System.Security.Cryptography.ICryptoTransform cTransform = rm.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Encoding.UTF8.GetString(resultArray);
        }

        #endregion
    }
}
