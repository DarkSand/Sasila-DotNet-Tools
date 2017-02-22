using System;
using System.Security.Cryptography;
using System.Text;
namespace Sasila.Common.DEncrypt
{
    /// <summary>
    /// DES加解密（KEY一般是8位）
    /// </summary>
    public class DESCryption
    {
        /// <summary>
        /// DES加解密
        /// </summary>
        public DESCryption() { }

        #region DES 趣软加密（CBC模式）

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="Text">明文</param>
        /// <returns>暗码</returns>
        public static string Encrypt(string Text)
        {
            //return Encrypt(Text,"~c^e-s_c#");
            return Encrypt(Text, "cesc");
        }

        /// <summary> 
        /// DES加密
        /// </summary> 
        /// <param name="Text">明文</param> 
        /// <param name="sKey">密匙</param> 
        /// <returns>暗码</returns> 
        public static string Encrypt(string Text, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray;
            inputByteArray = Encoding.Default.GetBytes(Text);
            des.Key = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
            des.IV = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            return ret.ToString();
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="Text">暗码</param>
        /// <returns>明文</returns>
        public static string Decrypt(string Text)
        {
            //return Decrypt(Text, "~c^e-s_c#");
            return Decrypt(Text, "cesc");
        }

        /// <summary> 
        /// DES解密
        /// </summary> 
        /// <param name="Text">暗码</param> 
        /// <param name="sKey">密匙</param> 
        /// <returns>明文</returns> 
        public static string Decrypt(string Text, string sKey)
        {
            if (string.IsNullOrEmpty(Text)) return "";
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            int len;
            len = Text.Length / 2;
            byte[] inputByteArray = new byte[len];
            int x, i;
            for (x = 0; x < len; x++)
            {
                i = Convert.ToInt32(Text.Substring(x * 2, 2), 16);
                inputByteArray[x] = (byte)i;
            }
            des.Key = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
            des.IV = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Encoding.Default.GetString(ms.ToArray());
        }

        #endregion

        #region DES IV(CBC)模式加密

        /// <summary>
        /// DES8位密钥CBC加密
        /// </summary>
        /// <param name="encryptStr">加密的明文</param>
        /// <param name="encryptKey">8位加密key</param>
        /// <param name="encryptIv">8位加密iv</param>
        /// <returns>返回base64字符串</returns>
        public static string CBCEncrypt(string encryptStr, byte[] encryptKey, byte[] encryptIv)
        {
            if (string.IsNullOrEmpty(encryptStr))
            {
                return string.Empty;
            }
            byte[] keyArray = new byte[8];

            byte[] keyArray1 = encryptKey;
            for (int i = 0; i < keyArray1.Length; i++)
            {
                if (i >= keyArray.Length)
                {
                    break;
                }
                keyArray[i] = keyArray1[i];
            }
            byte[] toEncryptArray = Encoding.ASCII.GetBytes(encryptStr);

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = keyArray;
            des.IV = encryptIv;
            des.Mode = CipherMode.CBC;
            des.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = des.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        /// DES8位密钥CBC解密
        /// </summary>
        /// <param name="decryptStr">需要解密的字符串（base64）</param>
        /// <param name="decryptKey">8位加密key</param>
        /// <param name="decryptIv">8位加密iv</param>
        /// <returns>明文</returns>
        public static string CBCDecrypt(string decryptStr, byte[] decryptKey, byte[] decryptIv)
        {
            if (string.IsNullOrEmpty(decryptStr))
            {
                return string.Empty;
            }
            byte[] keyArray = new byte[8];

            byte[] keyArray1 = decryptKey;
            for (int i = 0; i < keyArray1.Length; i++)
            {
                if (i >= keyArray.Length)
                {
                    break;
                }
                keyArray[i] = keyArray1[i];
            }
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = keyArray;
            des.IV = decryptIv;
            des.Mode = CipherMode.CBC;
            des.Padding = PaddingMode.PKCS7;
            byte[] inputByteArray = Convert.FromBase64String(decryptStr);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Encoding.ASCII.GetString(ms.ToArray());
        }

        #endregion

        #region DES8位ECB模式通用加密

        /// <summary>
        /// DES8位密钥ECB加密
        /// </summary>
        /// <param name="encryptStr">加密的明文</param>
        /// <param name="encryptKey">8位加密key</param>
        /// <returns>返回base64字符串</returns>
        public static string ECBEncrypt(string encryptStr, string encryptKey)
        {
            if (null == encryptStr || encryptStr.Length == 0)
            {
                return string.Empty;
            }
            byte[] keyArray = new byte[8];

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

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = keyArray;
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = des.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        /// DES8位密钥ECB加密
        /// </summary>
        /// <param name="encryptBts">加密的明文</param>
        /// <param name="encryptKey">8位加密key</param>
        /// <returns>返回base64字符串</returns>
        public static string ECBEncrypt(byte[] encryptBts, string encryptKey)
        {
            if (null == encryptBts || encryptBts.Length == 0)
            {
                return string.Empty;
            }
            byte[] keyArray = new byte[8];

            byte[] keyArray1 = Encoding.ASCII.GetBytes(encryptKey);
            for (int i = 0; i < keyArray1.Length; i++)
            {
                if (i >= keyArray.Length)
                {
                    break;
                }
                keyArray[i] = keyArray1[i];
            }
            byte[] toEncryptArray = encryptBts;

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = keyArray;
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = des.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        /// DES8位密钥ECB解密
        /// </summary>
        /// <param name="decryptStr">需要解密的base64字符</param>
        /// <param name="decryptKey">8位加密key</param>
        /// <returns>解密后的字节数组</returns>
        public static byte[] ECBDecrypt(string decryptStr, string decryptKey)
        {
            if (null == decryptStr || decryptStr.Length == 0)
            {
                return new byte[0];
            }
            byte[] keyArray = new byte[8];

            byte[] keyArray1 = Encoding.ASCII.GetBytes(decryptKey);
            for (int i = 0; i < keyArray1.Length; i++)
            {
                if (i >= keyArray.Length)
                {
                    break;
                }
                keyArray[i] = keyArray1[i];
            }
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = keyArray;
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;
            byte[] inputByteArray = Convert.FromBase64String(decryptStr);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return ms.ToArray();
        }

        #endregion
    }
}
